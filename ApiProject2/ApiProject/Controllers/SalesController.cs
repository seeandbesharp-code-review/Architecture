using ApiProject.Attributes;
using ApiProject.DTO;
using ApiProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRole(Roles.Manager)]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _context;

        public SalesController(ISalesService context)
        {
            _context = context;
        }

        [HttpGet("gift-resumes")]
        public async Task<ActionResult<List<SalesDto.GiftResume>>> GetGiftResumes()
        {
            var giftResumes = await _context.GetGiftResumes();
            return Ok(giftResumes);
        }

        [HttpGet("gift-buyers")]
        public async Task<ActionResult<List<SalesDto.GiftsWithBuyers>>> GetGiftWithBuyers()
        {
            var result = await _context.GetGiftWithBuyers();
            return Ok(result);
        }

        [HttpGet("gifts-by-price")]
        public async Task<ActionResult<List<SalesDto.GiftsWithBuyers>>> GetGiftsByPriceDesc()
        {
            var result = await _context.GetGiftsByPriceDesc();
            return Ok(result);
        }

        [HttpGet("gifts-by-quantity")]
        public async Task<ActionResult<List<SalesDto.GiftsWithBuyers>>> GetGiftsByQuantityDesc()
        {
            var result = await _context.GetGiftsByQuantityDesc();
            return Ok(result);
        }
    }
}
