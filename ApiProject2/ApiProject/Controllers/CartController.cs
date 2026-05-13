using ApiProject.Attributes;
using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Services;
using ApiProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _context;
        public CartController(ICartService context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<CartDto.CartModelDto>> GetCartById(int userId)
        {
            try
            {
                var result = await _context.GetCartByUserId(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching cart for user {userId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("unpaid/{userId}")]
        public async Task<ActionResult<CartDto.CartModelDto>> GetUnpaidCartByUserId(int userId)
        {
            try
            {
                var result = await _context.GetUnpaidCartByUserId(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching unpaid cart for user {userId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("paid/{userId}")]
        public async Task<ActionResult<CartDto.CartModelDto>> GetPaidCartByUserId(int userId)
        {
            try
            {
                var result = await _context.GetPaidCartByUserId(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching paid cart for user {userId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("addGift")]
        public async Task<ActionResult<CartDto.CartModelDto>> AddGiftToCart([FromBody] CartDto.AddCartItemDto addCartItem)
        {
            try
            {
                var result = await _context.AddGiftToCart(addCartItem);
                if (result == null)
                    return BadRequest("לא ניתן לרכוש מתנה זו");
                Console.WriteLine($"Gift {addCartItem.GiftId} added to cart for user {addCartItem.UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding gift {addCartItem.GiftId} to cart for user {addCartItem.UserId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("plusOne")]
        public async Task<ActionResult<CartDto.CartModelDto>> PlusOne([FromBody] CartDto.AddCartItemDto plusOne)
        {
            try
            {
                var result = await _context.PlusOneGiftToCart(plusOne);
                Console.WriteLine($"Increased quantity of gift {plusOne.GiftId} for user {plusOne.UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error increasing gift {plusOne.GiftId} for user {plusOne.UserId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("minusOne")]
        public async Task<ActionResult<CartDto.CartModelDto>> MinusOne([FromBody] CartDto.AddCartItemDto minusOne)
        {
            try
            {
                var result = await _context.MinusOneGiftToCart(minusOne);
                Console.WriteLine($"Decreased quantity of gift {minusOne.GiftId} for user {minusOne.UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decreasing gift {minusOne.GiftId} for user {minusOne.UserId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("deleteGift")]
        public async Task<ActionResult<CartDto.CartModelDto>> DeleteGiftFromCart(int userId, int giftId)
        {
            try
            {
                var result = await _context.DeleteGiftFromCart(userId, giftId);
                Console.WriteLine($"Deleted gift {giftId} from cart for user {userId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting gift {giftId} from cart for user {userId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("updateGift")]
        public async Task<ActionResult<CartDto.CartModelDto>> UpdateGiftToCart([FromBody] CartDto.UpdateCartItemDto updateCartItem)
        {
            try
            {
                var result = await _context.UpdateGiftToCart(updateCartItem);
                Console.WriteLine($"Updated gift {updateCartItem.GiftId} in cart for user {updateCartItem.UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating gift {updateCartItem.GiftId} for user {updateCartItem.UserId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("purchase")]
        public async Task<ActionResult<CartDto.CartModelDto>> PurchaseCart(int userId)
        {
            try
            {
                var result = await _context.PurchaseCart(userId);
                Console.WriteLine($"Cart purchased for user {userId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error purchasing cart for user {userId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
