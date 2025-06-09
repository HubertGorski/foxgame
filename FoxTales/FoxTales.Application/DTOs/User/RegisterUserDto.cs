using System.ComponentModel.DataAnnotations;

namespace FoxTales.Application.DTOs.User;

public class RegisterUserDto
{
    public required string Username { get; set; }

    public required string Email { get; set; }

    [MinLength(6)]
    public required string Password { get; set; }
}
