using System.ComponentModel.DataAnnotations;

namespace ApiProject.Models
{
    public class PurchaseModel//כבר הוזמן
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserModel  User { get; set; }//אם ישבעיה
        public int GiftModelId { get; set; }
        public GiftModel GiftModel { get; set; }
        [Range(1,50)]
        public int Quantity { get; set; }
        [Required]
        public int TotalPrice { get; set; }
    }
}