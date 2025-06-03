using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoxTales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterUserDto registerUserDto)
    {
        try
        {
            await _userService.RegisterAsync(registerUserDto);
            return Ok("Registered");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
