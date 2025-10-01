using System.Text;
using FluentValidation;
using DotNetEnv;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Exceptions;
using FoxTales.Application.Helpers;
using FoxTales.Application.Interfaces;
using FoxTales.Application.Mappings;
using FoxTales.Application.Services;
using FoxTales.Application.Validators.User;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;
using FoxTales.Infrastructure.Data;
using FoxTales.Infrastructure.Data.Seeders;
using FoxTales.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Application.Services.Psych;
using FoxTales.Application.Interfaces.Logics;
using FoxTales.Application.Services.Logics;

namespace FoxTales.Composition;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserLimitService, UserLimitService>();
        services.AddScoped<IPsychLibraryService, PsychLibraryService>();
        services.AddScoped<IDylematyService, DylematyService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IRoundService, RoundService>();
        services.AddScoped<IRoundLogic, RoundLogic>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IValidator<RegisterTmpUserDto>, RegisterTmpUserDtoValidator>();
        services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
        services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
        services.AddScoped<IValidator<SetUsernameRequestDto>, SetUsernameRequestDtoValidator>();
        services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DylematyCardProfile>();
                cfg.AddProfile<UserProfile>();
            });

        JwtSettings jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? throw new ConfigException("Jwt Settings not found");

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
                SaveSigninToken = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        });

        return services;
    }

    public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = "";
        bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (isDocker)
        {
            connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnection") ?? "";
        }
        else
        {
            Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName, ".env"));
            connectionString = Environment.GetEnvironmentVariable("ConnectionStringLocal") ?? "";
        }

        services.AddDbContext<FoxTalesDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IPsychLibraryRepository, EfPsychLibraryRepository>();
        services.AddScoped<IDylematyRepository, EfDylematyRepository>();
        services.AddScoped<IFoxGameRepository, EfFoxGameRepository>();

        services.AddScoped<DatabaseSeeder>();
        services.AddTransient<AchievementSeeder>();
        services.AddTransient<RoleSeeder>();
        services.AddTransient<CatalogTypesSeeder>();
        services.AddTransient<FoxGamesSeeder>();
        services.AddTransient<LimitThresholdSeeder>();
        services.AddTransient<AvatarsSeeder>();
        services.AddTransient<PublicQuestionsSeeder>();

        await services.MigrateDatabaseAsync();
        await services.SeedDatabaseAsync();
        return services;
    }

    private static async Task MigrateDatabaseAsync(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<FoxTalesDbContext>();

        await context.Database.MigrateAsync();
    }

    private static async Task SeedDatabaseAsync(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        await using var scope = serviceProvider.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}