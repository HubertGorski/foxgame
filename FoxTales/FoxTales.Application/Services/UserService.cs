using AutoMapper;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.DTOs.UserCard;
using FoxTales.Application.Exceptions;
using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FoxTales.Application.Services;

public class UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task RegisterAsync(RegisterUserDto registerUserDto)
    {
        if (await _userRepository.ExistsByUsernameAsync(registerUserDto.Username))
            throw new UserAlreadyExistsException("username", registerUserDto.Username);

        if (await _userRepository.ExistsByEmailAsync(registerUserDto.Email))
            throw new UserAlreadyExistsException("email", registerUserDto.Email);

        User user = _mapper.Map<User>(registerUserDto);
        user.PasswordHash = _passwordHasher.HashPassword(user, registerUserDto.Password);
        await _userRepository.AddAsync(user);
    }

    public async Task<ICollection<UserDto>> GetAllUsers()
    {
        ICollection<User> users = await _userRepository.GetAllUsers();
        return _mapper.Map<ICollection<UserDto>>(users);
    }

    public async Task<UserDto> GetUserById(int userId)
    {
        User? user = await _userRepository.GetUserById(userId) ?? throw new NotFoundException("User doesn't exist!");
        return _mapper.Map<UserDto>(user);
    }

    public async Task<ICollection<UserWithCardsDto>> GetAllUsersWithCards()
    {
        var users = await _userRepository.GetAllUsersWithCards();
        return _mapper.Map<ICollection<UserWithCardsDto>>(users);
    }
}
