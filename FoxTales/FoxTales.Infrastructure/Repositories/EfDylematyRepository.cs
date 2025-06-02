using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Repositories;

public class EfDylematyRepository(FoxTalesDbContext db) : IDylematyRepository
{
    private readonly FoxTalesDbContext _db = db;
    public async Task<IEnumerable<DylematyCard>> GetAllCards()
    {
        return await _db.DylematyCards.ToListAsync();
    }
}
