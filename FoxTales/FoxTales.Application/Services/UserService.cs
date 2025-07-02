using AutoMapper;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.DTOs.UserCard;
using FoxTales.Application.Exceptions;
using FoxTales.Application.Helpers;
using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using FoxTales.Domain.Extensions;
using FoxTales.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FoxTales.Application.Services;

public class UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher, IJwtTokenGenerator tokenGenerator, IUserLimitService userLimitService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IUserLimitService _userLimitService = userLimitService;

    public async Task RegisterAsync(RegisterUserDto registerUserDto)
    {
        User user = _mapper.Map<User>(registerUserDto);
        user.PasswordHash = _passwordHasher.HashPassword(user, registerUserDto.Password);
        user.UserLimits = _userLimitService.CreateDefaultLimitsForUser(user.UserId);
        user.RoleId = (int)RoleName.User;

        await _userRepository.AddAsync(user);
    }

    public async Task<ICollection<UserDto>> GetAllUsers()
    {
        ICollection<User> users = await _userRepository.GetAllUsers();
        ICollection<UserDto> usersDtos = _mapper.Map<ICollection<UserDto>>(users);
        foreach (var userDto in usersDtos)
        {
            foreach (var limitDto in userDto.UserLimits)
            {
                limitDto.ComputeClosestThreshold();
            }
        }

        return usersDtos;
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

    private async Task<TokensResponseDto> GetTokens(User user)
    {
        TokensResponseDto tokens = _tokenGenerator.GetTokens(user);
        await _userRepository.StoreRefreshToken(tokens.RefreshToken);
        return tokens;
    }

    public async Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto)
    {
        User? user = await _userRepository.GetUserByEmail(loginUserDto.Email) ?? throw new UnauthorizedException(DictHelper.Validation.InvalidEmailOrPassword);
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed) throw new UnauthorizedException(DictHelper.Validation.InvalidEmailOrPassword);
        TokensResponseDto tokens = await GetTokens(user);
        return new()
        {
            UserId = user.UserId,
            Username = user.Username,
            Options = tokens.Options,
            RefreshToken = tokens.RefreshToken,
            AccessToken = tokens.AccessToken
        };
    }

    public async Task<TokensResponseDto> GenerateNewTokens(string refreshToken)
    {
        RefreshToken tokenEntity = await _userRepository.GetRefreshTokenWithUser(refreshToken);

        if (tokenEntity.ExpiryDate <= DateTime.UtcNow || tokenEntity.IsRevoked)
            throw new UnauthorizedException("Invalid or expired refresh token");

        await _userRepository.RevokeRefreshToken(tokenEntity);
        return await GetTokens(tokenEntity.User);
    }

    public async Task Logout(string refreshToken)
    {
        RefreshToken tokenEntity = await _userRepository.GetRefreshTokenWithUser(refreshToken);

        if (tokenEntity.ExpiryDate <= DateTime.UtcNow || tokenEntity.IsRevoked)
            throw new UnauthorizedException("Invalid or expired refresh token");

        await _userRepository.RevokeRefreshToken(tokenEntity);
    }

    public async Task ClearTokens()
    {
        await _userRepository.ClearTokens();
    }
}