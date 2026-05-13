using ApiProject.DTO;
using ApiProject.Models;

namespace ApiProject.Services.Interface
{
    public interface IGiftsService
    {
        Task<IEnumerable<GiftDto.GiftModelDto>> getAllGifts();
        Task<GiftDto.GiftModelDto> GetGiftById(int id);
        Task<string> AddGift(GiftDto.AddGiftDto gift);
        Task<IEnumerable<GiftDto.GiftModelDto>> DeleteGift(int id);
        Task<GiftDto.GiftModelDto> UpdateGift(GiftDto.UpdateGiftDto gift , int id);
        Task<string> RuffleGift(GiftDto.GiftRaffled gift);
        Task<IEnumerable<GiftDto.GiftModelDto>> SortByName(string name);
        Task<IEnumerable<GiftDto.GiftModelDto>> SortByDonorName(string name);
        Task<IEnumerable<GiftDto.GiftToManager>> SortByBuyers();
        Task<IEnumerable<GiftDto.GiftModelDto>> OrderByPrice();
        Task<IEnumerable<GiftDto.GiftModelDto>> SortByCategory(int id);

    }
}