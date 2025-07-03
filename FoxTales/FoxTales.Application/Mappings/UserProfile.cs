using AutoMapper;
using FoxTales.Application.DTOs.FoxGame;
using FoxTales.Application.DTOs.User;
using FoxTales.Domain.Entities;

namespace FoxTales.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
            .ReverseMap();

        CreateMap<User, RegisterUserDto>()
            .ReverseMap();

        CreateMap<Avatar, AvatarDto>()
            .ReverseMap();

        CreateMap<FoxGame, FoxGameDto>()
            .ReverseMap();

        CreateMap<Question, QuestionDto>()
            .ReverseMap();

        CreateMap<UserLimit, UserLimitDto>()
            .ForMember(dest => dest.ClosestThreshold, opt => opt.Ignore())
            .ForMember(dest => dest.Thresholds, opt => opt.MapFrom(
                src => src.LimitDefinition != null && src.LimitDefinition.Thresholds != null
                    ? src.LimitDefinition.Thresholds.Select(t => t.ThresholdValue)
                    : new List<int>()
            ))
            .ReverseMap();
    }
}