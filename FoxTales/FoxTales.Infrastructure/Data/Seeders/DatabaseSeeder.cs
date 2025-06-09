using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class DatabaseSeeder(FoxTalesDbContext context, ILogger<DatabaseSeeder> logger, AchievementSeeder achievementSeeder, RoleSeeder roleSeeder)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<DatabaseSeeder> _logger = logger;
    private readonly AchievementSeeder _achievementSeeder = achievementSeeder;
    private readonly RoleSeeder _roleSeeder = roleSeeder;

    public async Task SeedAsync()
    {
        if (!await _context.Database.CanConnectAsync())
        {
            _logger.LogError("A connection to the database cannot be made");
            return;
        }

        await _achievementSeeder.SeedAsync();
        await _roleSeeder.SeedAsync();
    }
}