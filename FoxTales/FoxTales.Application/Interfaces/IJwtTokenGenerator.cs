using FoxTales.Application.DTOs.User;
using FoxTales.Domain.Entities;

namespace FoxTales.Application.Interfaces;

public interface IJwtTokenGenerator
{
    TokensResponseDto GetTokens(UserDto user);
}
