using ApiProject.DTO;

namespace ApiProject.Repositories.Interface
{
    public interface ISalesRepository
    {
        Task<List<SalesDto.GiftResume>> GetGiftResumes();
        Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByPriceDesc();
        Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByQuantityDesc();
        Task<List<SalesDto.GiftsWithBuyers>> GetGiftWithBuyers();
    }
}