using FoxTales.Application.DTOs.FoxGame;

namespace FoxTales.Application.Interfaces;

public interface IFoxGameService
{
    Task<ICollection<FoxGameDto>> GetAllFoxGames();
}
