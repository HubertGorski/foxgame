using FoxTales.Api.Filters;
using FoxTales.Api.Helpers;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoxTales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    private const string RefreshToken = "refreshToken";
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        await _userService.RegisterAsync(registerUserDto);
        return Ok("Registered");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [RejectIfAuthenticated]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        LoginUserResponseDto response = await _userService.Login(loginUserDto);
        Response.Cookies.Append(RefreshToken, response.RefreshToken.Token, response.Options);
        return Ok(new { response.User, response.FoxGames, response.Avatars });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue(RefreshToken, out var refreshToken))
            return Unauthorized();

        await _userService.Logout(refreshToken);
        Response.Cookies.Delete(RefreshToken);
        return NoContent();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshSession()
    {
        if (!Request.Cookies.TryGetValue(RefreshToken, out var refreshToken))
            return Unauthorized();

        TokensResponseDto tokens = await _userService.GenerateNewTokens(refreshToken);
        Response.Cookies.Append(RefreshToken, tokens.RefreshToken.Token, tokens.Options);
        return Ok(tokens.AccessToken);
    }

    [HttpPost("clearTokens")]
    [AllowAnonymous]
    public async Task<IActionResult> ClearTokens()
    {
        await _userService.ClearTokens();
        return Ok();
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

    [HttpGet("getAvatars")]
    public async Task<IActionResult> GetAllAvatars()
    {
        var avatars = await _userService.GetAllAvatars();
        return Ok(avatars);
    }

    [HttpPost("setUsername")]
    public async Task<IActionResult> SetUsername([FromBody] SetUsernameRequestDto request)
    {
        int userId = User.GetUserId();
        bool response = await _userService.SetUsername(request.Username, userId);
        return Ok(response);
    }

    [HttpPost("setAvatar")]
    public async Task<IActionResult> SetAvatar([FromBody] SetAvatarRequestDto request)
    {
        int userId = User.GetUserId();
        bool response = await _userService.SetAvatar(request.AvatarId, userId);
        return Ok(response);
    }

    [HttpPost("addQuestion")]
    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequestDto request)
    {
        if (!User.IsOwner())
            return Unauthorized();

        int response = await _userService.AddQuestion(request.Question);
        return Ok(response);
    }
}
