using FoxTales.Domain.Entities;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Services;

public class DylematyService(IDylematyRepository dylematyRepository) : IDylematyService
{
    private readonly IDylematyRepository _dylematyRepository = dylematyRepository;

    public async Task<IEnumerable<DylematyCard>> GetAllCards()
    {
        IEnumerable<DylematyCard> cards = await _dylematyRepository.GetAllCards();
        return cards;
    }
}
