using FoxTales.Domain.Entities;

namespace FoxTales.Application.Interfaces;

public interface IUserLimitService
{
    ICollection<UserLimit> CreateDefaultLimitsForUser(int userId);
}
