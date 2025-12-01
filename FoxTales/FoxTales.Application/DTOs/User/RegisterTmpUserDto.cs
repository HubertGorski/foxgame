namespace FoxTales.Application.DTOs.User;

public class RegisterTmpUserDto
{
    public required string Username { get; set; }
    public required bool TermsAccepted { get; set; }
}
