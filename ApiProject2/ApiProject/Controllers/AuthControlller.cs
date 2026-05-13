using ApiProject.DTO;
using ApiProject.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthorDto.RegisterDto dto)
        {
            var user = await _authService.Register(dto);

            if (user == null)
                return BadRequest(new { message = "המשתמש כבר קיים במערכת" });

            SetJwtCookie(user.Token);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthorDto.LoginDto dto)
        {
            var user = await _authService.Login(dto);

            if (user == null)
                return Unauthorized(new { message = "אחד מהנתונים שהוקשו שגוי" });

            SetJwtCookie(user.Token);
            return Ok(user);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return Ok(new { message = "התנתקת בהצלחה" });
        }

        private void SetJwtCookie(string token)
        {
            var expireHours = int.Parse(_config["Jwt:ExpireHours"] ?? "2");
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = false,
                Expires = DateTimeOffset.UtcNow.AddHours(expireHours)
            });
        }
    }
}
