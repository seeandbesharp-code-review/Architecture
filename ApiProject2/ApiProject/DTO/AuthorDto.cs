using System.ComponentModel.DataAnnotations;

namespace ApiProject.DTO
{
    public class AuthorDto
    {
        public class LoginDto
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [MaxLength(50)]
            public string Email { get; set; } = string.Empty;
            [Required(ErrorMessage = "Phone is required")]
            [Phone(ErrorMessage = "Invalid phone format")]
            [MaxLength(50)]
            public string Phone { get; set; } = string.Empty;


            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
            [MaxLength(50)]
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterDto
        {
            [Required(ErrorMessage = "First name is required")]
            [MaxLength(50, ErrorMessage = "First name can be up to 50 characters")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required")]
            [MaxLength(50, ErrorMessage = "Last name can be up to 50 characters")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [MaxLength(100)]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Phone is required")]
            [Phone(ErrorMessage = "Invalid phone number")]
            [MaxLength(20)]
            public string Phone { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
            [MaxLength(100)]
            public string Password { get; set; } = string.Empty;
        }

        public class UserModelDto
        {
            public int Id { get; set; }

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string Phone { get; set; } = string.Empty;

            public string Role { get; set; } = string.Empty;

            // JWT Token
            public string Token { get; set; } = string.Empty;
        }
    }
}
