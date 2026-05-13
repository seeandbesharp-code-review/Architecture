using ApiProject.Attributes;
using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Services.Implement;
using ApiProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(Roles.Manager)]
    public class DonorsController : ControllerBase
    {
        private readonly IDonorService _service;

        public DonorsController(IDonorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllDonors());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var donor = await _service.GetDonorById(id);
            if (donor == null)
                return NotFound(new { message = $"תורם עם מזהה {id} לא נמצא" });

            return Ok(donor);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DonorDto.AddDonorDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "נתוני תורם חסרים" });

            return Ok(await _service.AddDonor(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] DonorDto.UpdateDonorDto donor, int id)
        {
            var result = await _service.UpdateDonor(donor, id);
            if (result == null)
                return NotFound(new { message = "עדכון נכשל: התורם לא נמצא" });

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteDonor(id);
            if (result == null)
                return NotFound(new { message = "מחיקה נכשלה: התורם לא נמצא" });

            return Ok(result);
        }

        [HttpGet("searchDonorByFullName")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Search term cannot be empty." });

            var donor = await _service.SearchDonorByNameAsync(name);
            return Ok(donor);
        }
    }
}
