using System.Security.Claims;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.DTOs.UserCard;

namespace FoxTales.Application.Interfaces;

public interface IUserService
{
    Task RegisterAsync(RegisterUserDto registerUserDto);
    Task<ICollection<Claim>> GenerateClaims(LoginUserDto loginUserDto);
    Task<ICollection<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int userId);
    Task<ICollection<UserWithCardsDto>> GetAllUsersWithCards();
}
