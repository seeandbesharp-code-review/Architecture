using ApiProject.DTO;
using ApiProject.Models;

namespace ApiProject.Repositories.Interface
{
    public interface ILotteryRepository
    {
        Task<IEnumerable<RaffleDto.RaffleEntry>> GetRaffleEntries(int giftId);
        Task<string> SetWinner(int giftId, int winnerUserId);

        Task<UserModel> GetWinnerDetails(int userId);

    }
}
