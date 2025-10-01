
using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IUserRepository
{
    Task SaveChangesAsync();
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetUserById(int userId);
    Task<int> AddAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetUser(string? email, int? userId);
    Task<int> StoreRefreshToken(RefreshToken refreshToken);
    Task<List<RefreshToken>> GetInactiveTokens(DateTime now, TimeSpan accessTokenTtl);
    Task<RefreshToken> GetRefreshTokenWithUser(string refreshToken);
    Task ClearTokens();
    Task<ICollection<Avatar>> GetAllAvatars();
    Task<bool> SetUsername(string username, int userId);
    Task<bool> SetAvatar(int avatarId, int userId);
}
