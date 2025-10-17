using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class DatabaseSeeder(FoxTalesDbContext context, ILogger<DatabaseSeeder> logger, IEnumerable<ISeeder> seeders)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<DatabaseSeeder> _logger = logger;
    private readonly IEnumerable<ISeeder> _seeders = seeders;

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

        if (!clearDatabase)
            return;

        _logger.LogInformation("Clearing database before seeding...");

        foreach (var seeder in _seeders.OfType<IClearableSeeder>())
        {
            _logger.LogDebug("Clearing data using {Seeder}", seeder.GetType().Name);
            await seeder.ClearAsync();

        }
        await _context.SaveChangesAsync();

        foreach (var seeder in _seeders)
        {
            _logger.LogDebug("Seeding data using {Seeder}", seeder.GetType().Name);
            await seeder.SeedAsync();
        }
        await _context.SaveChangesAsync();

        _logger.LogInformation("Database seeding completed successfully");
    }
}