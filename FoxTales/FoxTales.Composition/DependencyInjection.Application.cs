using FluentValidation;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces;
using FoxTales.Application.Interfaces.Logics;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Application.Interfaces.Stores;
using FoxTales.Application.Mappings;
using FoxTales.Application.Services;
using FoxTales.Application.Services.Logics;
using FoxTales.Application.Services.Psych;
using FoxTales.Application.Services.Stores;
using FoxTales.Application.Validators.User;
using FoxTales.Domain.Entities;
using FoxTales.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoxTales.Composition;

public static partial class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.AddSingleton<IRoomStore, RoomStore>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IRoundLogic, RoundLogic>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddServices();
        services.AddValidators();
        services.AddMappers();

        return services;
    }

    private static void AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DylematyCardProfile>();
                cfg.AddProfile<UserProfile>();
            });
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserLimitService, UserLimitService>();
        services.AddScoped<IPsychLibraryService, PsychLibraryService>();
        services.AddScoped<IDylematyService, DylematyService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IRoundService, RoundService>();

        services.AddHostedService<TokenCleanupService>();
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<RegisterTmpUserDto>, RegisterTmpUserDtoValidator>();
        services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
        services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
        services.AddScoped<IValidator<SetUsernameRequestDto>, SetUsernameRequestDtoValidator>();
    }
}
