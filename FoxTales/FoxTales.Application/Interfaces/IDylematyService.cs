using FoxTales.Domain.Entities;

namespace FoxTales.Application.Interfaces;

public interface IDylematyService
{
    Task<IEnumerable<DylematyCard>> GetAllCards();
    Task AddCard(DylematyCard card);
}
