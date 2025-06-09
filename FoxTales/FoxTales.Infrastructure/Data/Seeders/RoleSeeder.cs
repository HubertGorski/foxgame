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
                    Name = "User",
                },
                new()
                {
                    Name = "Super User",
                },
                new()
                {
                    Name = "Admin",
                }
            };

        return roles;
    }
}

