using FoxTales.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FoxTales.Application.DTOs.User;

public class LoginUserResponseDto
{
    public required UserDto User { get; set; }
    public required CookieOptions Options { get; set; }
    public required RefreshToken RefreshToken { get; set; }
    public required string AccessToken { get; set; }
}
