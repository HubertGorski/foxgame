using System.Security.Claims;

namespace FoxTales.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(ICollection<Claim> claims);
}
