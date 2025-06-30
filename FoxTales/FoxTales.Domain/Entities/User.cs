using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public ICollection<DylematyCard> Cards { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public virtual ICollection<UserLimit> UserLimits { get; set; } = [];
    [NotMapped]
    public IEnumerable<UserLimit> AchievementLimits => UserLimits.Where(l => l.Type == LimitType.Achievement);
    [NotMapped]
    public IEnumerable<UserLimit> PermissionLimits => UserLimits.Where(l => l.Type == LimitType.Permission);
    [NotMapped]
    public IEnumerable<UserLimit> AvailableFoxGames => UserLimits.Where(l => l.Type == LimitType.PermissionGame);

}
