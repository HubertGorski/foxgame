using AutoMapper;
using FoxTales.Application.DTOs.Catalog;
using FoxTales.Application.DTOs.FoxGame;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Exceptions;
using FoxTales.Application.Helpers;
using FoxTales.Application.Interfaces;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using FoxTales.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FoxTales.Application.Services;

public class UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher, IJwtTokenGenerator tokenGenerator, IUserLimitService userLimitService, IPsychLibraryService psychLibraryService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IUserLimitService _userLimitService = userLimitService;
    private readonly IPsychLibraryService _psychLibraryService = psychLibraryService;

    public async Task RegisterUser(RegisterUserDto registerUserDto)
    {
        User user = _mapper.Map<User>(registerUserDto);
        user.PasswordHash = _passwordHasher.HashPassword(user, registerUserDto.Password);
        user.RoleId = (int)RoleName.User;
        await Register(user);
    }

    public async Task<int> RegisterTmpUser(RegisterTmpUserDto registerTmpUserDto)
    {
        User user = _mapper.Map<User>(registerTmpUserDto);
        user.ExpirationDate = DateTime.UtcNow.AddHours(24); // TODO: zrobic joba czyszczacego i ustawiac 24 z configa.
        user.RoleId = (int)RoleName.TmpUser;
        await Register(user);
        return user.UserId;
    }

    private async Task Register(User user)
    {
        user.UserLimits = _userLimitService.CreateDefaultLimitsForUser(user.UserId);
        user.TermsAcceptedAt = DateTime.UtcNow;
        user.UserStatus = UserStatus.Active; // TODO: jest od razu aktywny, zmienić gdy będzie potwierdzanie konta.
        await _userRepository.AddAsync(user);
    }
    private async Task<TokensResponseDto> GetTokens(UserDto user)
    {
        TokensResponseDto tokens = _tokenGenerator.GetTokens(user);
        await _userRepository.StoreRefreshToken(tokens.RefreshToken);
        return tokens;
    }

    public async Task<LoginUserResponseDto> LoginUser(LoginUserDto loginUserDto)
    {
        User? user = await _userRepository.GetUser(loginUserDto.Email, null);
        if (user == null || user.PasswordHash == null)
            throw new UnauthorizedException(DictHelper.Validation.InvalidEmailOrPassword);

        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedException(DictHelper.Validation.InvalidEmailOrPassword);

        return await Login(user);
    }

    public async Task<LoginUserResponseDto> LoginTmpUser(int UserId)
    {
        User? user = await _userRepository.GetUser(null, UserId) ?? throw new InvalidOperationException($"User '{UserId}' does not exist!");
        return await Login(user);
    }

    private async Task<LoginUserResponseDto> Login(User user)
    {
        UserDto userDto = _mapper.Map<UserDto>(user);
        _userLimitService.ApplyClosestThresholds(userDto.UserLimits);

        TokensResponseDto tokens = await GetTokens(userDto);
        userDto.AccessToken = tokens.AccessToken;

        ICollection<Avatar> avatars = await _userRepository.GetAllAvatars();
        ICollection<AvatarDto> avatarsDto = _mapper.Map<ICollection<AvatarDto>>(avatars);

        ICollection<FoxGameDto> foxGamesDto = await _userLimitService.GetAllFoxGames();
        ICollection<CatalogTypeDto> availableCatalogTypesDto = await _psychLibraryService.GetCatalogTypesByPresetName(CatalogTypePresetName.DEFAULT_SIZES);
        ICollection<CatalogDto> PublicCatalogsDto = await _psychLibraryService.GetPublicCatalogsWithExampleQuestions();

        return new()
        {
            User = userDto,
            Avatars = avatarsDto,
            AvailableCatalogTypes = availableCatalogTypesDto,
            PublicCatalogs = PublicCatalogsDto,
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

        tokenEntity.IsRevoked = true;
        await _userRepository.SaveChangesAsync();

        UserDto userDto = _mapper.Map<UserDto>(tokenEntity.User);
        return await GetTokens(userDto);
    }

    public async Task Logout(string refreshToken)
    {
        RefreshToken tokenEntity = await _userRepository.GetRefreshTokenWithUser(refreshToken);

        if (tokenEntity.ExpiryDate <= DateTime.UtcNow || tokenEntity.IsRevoked)
            throw new UnauthorizedException("Invalid or expired refresh token");

        if (tokenEntity.User.Role.Name == RoleName.TmpUser)
            tokenEntity.User.UserStatus = UserStatus.Deleted;

        tokenEntity.IsRevoked = true;
        await _userRepository.SaveChangesAsync();
    }

    public async Task ClearTokens()
    {
        await _userRepository.ClearTokens();
    }

    public async Task CleanupInactiveTokens()
    {
        TimeSpan accessTokenTtl = TimeSpan.FromMinutes(15); // TODO: ustawic w configu
        List<RefreshToken> expiredTokens = await _userRepository.GetInactiveTokens(DateTime.UtcNow, accessTokenTtl);


        foreach (RefreshToken token in expiredTokens)
        {
            token.IsRevoked = true;
            if (token.User.Role.Name == RoleName.TmpUser)
                token.User.UserStatus = UserStatus.Deleted;
        }


        await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetUsername(string username, int userId)
    {
        return await _userRepository.SetUsername(username, userId);
    }

    public async Task<bool> SetAvatar(int avatarId, int userId)
    {
        return await _userRepository.SetAvatar(avatarId, userId);
    }

}