using FoxTales.Application.DTOs.User;
using FoxTales.Application.DTOs.UserCard;

namespace FoxTales.Application.Interfaces;

public interface IUserService
{
    Task RegisterAsync(RegisterUserDto registerUserDto);
    Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto);
    Task<ICollection<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int userId);
    Task<ICollection<UserWithCardsDto>> GetAllUsersWithCards();
    Task<TokensResponseDto> GenerateNewTokens(string refreshToken);
    Task Logout(string refreshToken);
    Task ClearTokens();
    Task<ICollection<AvatarDto>> GetAllAvatars();
    Task<bool> SetUsername(string username, int userId);
    Task<bool> SetAvatar(int avatarId, int userId);
}
