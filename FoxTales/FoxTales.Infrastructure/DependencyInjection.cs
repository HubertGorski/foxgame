using System;
using FoxTales.Infrastructure.Data;
using FoxTales.Infrastructure.Repositories;
using Hub.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoxTales.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default"))

        );

        services.AddScoped<IUserRepository, EfUserRepository>();
        return services;
    }
}
