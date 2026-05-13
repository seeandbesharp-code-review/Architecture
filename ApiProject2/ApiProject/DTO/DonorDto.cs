using ApiProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ApiProject.DTO
{
    public class DonorDto
    {
        // DTO ליצירת תורם חדש
        public class AddDonorDto
        {
            [Required(ErrorMessage = "First name is required")]
            [MaxLength(50, ErrorMessage = "First name can be up to 50 characters")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required")]
            [MaxLength(50, ErrorMessage = "Last name can be up to 50 characters")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [MaxLength(100, ErrorMessage = "Email can be up to 100 characters")]
            public string EmailAddress { get; set; } = string.Empty;

            [Required(ErrorMessage = "Phone is required")]
            [Phone(ErrorMessage = "Invalid phone number")]
            [MaxLength(20, ErrorMessage = "Phone can be up to 20 characters")]
            public string Phone { get; set; } = string.Empty;
        }

        // DTO לעדכון תורם קיים
        public class UpdateDonorDto : AddDonorDto
        {
            [Required(ErrorMessage = "DonorId is required")]
            [Range(1, int.MaxValue, ErrorMessage = "DonorId must be greater than 0")]
            public int DonorId { get; set; }
        }

        // DTO להצגת תורם כולל מתנות
        public class DonorModelDto
        {
            public int Id { get; set; }

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string Phone { get; set; } = string.Empty;

            public List<GiftDto.GiftModelDto> Gifts { get; set; } = new List<GiftDto.GiftModelDto>();
        }
    }
}
