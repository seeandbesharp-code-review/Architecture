using System.ComponentModel.DataAnnotations;

namespace ApiProject.Models
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public  int CartId { get; set; }
        public CartModel Cart { get; set; }//אם יש בעיה...
        public int GiftModelId { get; set; }
        public GiftModel Gift { get; set; }

        [Range(1, 50)]
        public int Quantity { get; set; }
    }
}
//טבלה מקשרת בין רבים לרבים
//לכל סל הרבה מתנות 
//ולכל מתנה הרבה סלים
//עמודה של סל ועמודה של מתנה
//אבל יכולה גם להכיל שדות נוספים