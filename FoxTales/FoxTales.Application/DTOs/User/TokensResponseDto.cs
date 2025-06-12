using FoxTales.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FoxTales.Application.DTOs.User;

public class TokensResponseDto
{
    public required CookieOptions Options { get; set; }
    public required RefreshToken RefreshToken { get; set; }
    public required string AccessToken { get; set; }
}
