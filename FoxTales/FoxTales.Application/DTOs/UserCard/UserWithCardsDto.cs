using FoxTales.Application.DTOs.Dylematy;

namespace FoxTales.Application.DTOs.UserCard;

public class UserWithCardsDto
{
    // public required UserDto User { get; set; } TODO: zrobic mapper
    public required string Username { get; set; }
    public required string Email { get; set; }
    public ICollection<DylematyCardDto> Cards { get; set; } = [];
}
