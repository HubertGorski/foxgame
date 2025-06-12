using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Exceptions;
using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoxTales.Application.Helpers;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? throw new ConfigException("Jwt Settings not found");


    public TokensResponseDto GetTokens(User user)
    {
        string accessToken = GenerateAccessToken(user);
        RefreshToken refreshToken = GenerateRefreshToken(user.UserId);

        CookieOptions options = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        return new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Options = options
        };
    }


    private string GenerateAccessToken(User user)
    {
        ICollection<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Name),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken GenerateRefreshToken(int userId)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenEntity = new RefreshToken
        {
            Token = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            UserId = userId
        };

        return tokenEntity;
    }
}

public class JwtSettings
{
    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int AccessTokenExpiryMinutes { get; set; }
    public int RefreshTokenExpiryDays { get; set; }
}