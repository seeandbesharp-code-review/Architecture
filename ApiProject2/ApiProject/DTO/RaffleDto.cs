namespace ApiProject.DTO
{
    public class RaffleDto
    {
        
            public class WinnerDto
            {
                public int? WinnerId { get; set; }
                public string WinnerName { get; set; }
                public string WinnerEmail { get; set; }
            }

            public class RaffleEntry
            {
                public int UserId { get; set; }
                public int Quantity { get; set; }
            }
            public class LotteryResult
            {
                public int GiftId { get; set; }
                public string GiftName { get; set; }
                public int WinnerUserId { get; set; }
                public string WinnerName { get; set; }
                public string WinnerEmail { get; set; }
            public string DonorName { get; set; }
            public int TotalTicketsSold { get; set; }
                public int TotalParticipants { get; set; }
                public decimal TotalIncome { get; set; }
            }
        

    }
}

