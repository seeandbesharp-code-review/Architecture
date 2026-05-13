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
    public class LotteryController : ControllerBase
    {
        private readonly ILotteryService _service;

        public LotteryController(ILotteryService service)
        {
            _service = service;
        }

        [HttpPost("{giftId}")]
        public async Task<ActionResult<RaffleDto.LotteryResult>> DrawRaffle(int giftId)
        {
            var result = await _service.RaffleGift(giftId);

            return Ok(result);
        }
    }
}
