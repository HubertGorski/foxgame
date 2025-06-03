using AutoMapper;
using FoxTales.Application.DTOs.User;
using FoxTales.Domain.Entities;

namespace FoxTales.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ReverseMap();

        CreateMap<User, RegisterUserDto>()
            .ReverseMap();
    }
}