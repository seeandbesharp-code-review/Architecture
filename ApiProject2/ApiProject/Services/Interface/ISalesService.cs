using ApiProject.DTO;

namespace ApiProject.Services.Interface
{
    public interface ISalesService
    {
        Task<List<SalesDto.GiftResume>> GetGiftResumes();
        Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByPriceDesc();
        Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByQuantityDesc();
        Task<List<SalesDto.GiftsWithBuyers>> GetGiftWithBuyers();
    }
}