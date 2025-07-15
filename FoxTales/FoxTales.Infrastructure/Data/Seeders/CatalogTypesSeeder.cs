using FoxTales.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class CatalogTypesSeeder(FoxTalesDbContext context, ILogger<CatalogTypesSeeder> logger)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<CatalogTypesSeeder> _logger = logger;

    public async Task SeedAsync()
    {
        if (!_context.CatalogTypes.Any())
        {
            _logger.LogInformation("Start catalog types seeding");
            IEnumerable<CatalogType> catalogTypes = GetCatalogTypes();
            await _context.CatalogTypes.AddRangeAsync(catalogTypes);
            await _context.SaveChangesAsync();
        }
    }

    private static List<CatalogType> GetCatalogTypes()
    {
        var catalogTypes = new List<CatalogType>
            {
                new()
                {
                    Name = "Small",
                    Size = 10
                },
                new()
                {
                    Name = "Medium",
                    Size = 20
                },
                new()
                {
                    Name = "Large",
                    Size = 45
                },
                new()
                {
                    Name = "NoLimit",
                    Size = 99999
                }
            };

        return catalogTypes;
    }
}

