using ApiProject.Attributes;
using ApiProject.DTO;
using ApiProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllCategories();
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetCategoryById(id);
            if (category == null)
                return NotFound(new { message = $"קטגוריה מספר {id} לא נמצאה" });

            return Ok(category);
        }

        [AuthorizeRole(Roles.Manager)]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto category)
        {
            if (category == null || string.IsNullOrEmpty(category.Name))
                return BadRequest(new { message = "נתוני קטגוריה חסרים או לא תקינים" });

            var categories = await _service.AddCategory(category);
            return Ok(categories);
        }

        [AuthorizeRole(Roles.Manager)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto category)
        {
            var updatedCategory = await _service.UpdateCategory(category, id);
            if (updatedCategory == null)
                return NotFound(new { message = "עדכון נכשל: קטגוריה לא נמצאה" });

            return Ok(updatedCategory);
        }

        [AuthorizeRole(Roles.Manager)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var categories = await _service.DeleteCategory(id);
            if (categories == null)
                return NotFound(new { message = "מחיקה נכשלה: קטגוריה לא נמצאה" });

            return Ok(categories);
        }
    }
}
