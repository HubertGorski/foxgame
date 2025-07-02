
using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IFoxGameRepository
{
    Task<ICollection<FoxGame>> GetAllFoxGames();
    Task<ICollection<FoxGame>> GetFoxGamesByUserId(int userId);
}
