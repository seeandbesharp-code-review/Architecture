using System.ComponentModel.DataAnnotations;

namespace ApiProject.Models
{
    public class DonorModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        [Required]
        public string Phone { get; set; }
        public  List<GiftModel> MyGifts { get; set; }
    }
}