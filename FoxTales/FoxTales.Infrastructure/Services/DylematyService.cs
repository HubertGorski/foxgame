using FoxTales.Application.Interfaces;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Infrastructure.Services;

public class DylematyService(IDylematyRepository dylematyRepository) : IDylematyService
{
    private readonly IDylematyRepository _dylematyRepository = dylematyRepository;

    public async Task<IEnumerable<DylematyCard>> GetAllCards()
    {
        IEnumerable<DylematyCard> cards = await _dylematyRepository.GetAllCards();
        return cards;
    }

    public async Task AddCard(DylematyCard card)
    {
        await _dylematyRepository.AddCard(card);
    }
}
