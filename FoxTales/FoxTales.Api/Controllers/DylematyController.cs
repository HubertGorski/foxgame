using FoxTales.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoxTales.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DylematyController(IDylematyService dylematyService) : ControllerBase
    {
        private readonly IDylematyService _dylematyService = dylematyService;

        [HttpGet("get")]
        public async Task<IActionResult> GetAllCards()
        {
            try
            {
                var cards = await _dylematyService.GetAllCards();
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
