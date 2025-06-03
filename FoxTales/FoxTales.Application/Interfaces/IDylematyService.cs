using FoxTales.Application.DTOs;

namespace FoxTales.Application.Interfaces;

public interface IDylematyService
{
    Task<IEnumerable<DylematyCardDto>> GetAllCards();
    Task AddCard(CreateDylematyCardDto card);
}
