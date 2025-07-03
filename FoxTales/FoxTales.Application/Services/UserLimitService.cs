using AutoMapper;
using FoxTales.Application.DTOs.FoxGame;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Services;

public class UserLimitService(IFoxGameRepository foxGameRepository, IMapper mapper) : IUserLimitService
{
    private readonly IFoxGameRepository _foxGameRepository = foxGameRepository;
    private readonly IMapper _mapper = mapper;
    
    public async Task<ICollection<FoxGameDto>> GetAllFoxGames()
    {
        ICollection<FoxGame> games = await _foxGameRepository.GetAllFoxGames();
        return _mapper.Map<ICollection<FoxGameDto>>(games);
    }

    public ICollection<UserLimitDto> ApplyClosestThresholds(ICollection<UserLimitDto> userLimits)
    {
        foreach (var limitDto in userLimits)
        {
            limitDto.ComputeClosestThreshold();
        }

        return userLimits;
    }

    public ICollection<UserLimit> CreateDefaultLimitsForUser(int userId)
    {
        ICollection<UserLimit> basicInfo = SetupBasicInfo(userId);
        ICollection<UserLimit> foxGames = SetupFoxGames(userId);

        return [.. basicInfo, .. foxGames];
    }
    private static ICollection<UserLimit> SetupBasicInfo(int userId)
    {
        return
        [
            new UserLimit {
                UserId = userId,
                Type = LimitType.UserExp,
                LimitId = (int)LimitType.UserExp,
                CurrentValue = 0
            },
            new UserLimit {
                UserId = userId,
                Type = LimitType.Avatar,
                LimitId = (int)AvatarName.Default,
                CurrentValue = 1
            }
        ];
    }

    private static ICollection<UserLimit> SetupFoxGames(int userId)
    {
        return
        [
            new UserLimit {
                UserId = userId,
                Type = LimitType.PermissionGame,
                LimitId = (int)FoxGameName.Dylematy,
                CurrentValue = 1
            },
            new UserLimit {
                UserId = userId,
                Type = LimitType.PermissionGame,
                LimitId = (int)FoxGameName.Psych,
                CurrentValue = 1
            }
        ];
    }
}
