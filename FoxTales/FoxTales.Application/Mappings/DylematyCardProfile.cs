using AutoMapper;
using FoxTales.Application.DTOs.Dylematy;
using FoxTales.Domain.Entities;

namespace FoxTales.Application.Mappings;

public class DylematyCardProfile : Profile
{
    public DylematyCardProfile()
    {
        CreateMap<DylematyCard, DylematyCardDto>()
            .ReverseMap();

        CreateMap<CreateDylematyCardDto, DylematyCard>()
            .ReverseMap();
    }
}
