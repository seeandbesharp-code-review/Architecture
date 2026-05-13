using ApiProject.DTO;
using ApiProject.Models;
using System.Drawing;

namespace ApiProject.Services.Interface
{
    public interface IDonorService
    {
        Task<IEnumerable<DonorDto.DonorModelDto>> GetAllDonors();
        Task<DonorDto.DonorModelDto> GetDonorById(int id);
        Task<DonorDto.DonorModelDto> AddDonor(DonorDto.AddDonorDto dto);
        Task<DonorDto.DonorModelDto> UpdateDonor(DonorDto.UpdateDonorDto donor , int id);
        Task<string> DeleteDonor(int id);
        Task<IEnumerable<DonorDto.DonorModelDto>> SearchDonorByNameAsync(string searchTerm);
    }
}
