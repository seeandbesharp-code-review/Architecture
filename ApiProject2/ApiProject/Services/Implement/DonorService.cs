using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Implement;
using ApiProject.Repositories.Interface;
using ApiProject.Services.Interface;
using Microsoft.Extensions.Logging;

namespace ApiProject.Services.Implement
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _repository;
        private readonly ILogger<DonorService> _logger;

        public DonorService(IDonorRepository repository, ILogger<DonorService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<DonorDto.DonorModelDto> AddDonor(DonorDto.AddDonorDto dto)
        {
            try
            {
                _logger.LogInformation("Attempting to add a new donor: {FirstName} {LastName}", dto.FirstName, dto.LastName);

                var donor = new DonorModel
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.EmailAddress,
                    Phone = dto.Phone,
                    MyGifts = new List<GiftModel>()
                };

                await _repository.AddDonor(donor);
                return await GetDonorById(donor.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding donor {Email}", dto.EmailAddress);
                throw;
            }
        }

        public async Task<string> DeleteDonor(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete donor with ID: {Id}", id);
                var donor = await _repository.GetDonorById(id);
                if (donor == null)
                {
                    _logger.LogWarning("Delete failed: Donor with ID {Id} not found", id);
                    return null;
                }
                return await _repository.DeleteDonor(donor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting donor {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<DonorDto.DonorModelDto>> GetAllDonors()
        {
            try
            {
                var donors = await _repository.GetAllDonors();
                return donors.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all donors");
                throw;
            }
        }

        public async Task<DonorDto.DonorModelDto> GetDonorById(int id)
        {
            try
            {
                var donor = await _repository.GetDonorById(id);
                if (donor == null) return null;

                return MapToDto(donor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching donor {Id}", id);
                throw;
            }
        }

        public async Task<DonorDto.DonorModelDto> UpdateDonor(DonorDto.UpdateDonorDto dto, int id)
        {
            try
            {
                _logger.LogInformation("Updating donor with ID: {Id}", id);
                var donor = await _repository.GetDonorById(id);
                if (donor == null) return null;

                donor.FirstName = dto.FirstName;
                donor.LastName = dto.LastName;
                donor.Email = dto.EmailAddress;
                donor.Phone = dto.Phone;

                await _repository.UpdateDonor(donor, id);
                return await GetDonorById(donor.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating donor {Id}", id);
                throw;
            }
        }

        private static DonorDto.DonorModelDto MapToDto(DonorModel d)
        {
            return new DonorDto.DonorModelDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                Phone = d.Phone,
                Gifts = d.MyGifts?.Select(g => new GiftDto.GiftModelDto
                {
                    Name = g.Name,
                    Category = g.CategoryModel?.Name, 
                    TicketPrice = g.TicketPrice,
                    Image = g.Image
               
                }).ToList() ?? new List<GiftDto.GiftModelDto>()
            };
        }



        public async Task<IEnumerable<DonorDto.DonorModelDto>> SearchDonorByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<DonorDto.DonorModelDto>();
            }

            var donors = await _repository.SearchByNameAsync(searchTerm);
            return donors.Select(MapToDto);
        }

    }
}