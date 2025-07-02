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
                    FoxGameId = (int)FoxGameName.Psych,
                    Name = FoxGameName.Psych,
                },
                new()
                {
                    FoxGameId = (int)FoxGameName.Dylematy,
                    Name = FoxGameName.Dylematy,
                },
                new()
                {
                    FoxGameId = (int)FoxGameName.KillGame,
                    Name = FoxGameName.KillGame,
                }
            };

        return games;
    }
}

