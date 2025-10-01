
using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetUserById(int userId);
    Task<int> AddAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetUser(string? email, int? userId);
    Task<int> StoreRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenWithUser(string refreshToken);
    Task RevokeRefreshToken(RefreshToken tokenEntity);
    Task DeleteUser(User user);
    Task ClearTokens();
    Task<ICollection<Avatar>> GetAllAvatars();
    Task<bool> SetUsername(string username, int userId);
    Task<bool> SetAvatar(int avatarId, int userId);
}
