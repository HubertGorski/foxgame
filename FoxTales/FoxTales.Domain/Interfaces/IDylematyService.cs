using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IDylematyService
{
    Task<IEnumerable<DylematyCard>> GetAllCards();
}
