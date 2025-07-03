using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class DatabaseSeeder(FoxTalesDbContext context, ILogger<DatabaseSeeder> logger, AchievementSeeder achievementSeeder, RoleSeeder roleSeeder, FoxGamesSeeder foxGamesSeeder, LimitThresholdSeeder limitThresholdSeeder, AvatarsSeeder avatarSeeder)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<DatabaseSeeder> _logger = logger;
    private readonly AchievementSeeder _achievementSeeder = achievementSeeder;
    private readonly FoxGamesSeeder _foxGamesSeeder = foxGamesSeeder;
    private readonly RoleSeeder _roleSeeder = roleSeeder;
    private readonly LimitThresholdSeeder _limitThresholdSeeder = limitThresholdSeeder;
    private readonly AvatarsSeeder _avatarSeeder = avatarSeeder;

    public async Task SeedAsync(bool clearDatabase = false, bool deleteDatabase = false)
    {
        if (!await _context.Database.CanConnectAsync())
        {
            _logger.LogError("A connection to the database cannot be made");
            return;
        }


        if (deleteDatabase)
        {
            await _context.Database.EnsureDeletedAsync();
        }


        if (clearDatabase)
        {
            _logger.LogInformation("Clearing database before seeding...");
            await ClearDatabaseAsync();
        }

        await _achievementSeeder.SeedAsync();
        await _roleSeeder.SeedAsync();
        await _foxGamesSeeder.SeedAsync();
        await _limitThresholdSeeder.SeedAsync();
        await _avatarSeeder.SeedAsync();

        _logger.LogInformation("Database seeding completed successfully");
    }

    private async Task ClearDatabaseAsync()
    {
        _context.Achievements.RemoveRange(_context.Achievements);
        _context.Roles.RemoveRange(_context.Roles);
        _context.FoxGames.RemoveRange(_context.FoxGames);
        _context.LimitThresholds.RemoveRange(_context.LimitThresholds);
        _context.LimitDefinitions.RemoveRange(_context.LimitDefinitions);
        _context.Avatars.RemoveRange(_context.Avatars);

        await _context.SaveChangesAsync();
    }
}