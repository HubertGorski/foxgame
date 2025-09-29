using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Interfaces;

public interface IUserService
{
    Task RegisterAsync(RegisterUserDto registerUserDto);
    Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto);
    Task<TokensResponseDto> GenerateNewTokens(string refreshToken);
    Task Logout(string refreshToken);
    Task ClearTokens();
    Task<bool> SetUsername(string username, int userId);
    Task<bool> SetAvatar(int avatarId, int userId);
}
