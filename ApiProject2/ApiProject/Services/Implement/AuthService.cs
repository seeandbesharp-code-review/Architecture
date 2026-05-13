using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using ApiProject.Services.Interface;

namespace ApiProject.Services.Implement
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger; 

        public AuthService(IAuthRepository repository, IConfiguration config, ILogger<AuthService> logger)
        {
            _repository = repository;
            _config = config;
            _logger = logger;
        }

        public async Task<AuthorDto.UserModelDto> Register(AuthorDto.RegisterDto dto)
        {
            try
            {
                _logger.LogInformation("Attempting to register user with email: {Email}", dto.Email);

                var existing = await _repository.GetByEmail(dto.Email);
                if (existing != null)
                {
                    _logger.LogWarning("Registration failed: User with email {Email} already exists.", dto.Email);
                    return null;
                }

                var user = new UserModel
                {
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = RoleEnum.customer,
                    FirstName = dto.FirstName, 
                    LastName = dto.LastName,
                    Phone = dto.Phone
                };

                await _repository.AddUser(user);
                _logger.LogInformation("User {Email} registered successfully with ID {Id}", user.Email, user.Id);

                var token = GenerateJwtToken(user);


                return new AuthorDto.UserModelDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role.ToString(),
                    Token=token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration for email: {Email}", dto.Email);
                throw; 
            }
        }

        public async Task<AuthorDto.UserModelDto> Login(AuthorDto.LoginDto dto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

                var user = await _repository.GetByEmail(dto.Email);
                if(user.Phone != dto.Phone)
                {
                    _logger.LogWarning("Invalid login attempt for email: {Phone}", dto.Phone);
                    return null;

                }
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", dto.Email);
                    return null;
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("User {Email} logged in successfully.", dto.Email);

                return new AuthorDto.UserModelDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for email: {Email}", dto.Email);
                throw;
            }
        }

        private string GenerateJwtToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(int.Parse(_config["Jwt:ExpireHours"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}