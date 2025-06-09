namespace FoxTales.Application.DTOs.User;

public class UserDto
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}
