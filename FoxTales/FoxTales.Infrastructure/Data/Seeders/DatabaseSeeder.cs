using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class DatabaseSeeder(FoxTalesDbContext context, ILogger<DatabaseSeeder> logger, AchievementSeeder achievementSeeder, RoleSeeder roleSeeder, FoxGamesSeeder foxGamesSeeder, LimitThresholdSeeder limitThresholdSeeder)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<DatabaseSeeder> _logger = logger;
    private readonly AchievementSeeder _achievementSeeder = achievementSeeder;
    private readonly FoxGamesSeeder _foxGamesSeeder = foxGamesSeeder;
    private readonly RoleSeeder _roleSeeder = roleSeeder;
    private readonly LimitThresholdSeeder _limitThresholdSeeder = limitThresholdSeeder;

    public async Task SeedAsync()
    {
        if (!await _context.Database.CanConnectAsync())
        {
            _logger.LogError("A connection to the database cannot be made");
            return;
        }

        await _achievementSeeder.SeedAsync();
        await _roleSeeder.SeedAsync();
        await _foxGamesSeeder.SeedAsync();
        await _limitThresholdSeeder.SeedAsync();
        _logger.LogInformation("Database seeding completed successfully");
    }
}