using FoxTales.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class RoleSeeder(FoxTalesDbContext context, ILogger<RoleSeeder> logger)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<RoleSeeder> _logger = logger;

    public async Task SeedAsync()
    {
        if (!_context.Roles.Any())
        {
            IEnumerable<Role> roles = GetRoles();
            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
    }

    private static List<Role> GetRoles()
    {
        var roles = new List<Role>
            {
                new()
                {
                    RoleId = 1,
                    Name = "User",
                },
                new()
                {
                    RoleId = 2,
                    Name = "Super User",
                },
                new()
                {
                    RoleId = 3,
                    Name = "Admin",
                }
            };

        return roles;
    }
}

