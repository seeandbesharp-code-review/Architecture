using ApiProject.DTO;
using ApiProject.Models;

namespace ApiProject.Repositories.Interface
{
    public interface IGiftRepository
    {
        Task<IEnumerable<GiftModel>> GetAllGifts();
        Task<GiftModel> GetGiftById(int id);
        Task<string> AddGift(GiftModel gift);
        Task<bool> CheckDonorExist(int id);
        Task<IEnumerable<GiftModel>> DeleteGift(int id);
        Task<GiftModel> UpdateGift(GiftModel gift, int id);
        Task<string> RuffleGift(GiftModel gift);
        Task<bool> CheckCategoryExist(int categoryId);
        Task<IEnumerable<CartItemModel>> CheckGiftPayed(int id);
        Task<IEnumerable<GiftDto.GiftBuyersData>> SortByBuyers();
        Task<bool> CheckNameExist(string name);

    }
}
