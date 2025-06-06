using FoxTales.Application.DTOs.Dylematy;

namespace FoxTales.Application.Interfaces;

public interface IDylematyService
{
    Task<ICollection<DylematyCardDto>> GetAllCards();
    Task AddCard(CreateDylematyCardDto card);
}
