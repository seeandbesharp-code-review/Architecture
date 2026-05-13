using ApiProject.Models;

namespace ApiProject.Repositories.Interface
{
    public interface IAuthRepository
    {
        Task<UserModel> GetByEmail(string email);
        Task<UserModel> AddUser(UserModel user);

    }
}
