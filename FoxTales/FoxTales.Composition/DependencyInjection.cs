using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
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

namespace FoxTales.Composition;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserLimitService, UserLimitService>();
        services.AddScoped<IDylematyService, DylematyService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
        services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
        services.AddScoped<IValidator<SetUsernameRequestDto>, SetUsernameRequestDtoValidator>();
        services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DylematyCardProfile>();
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<UserCardProfile>();
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
        services.AddDbContext<FoxTalesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IDylematyRepository, EfDylematyRepository>();
        services.AddScoped<IFoxGameRepository, EfFoxGameRepository>();

        services.AddScoped<DatabaseSeeder>();
        services.AddTransient<AchievementSeeder>();
        services.AddTransient<RoleSeeder>();
        services.AddTransient<FoxGamesSeeder>();
        services.AddTransient<LimitThresholdSeeder>();
        services.AddTransient<AvatarsSeeder>();

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