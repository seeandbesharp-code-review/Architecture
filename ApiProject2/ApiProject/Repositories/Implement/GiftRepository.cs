using ApiProject.Data;
using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiProject.Repositories.Implement
{
    public class GiftRepository : IGiftRepository
    {
        private readonly ProjectContext _context;
        private readonly ILogger<GiftRepository> _logger;

        public GiftRepository(ProjectContext context, ILogger<GiftRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<GiftModel>> GetAllGifts()
        {
            try
            {
                _logger.LogInformation("Fetching all gifts");
                return await _context.gifts
                    .Include(a => a.DonorModel)
                    .Include(a => a.CategoryModel)
                    .Include(a => a.UserModel)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all gifts");
                throw new Exception("שגיאה בשליפת רשימת המתנות", ex);
            }
        }

        public async Task<GiftModel> GetGiftById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching gift with id {GiftId}", id);
                return await _context.gifts
                    .Include(a => a.DonorModel)
                    .Include(a => a.CategoryModel)
                    .Include(a => a.UserModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching gift with id {GiftId}", id);
                throw new Exception($"שגיאה בשליפת מתנה מספר {id}", ex);
            }
        }

        public async Task<string> AddGift(GiftModel gift)
        {
            try
            {
                _logger.LogInformation("Adding new gift");

                await _context.gifts.AddAsync(gift);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Gift added successfully with id {GiftId}", gift.Id);

                return "add to list";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new gift");
                throw new Exception("שגיאה בהוספת מתנה חדשה", ex);
            }
        }

        public async Task<bool> CheckDonorExist(int id)
        {
            return await _context.donors.AnyAsync(d => d.Id == id);
        }
        public async Task<bool> CheckNameExist(string name)
        {
            return await _context.gifts.AnyAsync(d => d.Name == name);
        }


        public async Task<bool> CheckCategoryExist(int categoryId)
        {
            return await _context.categories.AnyAsync(c => c.Id == categoryId);
        }

        public async Task<IEnumerable<GiftModel>> DeleteGift(int id)
        {
            try
            {
                _logger.LogInformation("Deleting gift with id {GiftId}", id);

                var gift = await _context.gifts.FindAsync(id);
                if (gift == null)
                {
                    _logger.LogWarning("Gift with id {GiftId} not found for deletion", id);
                    return null;
                }

                _context.gifts.Remove(gift);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Gift with id {GiftId} deleted successfully", id);

                return await GetAllGifts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting gift with id {GiftId}", id);
                throw new Exception($"שגיאה במחיקת מתנה מספר {id}", ex);
            }
        }
        public async Task<IEnumerable<CartItemModel>> CheckGiftPayed(int id)
        {
            return await _context.cartItems
                .Include(a => a.Gift)
                .Include(a => a.Cart)
                .Where(a => a.GiftModelId == id && a.Cart.MyStatus == Status.Purchase)
                .ToListAsync();
        }

        public async Task<GiftModel> UpdateGift(GiftModel gift, int id)
        {
            try
            {
                _logger.LogInformation("Updating gift with id {GiftId}", id);

                _context.gifts.Update(gift);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Gift with id {GiftId} updated successfully", id);

                return gift;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating gift with id {GiftId}", id);
                throw new Exception($"שגיאה בעדכון מתנה מספר {id}", ex);
            }
        }

        public async Task<string> RuffleGift(GiftModel gift)
        {
            try
            {
                _logger.LogInformation("Ruffling gift with id {GiftId}", gift.Id);

                _context.gifts.Update(gift);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Gift with id {GiftId} ruffled successfully", gift.Id);

                return "the gift ruffled";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ruffling gift with id {GiftId}", gift.Id);
                throw new Exception("שגיאה בעדכון סטטוס הגרלה למתנה", ex);
            }
        }

        public async Task<IEnumerable<GiftDto.GiftBuyersData>> SortByBuyers()
        {
            try
            {
                _logger.LogInformation("Fetching gifts sorted by buyers");
                return await _context.cartItems
                    .Include(a => a.Cart)
                    .Include(a => a.Gift)
                        .ThenInclude(g => g.CategoryModel)
                    .Include(a => a.Gift)
                        .ThenInclude(g => g.DonorModel)
                    .Where(a => a.Cart.MyStatus == Status.Purchase)
                    .GroupBy(a => a.GiftModelId)
                    .Select(g => new GiftDto.GiftBuyersData
                    {
                        Gift = g.First().Gift,
                        TotalTickets = g.Sum(x => x.Quantity),
                        PurchaseCount = g.Count()
                    })
                    .OrderByDescending(x => x.TotalTickets)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching gifts sorted by buyers");
                throw new Exception("שגיאה בשליפת כמות הרכישות", ex);
            }
        }
    }
}
