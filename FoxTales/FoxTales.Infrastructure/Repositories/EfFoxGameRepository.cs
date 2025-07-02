using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Repositories;

public class EfFoxGameRepository(FoxTalesDbContext db) : IFoxGameRepository
{
    private readonly FoxTalesDbContext _db = db;
    public async Task<ICollection<FoxGame>> GetAllFoxGames()
    {
        return await _db.FoxGames.ToListAsync();
    }

    public async Task<ICollection<FoxGame>> GetFoxGamesByUserId(int userId)
    {
        return await _db.FoxGames
            .Join(_db.UserLimits
                .Where(ul => ul.UserId == userId && ul.Type == Domain.Enums.LimitType.PermissionGame),
                    fg => fg.FoxGameId,
                    ul => ul.LimitId, (fg, ul) => fg)
            .ToListAsync();
    }
}