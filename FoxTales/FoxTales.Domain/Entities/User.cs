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
    public required int AvatarId { get; set; }
    public virtual Avatar Avatar { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public ICollection<DylematyCard> Cards { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public virtual ICollection<UserLimit> UserLimits { get; set; } = [];

}
