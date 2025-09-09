using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class AvatarsSeeder(FoxTalesDbContext context, ILogger<AvatarsSeeder> logger)
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
            await _context.SaveChangesAsync();
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
                    Source = $"/src/assets/imgs/defaultAvatars/{(int)AvatarName.Default}.webp",
                    IsPremium = false,
                },
                new()
                {
                    AvatarId = (int)AvatarName.Crazy,
                    Name = AvatarName.Crazy,
                    Source = $"/src/assets/imgs/defaultAvatars/{(int)AvatarName.Crazy}.webp",
                    IsPremium = false,
                },
                new()
                {
                    AvatarId = (int)AvatarName.Happy,
                    Name = AvatarName.Happy,
                    Source = $"/src/assets/imgs/defaultAvatars/{(int)AvatarName.Happy}.webp",
                    IsPremium = true,
                },
                new()
                {
                    AvatarId = (int)AvatarName.New,
                    Name = AvatarName.New,
                    Source = $"/src/assets/imgs/defaultAvatars/{(int)AvatarName.New}.webp",
                    IsPremium = true,
                },
                new()
                {
                    AvatarId = (int)AvatarName.Sad,
                    Name = AvatarName.Sad,
                    Source = $"/src/assets/imgs/defaultAvatars/{(int)AvatarName.Sad}.webp",
                    IsPremium = true,
                }
            };

        return avatars;
    }
}

