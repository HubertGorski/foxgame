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
    private readonly IUserService _userService = userService;
    private const string RefreshToken = "refreshToken";

    [HttpPost("registerUser")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
    {
        await _userService.RegisterUser(registerUserDto);
        return Ok("Registered");
    }

    [HttpPost("registerTmpUser")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterTmpUser([FromBody] RegisterTmpUserDto registerTmpUserDto)
    {
        int userId = await _userService.RegisterTmpUser(registerTmpUserDto);
        LoginUserResponseDto response = await _userService.LoginTmpUser(userId);
        Response.Cookies.Append(RefreshToken, response.RefreshToken.Token, response.Options);
        return Ok(new { response.User, response.FoxGames, response.Avatars, response.AvailableCatalogTypes, response.PublicQuestions });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [RejectIfAuthenticated]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        LoginUserResponseDto response = await _userService.LoginUser(loginUserDto);
        Response.Cookies.Append(RefreshToken, response.RefreshToken.Token, response.Options);
        return Ok(new { response.User, response.FoxGames, response.Avatars, response.AvailableCatalogTypes, response.PublicQuestions });
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
}
