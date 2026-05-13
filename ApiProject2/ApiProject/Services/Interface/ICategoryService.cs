using ApiProject.DTO;

namespace ApiProject.Services.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryModelDto>> AddCategory(CategoryDto category);
        Task<IEnumerable<CategoryModelDto>> DeleteCategory(int id);
        Task<IEnumerable<CategoryModelDto>> GetAllCategories();
        Task<CategoryModelDto> GetCategoryById(int id);
        Task<CategoryModelDto> UpdateCategory(CategoryDto category, int id);
    }
}