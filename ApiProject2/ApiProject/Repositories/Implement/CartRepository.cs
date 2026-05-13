using ApiProject.Data;
using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiProject.Repositories.Implement
{
    public class CartRepository : ICartRepository
    {
        private readonly ProjectContext _context;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(ProjectContext context, ILogger<CartRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartModel> CreateCart(CartModel cart)
        {
            try
            {
                _context.carts.Add(cart);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Cart created for user {UserId}, CartId: {CartId}",
                    cart.UserModelId,
                    cart.Id
                );

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating cart for user {UserId}",
                    cart.UserModelId
                );
                throw;
            }
        }

        public async Task<CartModel> GetCartByUserId(int userId)
        {
            try
            {
                return await _context.carts
                    .Include(a => a.CartItem)
                    .ThenInclude(x => x.Gift)
                    .FirstOrDefaultAsync(b => b.UserModelId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching cart for user {UserId}",
                    userId
                );
                throw;
            }
        }

        public async Task<CartModel> GetUnpaidCartByUserId(int userId)
        {
            try
            {
                return await _context.carts
                    .Include(a => a.CartItem)
                    .ThenInclude(b => b.Gift)
                    .FirstOrDefaultAsync(x => x.UserModelId == userId && x.MyStatus == Status.Draft);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching unpaid cart for user {UserId}",
                    userId
                );
                throw;
            }
        }

        public async Task<CartModel> GetPaidCartByUserId(int userId)
        {
            try
            {
                return await _context.carts
                    .Include(a => a.CartItem)
                    .ThenInclude(b => b.Gift)
                    .FirstOrDefaultAsync(x => x.UserModelId == userId && x.MyStatus == Status.Purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching paid cart for user {UserId}",
                    userId
                );
                throw;
            }
        }

        public async Task<List<CartDto.CartModelDto>> GetAllPaidCartDtosByUserId(int userId)
        {
            try
            {
                return await _context.carts
                    .Where(c => c.UserModelId == userId && c.MyStatus == Status.Purchase)
                    .Include(c => c.CartItem)
                        .ThenInclude(ci => ci.Gift).ThenInclude(a=> a.CategoryModel)
                    .Select(cart => new CartDto.CartModelDto
                    {
                        Id = cart.Id,
                        UserId = cart.UserModelId,
                        Status = cart.MyStatus,
                        Items = cart.CartItem.Select(item => new CartDto.CartItemDto
                        {
                            GiftId = item.GiftModelId,
                            GiftName = item.Gift.Name,
                            Quantity = item.Quantity,
                            TicketPrice = item.Gift.TicketPrice,
                            Category = item.Gift.CategoryModel.Name,
                            Image = item.Gift.Image
                        }).ToList()
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching all paid carts for user {UserId}",
                    userId
                );
                throw;
            }
        }

        public async Task<CartModel> GetOrCreateDraftCartByUserId(int userId)
        {
            try
            {
                var cart = await _context.carts
                    .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.Gift)
                    .FirstOrDefaultAsync(c => c.UserModelId == userId && c.MyStatus == Status.Draft);

                if (cart == null)
                {
                    cart = new CartModel
                    {
                        UserModelId = userId,
                        MyStatus = Status.Draft,
                        CartItem = new List<CartItemModel>()
                    };

                    await CreateCart(cart);

                    _logger.LogInformation(
                        "Draft cart created for user {UserId}",
                        userId
                    );
                }

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting or creating draft cart for user {UserId}",
                    userId
                );
                throw;
            }
        }

        public async Task<CartItemModel> AddGiftToCart(CartItemModel cartItem)
        {
            try
            {
                await _context.cartItems.AddAsync(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Gift {GiftId} added to cart {CartId}",
                    cartItem.GiftModelId,
                    cartItem.CartId
                );

                return cartItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error adding gift {GiftId} to cart {CartId}",
                    cartItem.GiftModelId,
                    cartItem.CartId
                );
                throw;
            }
        }

        public async Task<CartItemModel> UpdateGiftToCart(CartItemModel cartItem)
        {
            try
            {
                _context.cartItems.Update(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Updated gift {GiftId} in cart {CartId}",
                    cartItem.GiftModelId,
                    cartItem.CartId
                );

                return cartItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating gift {GiftId} in cart {CartId}",
                    cartItem.GiftModelId,
                    cartItem.CartId
                );
                throw;
            }
        }

        public async Task<CartModel> DeleteGiftFromCart(int userId, int giftId)
        {
            try
            {
                var cart = await _context.carts
                    .Include(a => a.CartItem)
                    .ThenInclude(b => b.Gift)
                    .FirstOrDefaultAsync(x => x.UserModelId == userId && x.MyStatus == Status.Draft);

                if (cart == null) return null;

                var item = cart.CartItem.FirstOrDefault(ci => ci.GiftModelId == giftId);
                if (item != null)
                {
                    _context.cartItems.Remove(item);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Deleted gift {GiftId} from cart for user {UserId}",
                        giftId,
                        userId
                    );
                }

                return await GetCartByUserId(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting gift {GiftId} from cart for user {UserId}",
                    giftId,
                    userId
                );
                throw;
            }
        }

        public async Task<CartItemModel> GetCartItemById(int cartItemId)
        {
            try
            {
                return await _context.cartItems
                    .Include(ci => ci.Gift)
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching cart item {CartItemId}",
                    cartItemId
                );
                throw;
            }
        }

        public async Task<CartModel> PurchaseCart(CartModel cart)
        {
            try
            {
                cart.MyStatus = Status.Purchase;
                _context.carts.Update(cart);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Cart {CartId} purchased for user {UserId}",
                    cart.Id,
                    cart.UserModelId
                );

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error purchasing cart {CartId} for user {UserId}",
                    cart.Id,
                    cart.UserModelId
                );
                throw;
            }
        }

        public async Task<CartModel> UpdateCart(CartModel cart)
        {
            try
            {
                _context.carts.Update(cart);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Updated cart {CartId} for user {UserId}",
                    cart.Id,
                    cart.UserModelId
                );

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating cart {CartId} for user {UserId}",
                    cart.Id,
                    cart.UserModelId
                );
                throw;
            }
        }

        public async Task<CartItemModel> CheckIfUserHaveThisGift(int cartId, int giftId)
        {
            try
            {
                return await _context.cartItems
                    .Include(x => x.Gift)
                    .FirstOrDefaultAsync(a => a.CartId == cartId && a.GiftModelId == giftId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error checking gift {GiftId} in cart {CartId}",
                    giftId,
                    cartId
                );
                throw;
            }
        }

        public async Task<bool> CheckGiftIsRaffled(int giftId)
        {
            var gifts = await _context.gifts.FirstOrDefaultAsync(a => a.Id == giftId);
            if (gifts.isRaffleDone == true)
                return true;

            return false;
        }
    }
}
