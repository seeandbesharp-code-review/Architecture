using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using ApiProject.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;

namespace ApiProject.Services.Implement
{
    public class GiftsService : IGiftsService
    {
        private readonly IGiftRepository _repository;
        private readonly ILogger<GiftsService> _logger;
        private readonly ICacheService _cache;
        private readonly TimeSpan _ttl;

        // מפתחות Cache קבועים — מונעים שגיאות typo
        private const string AllGiftsKey = "gifts:all";
        private static string GiftKey(int id) => $"gifts:{id}";

        public GiftsService(IGiftRepository repository, ILogger<GiftsService> logger,
            ICacheService cache, IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;
            _ttl = TimeSpan.FromSeconds(config.GetValue<int>("Redis:GiftsTtlSeconds", 60));
        }

        public async Task<IEnumerable<GiftDto.GiftModelDto>> getAllGifts()
        {
            try
            {
                // שלב 1: בדוק ב-Cache
                var cached = await _cache.GetAsync<IEnumerable<GiftDto.GiftModelDto>>(AllGiftsKey);
                if (cached != null)
                    return cached;

                // שלב 2: Cache Miss → לך ל-DB
                var gifts = await _repository.GetAllGifts();
                var result = gifts.Select(MapToDto).ToList();

                // שלב 3: שמור ב-Cache לפעם הבאה
                await _cache.SetAsync(AllGiftsKey, result, _ttl);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all gifts");
                throw;
            }
        }

