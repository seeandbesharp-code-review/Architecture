using ApiProject.Data;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiProject.Repositories.Implement
{
    public class DonorRepository : IDonorRepository
    {
        private readonly ProjectContext _context;
        private readonly ILogger<DonorRepository> _logger;

        public DonorRepository(ProjectContext context, ILogger<DonorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> AddDonor(DonorModel donor)
        {
            try
            {
                _logger.LogInformation("Adding new donor");

                await _context.donors.AddAsync(donor);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Donor added successfully with id {DonorId}",
                    donor.Id
                );

                return "added completed!!!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new donor");
                throw new Exception("שגיאה בהוספת תורם חדש למערכת", ex);
            }
        }

        public async Task<string> DeleteDonor(DonorModel donor)
        {
            try
            {
                _logger.LogInformation(
                    "Deleting donor with id {DonorId}",
                    donor.Id
                );

                _context.donors.Remove(donor);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Donor with id {DonorId} deleted successfully",
                    donor.Id
                );

                return "deleted!!";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting donor with id {DonorId}",
                    donor.Id
                );

                // שגיאה נפוצה כאן: תורם שיש לו מתנות במערכת (בעיית Foreign Key)
                throw new Exception(
                    "שגיאה במחיקת תורם. וודא שאין מתנות המשויכות לתורם זה.",
                    ex
                );
            }
        }

        public async Task<IEnumerable<DonorModel>> GetAllDonors()
        {
            try
            {
                _logger.LogInformation("Fetching all donors");

                return await _context.donors
                    .Include(a => a.MyGifts)
                    .ThenInclude(a => a.CategoryModel)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all donors");
                throw new Exception("שגיאה בשליפת רשימת התורמים", ex);
            }
        }

        public async Task<DonorModel> GetDonorById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching donor with id {DonorId}", id);

                return await _context.donors
                    .Include(a => a.MyGifts)
                    .ThenInclude(a => a.CategoryModel)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching donor with id {DonorId}", id);
                throw new Exception($"שגיאה בשליפת תורם מספר {id}", ex);
            }
        }

        public async Task<DonorModel> UpdateDonor(DonorModel donor, int id)
        {
            try
            {
                _logger.LogInformation(
                    "Updating donor with id {DonorId}",
                    id
                );

                _context.donors.Update(donor);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Donor with id {DonorId} updated successfully",
                    id
                );

                return donor;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating donor with id {DonorId}",
                    id
                );
                throw new Exception($"שגיאה בעדכון פרטי תורם מספר {id}", ex);
            }
        }


        public async Task<IEnumerable<DonorModel>> SearchByNameAsync(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            return await _context.donors.Include(x=>x.MyGifts)
                .Where(x=>(x.FirstName + " " +  x.LastName).ToLower().Contains(searchTerm)
                || x.Email.ToLower().Contains(searchTerm)
                || x.MyGifts.Any(g=>g.Name.ToLower().Contains(searchTerm))
                ).ToListAsync();
        }

    }
}
