using ApiProject.Models;
using System.Drawing;

namespace ApiProject.Repositories.Interface
{
    public interface IDonorRepository
    {
        Task<IEnumerable<DonorModel>> GetAllDonors();
        Task<DonorModel> GetDonorById(int id);
        Task<string> AddDonor(DonorModel donor);
        Task<DonorModel> UpdateDonor(DonorModel donor, int id);
        Task<string> DeleteDonor(DonorModel donor);
        Task<IEnumerable<DonorModel>> SearchByNameAsync(string searchTerm);

    }
}



