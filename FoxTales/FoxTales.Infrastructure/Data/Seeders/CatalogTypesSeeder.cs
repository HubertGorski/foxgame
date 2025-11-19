using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class CatalogTypesSeeder(FoxTalesDbContext context, ILogger<CatalogTypesSeeder> logger) : ISeeder
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
        }
    }

    private static List<CatalogType> GetCatalogTypes()
    {
        var catalogTypes = new List<CatalogType>
            {
                new()
                {
                    CatalogTypeId = (int)CatalogTypeName.Small,
                    Name = CatalogTypeName.Small,
                    Size = 10
                },
                new()
                {
                    CatalogTypeId = (int)CatalogTypeName.Medium,
                    Name = CatalogTypeName.Medium,
                    Size = 20
                },
                new()
                {
                    CatalogTypeId = (int)CatalogTypeName.Large,
                    Name = CatalogTypeName.Large,
                    Size = 45
                },
                new()
                {
                    CatalogTypeId = (int)CatalogTypeName.NoLimit,
                    Name = CatalogTypeName.NoLimit,
                    Size = 99999
                },
                new()
                {
                    CatalogTypeId = (int)CatalogTypeName.Public,
                    Name = CatalogTypeName.Public,
                    Size = 100
                }
            };

        return catalogTypes;
    }
}

