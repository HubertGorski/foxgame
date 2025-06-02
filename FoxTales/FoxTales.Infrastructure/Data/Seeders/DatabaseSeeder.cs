using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class DatabaseSeeder(ILogger<DatabaseSeeder> logger, AchievementSeeder achievementSeeder)
{
    private readonly ILogger<DatabaseSeeder> _logger = logger;
    private readonly AchievementSeeder _achievementSeeder = achievementSeeder;

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            await _achievementSeeder.SeedAsync();

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            throw;
        }
    }
}