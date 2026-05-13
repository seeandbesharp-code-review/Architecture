using ApiProject.Data;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiProject.Repositories.Implement
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ProjectContext _context;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(ProjectContext context, ILogger<AuthRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserModel> GetByEmail(string email)
        {
            try
            {
                _logger.LogInformation("Attempting to get user by email {Email}", email);

                var user = await _context.users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogWarning("User not found with email {Email}", email);
                }
                else
                {
                    _logger.LogInformation("User found with email {Email}", email);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email {Email}", email);
                throw new Exception("שגיאה בשליפת משתמש לפי אימייל מהמסד נתונים", ex);
            }
        }

        public async Task<UserModel> AddUser(UserModel user)
        {
            try
            {
                _logger.LogInformation("Attempting to add new user with email {Email}", user.Email);

                await _context.users.AddAsync(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "User created successfully with id {UserId} and email {Email}",
                    user.Id,
                    user.Email
                );

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error adding new user with email {Email}",
                    user.Email
                );

                // שגיאה נפוצה כאן יכולה להיות אימייל כפול (אם מוגדר כ-Unique)
                throw new Exception("שגיאה בשמירת משתמש חדש במסד הנתונים", ex);
            }
        }
    }
}
