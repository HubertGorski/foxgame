namespace FoxTales.Application.DTOs.User;

public class UserDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public ICollection<UserLimitDto> UserLimits { get; set; } = [];
}
