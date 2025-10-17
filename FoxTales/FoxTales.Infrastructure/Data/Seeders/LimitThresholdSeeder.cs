using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class LimitThresholdSeeder(FoxTalesDbContext context, ILogger<LimitThresholdSeeder> logger) : IClearableSeeder
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<LimitThresholdSeeder> _logger = logger;
    public async Task ClearAsync()
    {
        if (_context.LimitThresholds.Any())
            _context.LimitThresholds.RemoveRange(_context.LimitThresholds);

        if (_context.LimitDefinitions.Any())
            _context.LimitDefinitions.RemoveRange(_context.LimitDefinitions);
    }

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
    }

    private static List<LimitDefinition> GetLimitDefinitions()
    {
        var limitDefinitions = new List<LimitDefinition>
            {
                new()
                {
                    Type = LimitType.UserExp,
                    LimitId = (int)LimitType.UserExp,
                },
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
                },
                new()
                {
                    Type = LimitType.Avatar,
                    LimitId = (int)AvatarName.Crazy,
                },
                new()
                {
                    Type = LimitType.Avatar,
                    LimitId = (int)AvatarName.Default,
                },
                new()
                {
                    Type = LimitType.Avatar,
                    LimitId = (int)AvatarName.Happy,
                },
                new()
                {
                    Type = LimitType.Avatar,
                    LimitId = (int)AvatarName.Sad,
                },
                new()
                {
                    Type = LimitType.Avatar,
                    LimitId = (int)AvatarName.New,
                },
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
                    LimitId = (int)FoxGameName.KillGame,
                    ThresholdValue = 1,
                },
                new()
                {
                    Type = LimitType.UserExp,
                    LimitId = (int)LimitType.UserExp,
                    ThresholdValue = 6,
                },
                new()
                {
                    Type = LimitType.UserExp,
                    LimitId = (int)LimitType.UserExp,
                    ThresholdValue = 14,
                },
                new()
                {
                    Type = LimitType.UserExp,
                    LimitId = (int)LimitType.UserExp,
                    ThresholdValue = 31,
                }
            };

        return limitThreshold;
    }
}

