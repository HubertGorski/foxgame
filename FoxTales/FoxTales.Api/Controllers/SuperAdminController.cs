using FoxTales.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoxTales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] //TODO: dodac admina
public class SuperAdminController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("clearTokens")]
    public async Task<IActionResult> ClearTokens()
    {
        await _userService.ClearTokens();
        return Ok();
    }
}

