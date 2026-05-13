using ApiProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiProject.DTO
{
    public class CartDto
    {
        public class AddCartItemDto
        {
            [Required]
            public int UserId { get; set; }
            [Required]
            public int GiftId { get; set; }

        }
        public class UpdateCartItemDto
        {
            [Required]
            public int UserId { get; set; }

            [Required]
            public int GiftId { get; set; }

            [Required, Range(1, 50)]
            public int Quantity { get; set; }

        }
        public class CartItemDto
        {
            public int GiftId { get; set; }
            public string GiftName { get; set; }//לשים לב לקשר אותו למתנה
            public int Quantity { get; set; }

            public int TicketPrice { get; set; }
            public string Category { get; set; }
            public string Image { get; set; }
        }
        public class CartModelDto
        {
            public int Id { get; set; }//id של העגלה
            public int UserId { get; set; }//id של המשתמש
            public Status Status { get; set; } // Draft / Purchased
            public List<CartItemDto> Items { get; set; }

        }
        public class CrateCartDto
        {
            [Required]
            public int UserId { get; set; }
        }
        public class MergeDto///פה שיניתי!!!!!!!!!!!!!!
        {
            [Required]
            public int GiftId { get; set; }

            [Required, Range(1, 50)]
            public int Quantity { get; set; }
        }
    }
}