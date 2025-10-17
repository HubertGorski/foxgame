using FoxTales.Infrastructure.Data;
using FoxTales.Infrastructure.Data.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoxTales.Composition;

public static partial class DependencyInjection
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        await services.MigrateDatabaseAsync();
        await services.SeedDatabaseAsync();
    }

    private static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<FoxTalesDbContext>();

        await context.Database.MigrateAsync();
    }

    private static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}



