using FoxTales.Application.Exceptions;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Repositories;

public class EfUserRepository(FoxTalesDbContext db) : IUserRepository
{
    private readonly FoxTalesDbContext _db = db;

    public async Task<int> AddAsync(User user)
    {
        _db.Users.Add(user);
        return await _db.SaveChangesAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<ICollection<Avatar>> GetAllAvatars()
    {
        return await _db.Avatars.ToListAsync();
    }

    public async Task<User?> GetUserById(int userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUser(string? email, int? userId)
    {
        var query = _db.Users
            .AsNoTracking()
            .Include(u => u.Avatar)
            .Include(u => u.Role)
            .Include(u => u.Questions)
                .ThenInclude(u => u.Catalogs)
            .Include(u => u.Catalogs)
                .ThenInclude(u => u.Questions)
            .Include(u => u.Catalogs)
                .ThenInclude(u => u.CatalogType)
            .Include(u => u.Catalogs)
                .ThenInclude(u => u.AvailableTypes)
            .Include(u => u.UserLimits)
                .ThenInclude(ul => ul.LimitDefinition)
                .ThenInclude(ut => ut.Thresholds)
            .AsSplitQuery();

        if (!string.IsNullOrEmpty(email))
            return await query.FirstOrDefaultAsync(u => u.Email == email);

        if (userId != null)
            return await query.FirstOrDefaultAsync(u => u.UserId == userId);

        return null;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username && u.UserStatus != UserStatus.Deleted);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<int> StoreRefreshToken(RefreshToken refreshToken)
    {
        _db.RefreshTokens.Add(refreshToken);
        return await _db.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetRefreshTokenWithUser(string refreshToken)
    {
        var tokenEntity = await _db.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken)
            ?? throw new UnauthorizedException("Refresh token not found");

        return tokenEntity;
    }

    public async Task RevokeRefreshToken(RefreshToken tokenEntity)
    {
        tokenEntity.IsRevoked = true;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteUser(User user)
    {
        user.UserStatus = UserStatus.Deleted;
        await _db.SaveChangesAsync();
    }

    public async Task ClearTokens()
    {
        _db.RefreshTokens.RemoveRange(_db.RefreshTokens);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> SetUsername(string username, int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        user.Username = username;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetAvatar(int avatarId, int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        user.AvatarId = avatarId;
        await _db.SaveChangesAsync();

        return true;
    }
}
