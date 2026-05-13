namespace ApiProject.DTO
{
    public class SalesDto
    {
        public class GiftResume
        {
            public int GiftId { get; set; }
            public string GiftName { get; set; }
            public int GiftPrice { get; set; }
            public int TotalTicketsPurchase { get; set; }//כמה כרטיסים נקנו עבור המתנה
            public int TotalIncome { get; set; }//בסהכ כמה הרוויחו מהמתנה
        }
        public class GiftsWithBuyers
        {
            public int GiftId { get; set; }
            public string GiftName { get; set; }
            public int GiftPrice { get; set; }
            public List<BuyerInfo> Buyers { get; set; } = new List<BuyerInfo>();

        }
        public class BuyerInfo
        {
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public int TicketsPurshased { get; set; }//כמה כרטיסים רכש

        }
    }


}
