using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Interfaces;

public interface IUserService
{
    Task RegisterUser(RegisterUserDto registerUserDto);
    Task<int> RegisterTmpUser(RegisterTmpUserDto registerTmpUserDto);
    Task<LoginUserResponseDto> LoginUser(LoginUserDto loginUserDto);
    Task<LoginUserResponseDto> LoginTmpUser(int UserId);
    Task<TokensResponseDto> GenerateNewTokens(string refreshToken);
    Task Logout(string refreshToken);
    Task ClearTokens();
    Task<bool> SetUsername(string username, int userId);
    Task<bool> SetAvatar(int avatarId, int userId);
}
