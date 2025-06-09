using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoxTales.Application.Exceptions;
using FoxTales.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoxTales.Application.Helpers;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? throw new ConfigException("Jwt Settings not found");

    public string GenerateToken(ICollection<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);
            
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class JwtSettings
{
    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpiryMinutes { get; set; }
}