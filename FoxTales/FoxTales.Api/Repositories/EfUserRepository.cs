using FoxTales.Api.Data;
using Hub.Identity.Entities;
using Hub.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Api.Repositories;

public class EfUserRepository(IdentityDbContext db) : IUserRepository
{
    private readonly IdentityDbContext _db = db;

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username);
    }
}
