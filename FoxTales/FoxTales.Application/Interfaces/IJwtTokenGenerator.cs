using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Interfaces;

public interface IJwtTokenGenerator
{
    TokensResponseDto GetTokens(UserDto user);
}
