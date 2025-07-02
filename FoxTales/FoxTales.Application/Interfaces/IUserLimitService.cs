using FoxTales.Application.DTOs.User;
using FoxTales.Domain.Entities;

namespace FoxTales.Application.Interfaces;

public interface IUserLimitService
{
    ICollection<UserLimitDto> ApplyClosestThresholds(ICollection<UserLimitDto> userLimits);
    ICollection<UserLimit> CreateDefaultLimitsForUser(int userId);
}
