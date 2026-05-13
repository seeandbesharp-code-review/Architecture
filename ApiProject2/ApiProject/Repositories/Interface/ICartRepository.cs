using ApiProject.DTO;
using ApiProject.Models;

namespace ApiProject.Repositories.Interface
{
    public interface ICartRepository
    {
        Task<CartItemModel> AddGiftToCart(CartItemModel cartItem);
        Task<CartItemModel> CheckIfUserHaveThisGift(int cartId, int giftId);
        Task<CartModel> CreateCart(CartModel cart);
        Task<CartModel> DeleteGiftFromCart(int userId, int giftId);
        Task<List<CartDto.CartModelDto>> GetAllPaidCartDtosByUserId(int userId);
        Task<CartModel> GetCartByUserId(int userId);
        Task<CartItemModel> GetCartItemById(int cartItemId);
        Task<CartModel> GetOrCreateDraftCartByUserId(int userId);
        Task<CartModel> GetPaidCartByUserId(int userId);
        Task<CartModel> GetUnpaidCartByUserId(int userId);
        Task<CartModel> PurchaseCart(CartModel cart);
        Task<CartModel> UpdateCart(CartModel cart);
        Task<CartItemModel> UpdateGiftToCart(CartItemModel cartItem);
        Task<bool> CheckGiftIsRaffled(int giftId);
    }
}