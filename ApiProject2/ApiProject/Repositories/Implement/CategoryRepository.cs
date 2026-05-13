using ApiProject.Data;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiProject.Repositories.Implement
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProjectContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(ProjectContext context, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryModel>> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Fetching all categories");
                return await _context.categories.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all categories");
                throw new Exception("שגיאה בשליפת רשימת הקטגוריות", ex);
            }
        }

        public async Task<CategoryModel> GetCategoryById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching category with id {CategoryId}", id);
                return await _context.categories.FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category with id {CategoryId}", id);
                throw new Exception($"שגיאה בשליפת קטגוריה מספר {id}", ex);
            }
        }

        // הוספה
        public async Task<IEnumerable<CategoryModel>> AddCategory(CategoryModel category)
        {
            try
            {
                _logger.LogInformation("Adding new category with name {CategoryName}", category.Name);

                await _context.categories.AddAsync(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Category added successfully with id {CategoryId}",
                    category.Id
                );

                return await GetAllCategories();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error adding new category with name {CategoryName}",
                    category.Name
                );
                throw new Exception("שגיאה בתהליך הוספת קטגוריה חדשה", ex);
            }
        }

        // עדכון
        public async Task<CategoryModel> UpdateCategoryR(CategoryModel category, int id)
        {
            try
            {
                _logger.LogInformation("Updating category with id {CategoryId}", id);

                var existingCategory = await _context.categories.FirstOrDefaultAsync(c => c.Id == id);

                if (existingCategory == null)
                {
                    _logger.LogWarning("Category with id {CategoryId} not found for update", id);
                    return null;
                }

                existingCategory.Name = category.Name;
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Category with id {CategoryId} updated successfully",
                    id
                );

                return existingCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with id {CategoryId}", id);
                throw new Exception($"שגיאה בעדכון קטגוריה מספר {id}", ex);
            }
        }

        // מחיקה
        public async Task<IEnumerable<CategoryModel>> DeleteCategory(int id)
        {
            try
            {
                _logger.LogInformation("Deleting category with id {CategoryId}", id);

                var category = await GetCategoryById(id);

                if (category == null)
                {
                    _logger.LogWarning("Category with id {CategoryId} not found for deletion", id);
                    return null;
                }

                _context.categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Category with id {CategoryId} deleted successfully",
                    id
                );

                return await GetAllCategories();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with id {CategoryId}", id);
                throw new Exception(
                    $"שגיאה במחיקת קטגוריה מספר {id}. וודא שהיא לא בשימוש על ידי מתנות.",
                    ex
                );
            }
        }
    }
}
