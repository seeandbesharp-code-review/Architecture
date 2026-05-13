using ApiProject.DTO;

namespace ApiProject.Services.Interface
{
    public interface ICartService
    {
        Task<CartDto.CartModelDto> AddGiftToCart(CartDto.AddCartItemDto newItem);
        Task<CartDto.CartModelDto> CreateCart(int UserId);
        Task<CartDto.CartModelDto> DeleteGiftFromCart(int userId, int giftId);
        Task<CartDto.CartModelDto> GetCartByUserId(int userId);
        Task<CartDto.CartItemDto> GetCartItemByGiftId(int userId, int giftId);
        Task<List<CartDto.CartModelDto>> GetPaidCartByUserId(int userId);
        Task<CartDto.CartModelDto> GetUnpaidCartByUserId(int userId);
        Task<CartDto.CartModelDto> MergeGuestCart(int userId, List<CartDto.MergeDto> guestItems);
        Task<CartDto.CartModelDto> MinusOneGiftToCart(CartDto.AddCartItemDto addCartItem);
        Task<CartDto.CartModelDto> PlusOneGiftToCart(CartDto.AddCartItemDto addCartItem);
        Task<List<CartDto.CartModelDto>> PurchaseCart(int userId);
        Task<CartDto.CartModelDto> UpdateGiftToCart(CartDto.UpdateCartItemDto updateCartItem);
    }
}