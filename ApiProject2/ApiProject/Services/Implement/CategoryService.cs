using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using ApiProject.Services.Interface;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Extensions.Logging;

namespace ApiProject.Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // גט הכל
        public async Task<IEnumerable<CategoryModelDto>> GetAllCategories()
        {
            try
            {
                var categories = await _repository.GetAllCategories();
                return categories.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all categories");
                throw;
            }
        }

        // גט לפי מזהה
        public async Task<CategoryModelDto> GetCategoryById(int id)
        {
            try
            {
                var category = await _repository.GetCategoryById(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {Id} was not found", id);
                    return null;
                }
                return MapToDto(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching category {Id}", id);
                throw;
            }
        }

        // הוספת קטגוריה
        public async Task<IEnumerable<CategoryModelDto>> AddCategory(CategoryDto category)
        {
            try
            {
                _logger.LogInformation("Adding new category: {CategoryName}", category.Name);
                var categoryToAdd = new CategoryModel { Name = category.Name };
                var categories = await _repository.AddCategory(categoryToAdd);
                return categories.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding category {CategoryName}", category.Name);
                throw;
            }
        }

        // עדכון קטגוריה
        public async Task<CategoryModelDto> UpdateCategory(CategoryDto category, int id)
        {
            try
            {
                _logger.LogInformation("Updating category ID {Id} to Name: {CategoryName}", id, category.Name);
                var categoryToUpdate = new CategoryModel { Name = category.Name };
                var updatedCategory = await _repository.UpdateCategoryR(categoryToUpdate, id);

                if (updatedCategory == null)
                {
                    _logger.LogWarning("Failed to update: Category ID {Id} not found", id);
                    return null;
                }

                return MapToDto(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category {Id}", id);
                throw;
            }
        }

        // מחיקת קטגוריה
        public async Task<IEnumerable<CategoryModelDto>> DeleteCategory(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete category ID {Id}", id);
                var categories = await _repository.DeleteCategory(id);

                if (categories == null)
                {
                    _logger.LogWarning("Delete failed: Category ID {Id} not found", id);
                    return null;
                }

                return categories.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category {Id}", id);
                throw;
            }
        }

        // פונקציית עזר למיפוי
        private static CategoryModelDto MapToDto(CategoryModel category)
        {
            return new CategoryModelDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}