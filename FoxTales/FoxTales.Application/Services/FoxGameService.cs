using AutoMapper;
using FoxTales.Application.DTOs.FoxGame;
using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Services;

public class FoxGameService(IFoxGameRepository foxGameRepository, IMapper mapper) : IFoxGameService
{
    private readonly IFoxGameRepository _foxGameRepository = foxGameRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ICollection<FoxGameDto>> GetAllFoxGames()
    {
        ICollection<FoxGame> games = await _foxGameRepository.GetAllFoxGames();
        return _mapper.Map<ICollection<FoxGameDto>>(games);
    }
}