using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;

namespace FoxTales.Application.Services;

public class UserLimitService() : IUserLimitService
{ 
    public ICollection<UserLimit> CreateDefaultLimitsForUser(int userId)
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
