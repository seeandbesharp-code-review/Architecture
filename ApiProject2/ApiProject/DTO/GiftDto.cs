using ApiProject.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;

namespace ApiProject.DTO
{
    public class GiftDto
    {
        public class AddGiftDto
        {
            [Required(ErrorMessage = "Name is required")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Description is required")]
            [StringLength(500, ErrorMessage = "Description can be up to 500 characters")]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "Category is required")]
            [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
            public int CategoryId { get; set; }

            [Required(ErrorMessage = "Ticket price is required")]
            [Range(1, 100000, ErrorMessage = "Ticket price must be greater than 0")]
            public int TicketPrice { get; set; }

            [Required(ErrorMessage = "Image is required")]
            public string Image { get; set; } = string.Empty;

            [Required(ErrorMessage = "Donor is required")]
            [Range(1, int.MaxValue, ErrorMessage = "DonorId must be greater than 0")]
            public int DonorId { get; set; }
        }

        public class UpdateGiftDto : AddGiftDto
        {
            [Required(ErrorMessage = "Gift Id is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
            public int Id { get; set; }
        }

        public class GiftModelDto
        {
            public int Id { get; set; }
           [Required]
            public string Name { get; set; } = string.Empty;

            [Required]
            public string Category { get; set; } = string.Empty;

            [Range(1, int.MaxValue)]
            public int TicketPrice { get; set; }

            public string Image { get; set; } = string.Empty;

            public string Donor { get; set; } = string.Empty;
            public string WinnerName { get; set; }
            public string Description { get; set; }
        }
        public class GiftBuyersData
        {
            public GiftModel Gift { get; set; }
            public int TotalTickets { get; set; }
            public int PurchaseCount { get; set; }
        }
        public class GiftToManager:GiftModelDto
        {
            public int TotalTickets { get; set; }
            public int PurchaseCount { get; set; }
            public int TotalSum { get; set; }
        }

        public class GiftRaffled
        {
            [Required]
            [Range(1, int.MaxValue)]
            public int Id { get; set; }

            public int? WinnerUserId { get; set; }
        }
    }
}
