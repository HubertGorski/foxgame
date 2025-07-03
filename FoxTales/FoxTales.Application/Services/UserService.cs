using AutoMapper;
using FoxTales.Application.DTOs.FoxGame;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.DTOs.UserCard;
using FoxTales.Application.Exceptions;
using FoxTales.Application.Helpers;
using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
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
            _userLimitService.ApplyClosestThresholds(userDto.UserLimits);
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

    private async Task<TokensResponseDto> GetTokens(UserDto user)
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

        UserDto userDto = _mapper.Map<UserDto>(user);
        _userLimitService.ApplyClosestThresholds(userDto.UserLimits);

        TokensResponseDto tokens = await GetTokens(userDto);
        userDto.AccessToken = tokens.AccessToken;

        ICollection<FoxGameDto> foxGamesDto = await _userLimitService.GetAllFoxGames();

        ICollection<Avatar> avatars = await _userRepository.GetAllAvatars();
        ICollection<AvatarDto> avatarsDto = _mapper.Map<ICollection<AvatarDto>>(avatars);

        return new()
        {
            User = userDto,
            Avatars = avatarsDto,
            FoxGames = foxGamesDto,
            Options = tokens.Options,
            RefreshToken = tokens.RefreshToken
        };
    }

    public async Task<TokensResponseDto> GenerateNewTokens(string refreshToken)
    {
        RefreshToken tokenEntity = await _userRepository.GetRefreshTokenWithUser(refreshToken);

        if (tokenEntity.ExpiryDate <= DateTime.UtcNow || tokenEntity.IsRevoked)
            throw new UnauthorizedException("Invalid or expired refresh token");

        await _userRepository.RevokeRefreshToken(tokenEntity);

        UserDto userDto = _mapper.Map<UserDto>(tokenEntity.User);
        return await GetTokens(userDto);
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

    public async Task<ICollection<AvatarDto>> GetAllAvatars()
    {
        ICollection<Avatar> avatars = await _userRepository.GetAllAvatars();
        return _mapper.Map<ICollection<AvatarDto>>(avatars);
    }

    public async Task<bool> SetUsername(string username, int userId)
    {
        return await _userRepository.SetUsername(username, userId);
    }

    public async Task<bool> SetAvatar(int avatarId, int userId)
    {
        return await _userRepository.SetAvatar(avatarId, userId);
    }

    public async Task<int> AddQuestion(QuestionDto request)
    {
        Question question = _mapper.Map<Question>(request);
        return await _userRepository.AddQuestion(question);
    }

    public async Task<bool> RemoveQuestion(int questionId)
    {
        return await _userRepository.RemoveQuestion(questionId);
    }
}