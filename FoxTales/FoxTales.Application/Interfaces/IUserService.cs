using FoxTales.Application.DTOs.User;
using FoxTales.Application.DTOs.UserCard;

namespace FoxTales.Application.Interfaces;

public interface IUserService
{
    Task RegisterAsync(RegisterUserDto registerUserDto);
    Task<ICollection<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int userId);
    Task<ICollection<UserWithCardsDto>> GetAllUsersWithCards();
}
