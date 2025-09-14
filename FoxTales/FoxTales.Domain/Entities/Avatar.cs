
using System.ComponentModel.DataAnnotations;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class Avatar
{
    [Key]
    public int AvatarId { get; set; }
    public required AvatarName Name { get; set; }
    public required bool IsPremium { get; set; }
}
