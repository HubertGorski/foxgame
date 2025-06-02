using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IDylematyRepository
{
    Task<IEnumerable<DylematyCard>> GetAllCards();
}
