using FoxTales.Application.DTOs.Catalog;
using FoxTales.Application.DTOs.FoxGame;
using FoxTales.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FoxTales.Application.DTOs.User;

public class LoginUserResponseDto
{
    public required UserDto User { get; set; }
    public required CookieOptions Options { get; set; }
    public required RefreshToken RefreshToken { get; set; }
    public required ICollection<FoxGameDto> FoxGames { get; set; }
    public required ICollection<AvatarDto> Avatars { get; set; }
    public required ICollection<CatalogTypeDto> AvailableCatalogTypes { get; set; }
    public required ICollection<CatalogDto> PublicCatalogs { get; set; }
}
