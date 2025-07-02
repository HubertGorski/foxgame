using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.User;

public class AvatarDto
{
    public int AvatarId { get; set; }
    public required AvatarName Name { get; set; }
    public required string Source { get; set; }
    public required bool IsPremium { get; set; }
}
