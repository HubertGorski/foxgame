using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class LimitThresholdSeeder(FoxTalesDbContext context, ILogger<LimitThresholdSeeder> logger)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<LimitThresholdSeeder> _logger = logger;

    public async Task SeedAsync()
    {
        if (!_context.LimitDefinitions.Any())
        {
            _logger.LogInformation("Start limits definitions seeding");
            IEnumerable<LimitDefinition> limitDefinitions = GetLimitDefinitions();
            await _context.LimitDefinitions.AddRangeAsync(limitDefinitions);
        }

        if (!_context.LimitThresholds.Any())
        {
            _logger.LogInformation("Start limits thresholds seeding");
            IEnumerable<LimitThreshold> limitThresholds = GetLimitThreshold();
            await _context.LimitThresholds.AddRangeAsync(limitThresholds);
        }
        
        await _context.SaveChangesAsync();
    }

    private static List<LimitDefinition> GetLimitDefinitions()
    {
        var limitDefinitions = new List<LimitDefinition>
            {
                new()
                {
                    Type = LimitType.PermissionGame,
                    LimitId = (int)FoxGameName.Psych,
                },
                new()
                {
                    Type = LimitType.PermissionGame,
                    LimitId = (int)FoxGameName.Dylematy,
                },
                new()
                {
                    Type = LimitType.PermissionGame,
                    LimitId = (int)FoxGameName.KillGame,
                }
            };

        return limitDefinitions;
    }

    private static List<LimitThreshold> GetLimitThreshold()
    {
        var limitThreshold = new List<LimitThreshold>
            {
                new()
                {
                    Type = LimitType.PermissionGame,
                    LimitId = (int)FoxGameName.Psych,
                    ThresholdValue = 1,
                },
                new()
                {
                    Type = LimitType.PermissionGame,
                    LimitId = (int)FoxGameName.Dylematy,
                    ThresholdValue = 1,
                },
                new()
                {
                    Type = LimitType.PermissionGame,
                    LimitId = (int)FoxGameName.Dylematy,
                    ThresholdValue = 2,
                },
                new()
                {
                    Type = LimitType.PermissionGame,
                    LimitId = (int)FoxGameName.KillGame,
                    ThresholdValue = 1,
                }
            };

        return limitThreshold;
    }
}

