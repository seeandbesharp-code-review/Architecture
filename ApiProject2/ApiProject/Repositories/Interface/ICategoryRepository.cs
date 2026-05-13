using ApiProject.Models;

namespace ApiProject.Repositories.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryModel>> DeleteCategory(int id);
        Task<IEnumerable<CategoryModel>> GetAllCategories();
        Task<CategoryModel> GetCategoryById(int id);
        Task<IEnumerable<CategoryModel>> AddCategory(CategoryModel category);
        Task<CategoryModel> UpdateCategoryR(CategoryModel category, int id);
    }
}