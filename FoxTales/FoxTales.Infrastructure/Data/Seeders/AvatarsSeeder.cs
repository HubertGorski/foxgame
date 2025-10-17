using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class AvatarsSeeder(FoxTalesDbContext context, ILogger<AvatarsSeeder> logger) : ISeeder
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<AvatarsSeeder> _logger = logger;

    public async Task SeedAsync()
    {
        if (!_context.Avatars.Any())
        {
            _logger.LogInformation("Start avatars seeding");
            IEnumerable<Avatar> avatars = GetAvatars();
            await _context.Avatars.AddRangeAsync(avatars);
        }
    }

    private static List<Avatar> GetAvatars()
    {
        var avatars = new List<Avatar>
            {
                new()
                {
                    AvatarId = (int)AvatarName.Default,
                    Name = AvatarName.Default,
                    IsPremium = false,
                },
                new()
                {
                    AvatarId = (int)AvatarName.Crazy,
                    Name = AvatarName.Crazy,
                    IsPremium = false,
                },
                new()
                {
                    AvatarId = (int)AvatarName.Happy,
                    Name = AvatarName.Happy,
                    IsPremium = true,
                },
                new()
                {
                    AvatarId = (int)AvatarName.New,
                    Name = AvatarName.New,
                    IsPremium = true,
                },
                new()
                {
                    AvatarId = (int)AvatarName.Sad,
                    Name = AvatarName.Sad,
                    IsPremium = true,
                }
            };

        return avatars;
    }
}

