using FoxTales.Application.DTOs.Dylematy;
using FoxTales.Application.Interfaces;
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

        [HttpPost("add")]
        public async Task<IActionResult> AddCard([FromForm] CreateDylematyCardDto card)
        {
            try
            {
                await _dylematyService.AddCard(card);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
