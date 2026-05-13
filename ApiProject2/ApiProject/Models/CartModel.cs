using System.ComponentModel.DataAnnotations;

namespace ApiProject.Models
{
    public enum Status
    {
        Draft,
        Purchase
    }
    public class CartModel
    {
        public int Id { get; set; }
        public int  UserModelId { get; set; }
        public UserModel User { get; set; }//אם יש בעיה...

        [Required]
        public Status MyStatus { get; set; }
        public  List<CartItemModel> CartItem { get; set; }//רשימת הפריטים בסל
        // public DateTime CreatedAt { get; set; }
        //public int Quantity { get; set; }

    }
}