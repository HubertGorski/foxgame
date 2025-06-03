using AutoMapper;
using FoxTales.Application.DTOs;
using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Services;

public class DylematyService(IDylematyRepository dylematyRepository, IMapper mapper) : IDylematyService
{
    private readonly IDylematyRepository _dylematyRepository = dylematyRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<DylematyCardDto>> GetAllCards()
    {
        IEnumerable<DylematyCard> cards = await _dylematyRepository.GetAllCards();
        return _mapper.Map<List<DylematyCardDto>>(cards);
    }

    public async Task AddCard(CreateDylematyCardDto createDylematyCardDto)
    {
        DylematyCard card = _mapper.Map<DylematyCard>(createDylematyCardDto);
        await _dylematyRepository.AddCard(card);
    }
}
