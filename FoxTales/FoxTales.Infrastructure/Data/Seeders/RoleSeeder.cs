using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
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
            _logger.LogInformation("Start roles seeding");
            IEnumerable<Role> roles = GetRoles();
            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
        }
    }

    private static List<Role> GetRoles()
    {
        var roles = new List<Role>
            {
                new()
                {
                    RoleId = (int)RoleName.User,
                    Name = RoleName.User,
                },
                new()
                {
                    RoleId = (int)RoleName.SuperUser,
                    Name = RoleName.SuperUser,
                },
                new()
                {
                    RoleId = (int)RoleName.Admin,
                    Name = RoleName.Admin,
                }
            };

        return roles;
    }
}

