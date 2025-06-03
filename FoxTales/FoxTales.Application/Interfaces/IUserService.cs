using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Interfaces;

public interface IUserService
{
    Task RegisterAsync(RegisterUserDto registerUserDto);
    Task<IEnumerable<UserDto>> GetAllUsers();
}
