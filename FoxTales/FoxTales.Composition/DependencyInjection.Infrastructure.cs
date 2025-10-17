using System.Text;
using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using FoxTales.Infrastructure.Data.Seeders;
using FoxTales.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using FoxTales.Infrastructure.Authentication;
using FoxTales.Infrastructure.Configuration;
using FoxTales.Application.Exceptions;
using DotNetEnv;

namespace FoxTales.Composition;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();
        services.AddSeeders();
        services.AddJwtAuthentication(configuration);

        string connectionString = ConnectionStringProvider.GetConnectionString(configuration);
        services.AddDbContext<FoxTalesDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IPsychLibraryRepository, EfPsychLibraryRepository>();
        services.AddScoped<IDylematyRepository, EfDylematyRepository>();
        services.AddScoped<IFoxGameRepository, EfFoxGameRepository>();
    }

    private static void AddSeeders(this IServiceCollection services)
    {
        services.AddScoped<DatabaseSeeder>();
        services.AddTransient<ISeeder, AchievementSeeder>();
        services.AddTransient<ISeeder, RoleSeeder>();
        services.AddTransient<ISeeder, CatalogTypesSeeder>();
        services.AddTransient<ISeeder, FoxGamesSeeder>();
        services.AddTransient<ISeeder, LimitThresholdSeeder>();
        services.AddTransient<ISeeder, AvatarsSeeder>();
        services.AddTransient<ISeeder, PublicQuestionsSeeder>();
    }

    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSettings jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? throw new ConfigException("Jwt Settings not found");

        Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName, ".env"));
        jwtSettings.Key ??= Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ConfigException("Missing JWT key (set via environment variable JWT_KEY)");

        services.AddSingleton(jwtSettings);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        });
    }
}