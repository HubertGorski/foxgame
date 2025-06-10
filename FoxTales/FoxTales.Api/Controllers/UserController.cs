using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoxTales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService, IJwtTokenGenerator tokenGenerator) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        await _userService.RegisterAsync(registerUserDto);
        return Ok("Registered");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        var claims = await _userService.GenerateClaims(loginUserDto);
        string token = _tokenGenerator.GenerateToken(claims);
        return Ok(token);
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("get/{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var users = await _userService.GetUserById(userId);
        return Ok(users);
    }

    [HttpGet("getWithCards")]
    public async Task<IActionResult> GetAllUsersWithCards()
    {
        var users = await _userService.GetAllUsersWithCards();
        return Ok(users);
    }
}
