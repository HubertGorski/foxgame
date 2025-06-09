using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Repositories;

public class EfUserRepository(FoxTalesDbContext db) : IUserRepository
{
    private readonly FoxTalesDbContext _db = db;

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<ICollection<User>> GetAllUsers()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User?> GetUserById(int userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ICollection<User>> GetAllUsersWithCards()
    {
        return await _db.Users
            .Include(u => u.Cards)
            .ToListAsync();
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }
}
