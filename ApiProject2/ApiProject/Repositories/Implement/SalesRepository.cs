using ApiProject.Data;
using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using static ApiProject.Repositories.Implement.SalesRepository;

namespace ApiProject.Repositories.Implement
{
    public class SalesRepository : ISalesRepository
    {
        private readonly ProjectContext _context;
        public SalesRepository(ProjectContext context)
        {
            _context = context;
        }
        //צפייה ברכישות כרטיסים עבור כל מתנה.
        public async Task<List<SalesDto.GiftResume>> GetGiftResumes()
        {
            return await _context.cartItems.Where(x => x.Cart.MyStatus == Status.Purchase)
                .GroupBy(x => new
                {
                    x.GiftModelId,
                    x.Gift.Name,
                    x.Gift.TicketPrice
                }).Select(x => new SalesDto.GiftResume
                {
                    GiftId = x.Key.GiftModelId,
                    GiftName = x.Key.Name,
                    GiftPrice = x.Key.TicketPrice,
                    TotalTicketsPurchase = x.Sum(a => a.Quantity),
                    TotalIncome = x.Sum(a => a.Quantity * a.Gift.TicketPrice),
                }).ToListAsync();
        }


        public async Task<List<SalesDto.GiftsWithBuyers>> GetGiftWithBuyers()
        {
            var cartItems = await _context.cartItems
             .Where(ci => ci.Cart.MyStatus == Status.Purchase)
             .Include(ci => ci.Cart)
                 .ThenInclude(c => c.User)
             .Include(ci => ci.Gift)
             .ToListAsync();


            var result = cartItems
                .GroupBy(ci => new { ci.GiftModelId, ci.Gift.Name, ci.Gift.TicketPrice })
                .Select(g => new SalesDto.GiftsWithBuyers
                {
                    GiftId = g.Key.GiftModelId,
                    GiftName = g.Key.Name,
                    GiftPrice = g.Key.TicketPrice,
                    Buyers = g.GroupBy(ci => new
                    {
                        ci.Cart.User.Id,
                        ci.Cart.User.FirstName,
                        ci.Cart.User.LastName,
                        ci.Cart.User.Email
                    })
                              .Select(u => new SalesDto.BuyerInfo
                              {
                                  UserId = u.Key.Id,
                                  FirstName = u.Key.FirstName,
                                  LastName = u.Key.LastName,
                                  Email = u.Key.Email,
                                  TicketsPurshased = u.Sum(x => x.Quantity)
                              }).ToList()
                }).ToList();

            return result;
        }


        // פונקציה פרטית שמבצעת את הקיבוץ והחישוב של כל המתנות עם הרוכשים
        private async Task<List<SalesDto.GiftsWithBuyers>> GetAllGiftsWithBuyers()
        {
            var cartItems = await _context.cartItems
                .Where(ci => ci.Cart.MyStatus == Status.Purchase)
                .Include(ci => ci.Cart)
                    .ThenInclude(c => c.User)
                .Include(ci => ci.Gift)
                .ToListAsync();

            var result = cartItems
                .GroupBy(ci => new { ci.GiftModelId, ci.Gift.Name, ci.Gift.TicketPrice })
                .Select(g => new SalesDto.GiftsWithBuyers
                {
                    GiftId = g.Key.GiftModelId,
                    GiftName = g.Key.Name,
                    GiftPrice = g.Key.TicketPrice,
                    Buyers = g.GroupBy(ci => new
                    {
                        ci.Cart.User.Id,
                        ci.Cart.User.FirstName,
                        ci.Cart.User.LastName,
                        ci.Cart.User.Email
                    })
                    .Select(u => new SalesDto.BuyerInfo
                    {
                        UserId = u.Key.Id,
                        FirstName = u.Key.FirstName,
                        LastName = u.Key.LastName,
                        Email = u.Key.Email,
                        TicketsPurshased = u.Sum(x => x.Quantity)
                    }).ToList()
                }).ToList();

            return result;
        }

        //  מיון לפי מחיר מהגבוה לנמוך
        public async Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByPriceDesc()
        {
            var gifts = await GetAllGiftsWithBuyers();
            return gifts.OrderByDescending(g => g.GiftPrice).ToList();
        }

        //  מיון לפי כמות כרטיסים מהגבוה לנמוך
        public async Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByQuantityDesc()
        {
            var gifts = await GetAllGiftsWithBuyers();
            return gifts.OrderByDescending(g => g.Buyers.Sum(b => b.TicketsPurshased)).ToList();
        }


    }
}
