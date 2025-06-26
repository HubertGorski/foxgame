using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class FoxGamesSeeder(FoxTalesDbContext context, ILogger<FoxGamesSeeder> logger)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<FoxGamesSeeder> _logger = logger;

    public async Task SeedAsync()
    {
        if (!_context.FoxGames.Any())
        {
            _logger.LogInformation("Start fox games seeding");
            IEnumerable<FoxGame> games = GetGames();
            await _context.FoxGames.AddRangeAsync(games);
            await _context.SaveChangesAsync();
        }
    }

    private static List<FoxGame> GetGames()
    {
        var games = new List<FoxGame>
            {
                new()
                {
                    Name = FoxGameName.Psych,
                },
                new()
                {
                    Name = FoxGameName.Dylematy,
                },
                new()
                {
                    Name = FoxGameName.KillGame,
                }
            };

        return games;
    }
}

