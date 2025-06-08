using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using FoxTales.Infrastructure.Data.Seeders;
using FoxTales.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoxTales.Composition;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FoxTalesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IDylematyRepository, EfDylematyRepository>();

        services.AddScoped<DatabaseSeeder>();
        services.AddTransient<AchievementSeeder>();

        await services.SeedDatabaseAsync();
        return services;
    }

    private static async Task SeedDatabaseAsync(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        await using var scope = serviceProvider.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}