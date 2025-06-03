using AutoMapper;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Services;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task RegisterAsync(RegisterUserDto registerUserDto)
    {
        if (await _userRepository.ExistsByUsernameAsync(registerUserDto.Username))
            throw new Exception("Username already exists.");

        User user = _mapper.Map<User>(registerUserDto);
        await _userRepository.AddAsync(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        IEnumerable<User> users = await _userRepository.GetAllUsers();
        return _mapper.Map<List<UserDto>>(users);
    }
}
