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
        await _userService.RegisterAsync(registerUserDto);
        return Ok("Registered");
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var users = await _userService.GetUserById(id);
        return Ok(users);
    }

    [HttpGet("getWithCards")]
    public async Task<IActionResult> GetAllUsersWithCards()
    {
        var users = await _userService.GetAllUsersWithCards();
        return Ok(users);
    }
}
