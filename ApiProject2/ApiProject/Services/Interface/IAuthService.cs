using ApiProject.DTO;

namespace ApiProject.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthorDto.UserModelDto> Register(AuthorDto.RegisterDto dto);
        Task<AuthorDto.UserModelDto> Login(AuthorDto.LoginDto dto);

    }
}
