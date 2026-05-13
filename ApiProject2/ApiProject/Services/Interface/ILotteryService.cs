using ApiProject.DTO;

namespace ApiProject.Services.Interface
{
    public interface ILotteryService
    {
        Task<RaffleDto.LotteryResult> RaffleGift(int giftId);

    }
}
