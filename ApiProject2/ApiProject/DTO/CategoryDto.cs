using System.ComponentModel.DataAnnotations;

namespace ApiProject.DTO
{
    public class CategoryDto
    {
        [Required]
        public string Name { get; set; }
    }
    public class CategoryModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
