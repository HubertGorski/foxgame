
using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<ICollection<User>> GetAllUsers();
    Task<User?> GetUserById(int userId);
    Task<ICollection<User>> GetAllUsersWithCards();
    Task<int> AddAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetUserByEmail(string email);
    Task<int> StoreRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenWithUser(string refreshToken);
    Task RevokeRefreshToken(RefreshToken tokenEntity);
    Task ClearTokens();
    Task<ICollection<Avatar>> GetAllAvatars();
    Task<bool> SetUsername(string username, int userId);
    Task<bool> SetAvatar(int avatarId, int userId);
    Task<int> AddQuestion(Question question);
    Task<bool> RemoveQuestion(int questionId);
    Task<int> AddCatalog(Catalog catalog);
    Task<bool> EditCatalog(Catalog catalog);
}
