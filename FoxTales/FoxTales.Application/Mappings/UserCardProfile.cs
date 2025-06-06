using System;
using AutoMapper;
using FoxTales.Application.DTOs.UserCard;
using FoxTales.Domain.Entities;

namespace FoxTales.Application.Mappings;

public class UserCardProfile : Profile
{
    public UserCardProfile()
    {
        CreateMap<User, UserWithCardsDto>()
            .ReverseMap();
    }
}
