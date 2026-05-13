using ApiProject.Data;
using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiProject.Repositories.Implement
{
    public class LotteryRepository : ILotteryRepository
    {
        private readonly ProjectContext _context;
        private readonly ILogger<LotteryRepository> _logger;

        public LotteryRepository(ProjectContext context, ILogger<LotteryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RaffleDto.RaffleEntry>> GetRaffleEntries(int giftId)
        {
            try
            {
                _logger.LogInformation("Fetching raffle entries for gift id {GiftId}", giftId);

                return await _context.cartItems
                    .Where(a => a.GiftModelId == giftId && a.Cart.MyStatus == Status.Purchase)
                    .GroupBy(a => a.Cart.UserModelId)
                    .Select(a => new RaffleDto.RaffleEntry
                    {
                        UserId = a.Key,
                        Quantity = a.Sum(x => x.Quantity)
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching raffle entries for gift id {GiftId}", giftId);
                throw new Exception($"שגיאה בשליפת משתתפי ההגרלה עבור מתנה {giftId}", ex);
            }
        }

        public async Task<UserModel> GetWinnerDetails(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching winner details for user id {UserId}", userId);
                return await _context.users.FirstOrDefaultAsync(a => a.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching winner details for user id {UserId}", userId);
                throw new Exception($"שגיאה בשליפת פרטי הזוכה (מזהה: {userId})", ex);
            }
        }

        public async Task<string> SetWinner(int giftId, int winnerUserId)
        {
            try
            {
                _logger.LogInformation("Setting winner for gift id {GiftId} to user id {WinnerUserId}", giftId, winnerUserId);

                var gift = await _context.gifts.FirstOrDefaultAsync(a => a.Id == giftId);

                if (gift == null)
                {
                    _logger.LogWarning("Gift with id {GiftId} not found", giftId);
                    throw new Exception("מתנה לא נמצאה במערכת");
                }

                if (gift.isRaffleDone == true)
                {
                    _logger.LogWarning("Gift with id {GiftId} already raffled", giftId);
                    throw new Exception("לא ניתן לבצע הגרלה חוזרת - ההגרלה כבר בוצעה");
                }

                gift.UserModelId = winnerUserId;
                gift.isRaffleDone = true;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Winner set successfully for gift id {GiftId}", giftId);

                return "Changes saved";
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("מתנה")))
            {
                _logger.LogError(ex, "Error setting winner for gift id {GiftId}", giftId);
                throw new Exception("שגיאה בתהליך עדכון הזוכה במסד הנתונים", ex);
            }
            catch { throw; }
        }
    }
}
