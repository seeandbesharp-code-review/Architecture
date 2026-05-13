using ApiProject.DTO;
using ApiProject.Repositories.Interface;
using ApiProject.Services.Interface;

namespace ApiProject.Services.Implement
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _repository;
        public SalesService(ISalesRepository repository)
        {
            _repository = repository;
        }

        //צפייה ברכישות כרטיסים עבור כל מתנה.
        public async Task<List<SalesDto.GiftResume>> GetGiftResumes()
        {
            return await _repository.GetGiftResumes();
        }

        //  כל מתנה + רשימת רוכשים
        public async Task<List<SalesDto.GiftsWithBuyers>> GetGiftWithBuyers()
        {
            return await _repository.GetGiftWithBuyers();
            }

        // מיון לפי כמות כרטיסים מהגבוה לנמוך
        public async Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByQuantityDesc()
        {
            return await _repository.GetGiftsByQuantityDesc();
        }
        // מיון לפי מחיר מהגבוה לנמוך

        public async Task<List<SalesDto.GiftsWithBuyers>> GetGiftsByPriceDesc()
        {
            return await _repository.GetGiftsByPriceDesc();
        }
    }

}

