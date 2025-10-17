using FoxTales.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class AchievementSeeder(FoxTalesDbContext context, ILogger<AchievementSeeder> logger) : IClearableSeeder
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<AchievementSeeder> _logger = logger;

    public async Task ClearAsync()
    {
        if (_context.Achievements.Any())
            _context.Achievements.RemoveRange(_context.Achievements);
    }

    public async Task SeedAsync()
    {
        if (!_context.Achievements.Any())
        {
            _logger.LogInformation("Start achievements seeding");
            IEnumerable<Achievement> achievements = GetAchievements();
            await _context.Achievements.AddRangeAsync(achievements);
        }
    }

    private static List<Achievement> GetAchievements()
    {
        var achievements = new List<Achievement>
            {
                new()
                {
                    Title = "Pierwsze kroki",
                    Subtitle = "Rozpocznij przygodę",
                    Description = "Zaloguj się po raz pierwszy",
                    IconPath = "icons/first-steps.png"
                },
                new()
                {
                    Title = "Eksplorator",
                    Subtitle = "Odkrywca terenu",
                    Description = "Odwiedź wszystkie sekcje aplikacji",
                    IconPath = "icons/explorer.png"
                },
                new()
                {
                    Title = "Mistrz",
                    Subtitle = "Prawdziwy profesjonalista",
                    Description = "Ukończ wszystkie zadania",
                    IconPath = "icons/champion.png"
                }
            };

        return achievements;
    }
}

