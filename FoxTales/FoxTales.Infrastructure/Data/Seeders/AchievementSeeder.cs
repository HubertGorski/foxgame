using Hub.Identity.Entities;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class AchievementSeeder(IdentityDbContext context, ILogger<AchievementSeeder> logger)
{
    private readonly IdentityDbContext _context = context;
    private readonly ILogger<AchievementSeeder> _logger = logger;

    public async Task SeedAsync()
    {
        if (!await _context.Database.CanConnectAsync())
        {
            _logger.LogWarning("A connection to the database cannot be made");
            return;
        }

        if (!_context.Achievements.Any())
        {
            IEnumerable<Achievement> achievements = GetAchievements();
            await _context.Achievements.AddRangeAsync(achievements);
            await _context.SaveChangesAsync();
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

