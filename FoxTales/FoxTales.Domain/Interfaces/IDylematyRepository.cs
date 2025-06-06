using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IDylematyRepository
{
    Task<ICollection<DylematyCard>> GetAllCards();
    Task AddCard(DylematyCard card);
}