        public async Task<IEnumerable<GiftDto.GiftModelDto>> SortByName(string name)
        {
            try
            {
                // פעולות סינון עובדות על הרשימה הכללית (שכבר מגיעה מ-Cache אם קיים)
                var gifts = await getAllGifts();
                return gifts.Where(a => a.Name.Contains(name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all gifts");
                throw;
            }
        }

        public async Task<IEnumerable<GiftDto.GiftModelDto>> SortByDonorName(string name)
        {
            try
            {
                var gifts = await getAllGifts();
                return gifts.Where(a => a.Donor.Contains(name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all gifts");
                throw;
            }
        }

        public async Task<IEnumerable<GiftDto.GiftModelDto>> SortByCategory(int id)
        {
            try
            {
                var gifts = await _repository.GetAllGifts();
                return gifts.Where(a => a.CategoryModel.Id == id).Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all gifts");
                throw;
            }
        }

        public async Task<IEnumerable<GiftDto.GiftToManager>> SortByBuyers()
        {
            try
            {
                var gifts = await _repository.SortByBuyers();

                return gifts.Select(a => new GiftDto.GiftToManager
                {
                    Name = a.Gift.Name,
                    Category = a.Gift.CategoryModel.Name,
                    Image = a.Gift.Image,
                    TicketPrice = a.Gift.TicketPrice,
                    Donor = a.Gift.DonorModel.FirstName + " " + a.Gift.DonorModel.LastName,
                    TotalTickets = a.TotalTickets,
                    PurchaseCount = a.PurchaseCount,
                    TotalSum = a.TotalTickets * a.Gift.TicketPrice
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching gifts sorted by buyers");
                throw;
            }
        }

        public async Task<IEnumerable<GiftDto.GiftModelDto>> OrderByPrice()
        {
            try
            {
                var gifts = await getAllGifts();
                return gifts.OrderBy(a => a.TicketPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all gifts");
                throw;
            }
        }

        public async Task<GiftDto.GiftModelDto> GetGiftById(int id)
        {
            try
            {
                // Cache לפי מזהה בודד
                var cached = await _cache.GetAsync<GiftDto.GiftModelDto>(GiftKey(id));
                if (cached != null)
                    return cached;

                var gift = await _repository.GetGiftById(id);
                if (gift == null)
                {
                    _logger.LogWarning("Gift with ID {Id} was not found", id);
                    return null;
                }

                var result = MapToDto(gift);
                await _cache.SetAsync(GiftKey(id), result, _ttl);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching gift {Id}", id);
                throw;
            }
        }

        public async Task<string> AddGift(GiftDto.AddGiftDto gift)
        {
            try
            {
                _logger.LogInformation("Attempting to add a new gift: {GiftName}", gift.Name);

                if (!await _repository.CheckDonorExist(gift.DonorId))
                {
                    _logger.LogWarning("AddGift failed: Donor ID {DonorId} does not exist", gift.DonorId);
                    return "donor is not exist";
                }

                if (!await _repository.CheckCategoryExist(gift.CategoryId))
                {
                    _logger.LogWarning("AddGift failed: Category ID {CategoryId} does not exist", gift.CategoryId);
                    return "category is not exist";
                }
                if (await _repository.CheckNameExist(gift.Name))
                {
                    _logger.LogWarning("AddGift failed: Name {Name} already exist", gift.CategoryId);
                    return null;
                }

                var giftToAdd = new GiftModel
                {
                    Name = gift.Name,
                    Description = gift.Description,
                    TicketPrice = gift.TicketPrice,
                    Image = gift.Image,
                    DonorModelId = gift.DonorId,
                    CategoryModelId = gift.CategoryId,
                    isRaffleDone = false
                };

                var result = await _repository.AddGift(giftToAdd);

                // Invalidation: הרשימה הכללית השתנתה → מחק מה-Cache
                await _cache.RemoveAsync(AllGiftsKey);

                _logger.LogInformation("Gift {GiftName} added successfully", gift.Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding gift {GiftName}", gift.Name);
                throw;
            }
        }

        public async Task<IEnumerable<GiftDto.GiftModelDto>> DeleteGift(int id)
        {
            try
            {
                var cartItems = await _repository.CheckGiftPayed(id);
                if (cartItems.Any())
                    return null;

                _logger.LogInformation("Attempting to delete gift ID {Id}", id);
                await _repository.DeleteGift(id);

                // Invalidation: מחק את המתנה הספציפית ואת הרשימה הכללית
                await _cache.RemoveAsync(GiftKey(id));
                await _cache.RemoveAsync(AllGiftsKey);

                return await getAllGifts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting gift {Id}", id);
                throw;
            }
        }

        public async Task<GiftDto.GiftModelDto> UpdateGift(GiftDto.UpdateGiftDto updateGiftDto, int id)
        {
            try
            {
                var existingGift = await _repository.GetGiftById(id);
                if (existingGift == null) return null;

                var cartItems = await _repository.CheckGiftPayed(id);
                if (cartItems.Any())
                    return null;

                if (!await _repository.CheckCategoryExist(updateGiftDto.CategoryId))
                {
                    _logger.LogWarning("UpdateGift failed: Category ID {CategoryId} not found", updateGiftDto.CategoryId);
                    return null;
                }
                if (await _repository.CheckNameExist(updateGiftDto.Name))
                {
                    _logger.LogWarning("AddGift failed: Name {Name} already exist", updateGiftDto.CategoryId);
                    return null;
                }

                existingGift.Name = updateGiftDto.Name;
                existingGift.Description = updateGiftDto.Description;
                existingGift.TicketPrice = updateGiftDto.TicketPrice;
                existingGift.Image = updateGiftDto.Image;
                existingGift.DonorModelId = updateGiftDto.DonorId;
                existingGift.CategoryModelId = updateGiftDto.CategoryId;

                await _repository.UpdateGift(existingGift, id);

                // Invalidation: הנתון השתנה → מחק שניהם
                await _cache.RemoveAsync(GiftKey(id));
                await _cache.RemoveAsync(AllGiftsKey);

                return await GetGiftById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating gift {Id}", id);
                throw;
            }
        }

        public async Task<string> RuffleGift(GiftDto.GiftRaffled gift)
        {
            try
            {
                _logger.LogInformation("Starting raffle for gift ID {Id}", gift.Id);
                var giftRaffle = await _repository.GetGiftById(gift.Id);

                if (giftRaffle == null)
                {
                    _logger.LogWarning("Raffle failed: Gift ID {Id} not found", gift.Id);
                    return null;
                }

                giftRaffle.isRaffleDone = true;
                await _repository.UpdateGift(giftRaffle, gift.Id);

                var winner = await _repository.RuffleGift(giftRaffle);

                // Invalidation: מצב המתנה השתנה (isRaffleDone = true)
                await _cache.RemoveAsync(GiftKey(gift.Id));
                await _cache.RemoveAsync(AllGiftsKey);

                _logger.LogInformation("Raffle completed for {GiftName}. Winner: {Winner}", giftRaffle.Name, winner);
                return winner;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during raffle for gift {Id}", gift.Id);
                throw;
            }
        }

        private static GiftDto.GiftModelDto MapToDto(GiftModel g)
        {
            return new GiftDto.GiftModelDto
            {
                Id = g.Id,
                Name = g.Name,
                Category = g.CategoryModel?.Name ?? "Unknown",
                Description = g.Description,
                TicketPrice = g.TicketPrice,
                Image = g.Image,
                Donor = g.DonorModel != null ? $"{g.DonorModel.FirstName} {g.DonorModel.LastName}" : "Unknown Donor",
                WinnerName = g.isRaffleDone == true ? g.UserModel.FirstName + " " + g.UserModel.LastName : "אולי אתה תהיה הזוכה"
            };
        }
    }
}
