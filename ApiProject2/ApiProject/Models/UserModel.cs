using System.ComponentModel.DataAnnotations;

namespace ApiProject.Models
{       
    public enum RoleEnum
        {
            customer,
            manager
        }

    public class UserModel
    {
        public int Id { get; set; }
        [Required,MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [EmailAddress,MaxLength(50)]
        public string Email { get; set; }
        [Required,Phone]
        public string Phone {  get; set; }
        [Required,MinLength(6)]
        public string Password { get; set; }
        public RoleEnum Role { get; set; } = RoleEnum.customer;

    }
}




