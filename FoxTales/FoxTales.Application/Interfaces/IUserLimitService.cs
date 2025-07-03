using FoxTales.Application.DTOs.FoxGame;
using FoxTales.Application.DTOs.User;
using FoxTales.Domain.Entities;

namespace FoxTales.Application.Interfaces;

public interface IUserLimitService
{
    Task<ICollection<FoxGameDto>> GetAllFoxGames();
    ICollection<UserLimitDto> ApplyClosestThresholds(ICollection<UserLimitDto> userLimits);
    ICollection<UserLimit> CreateDefaultLimitsForUser(int userId);
}
