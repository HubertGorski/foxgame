using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.FoxGame;

public class FoxGameDto
{
    public required int FoxGameId { get; set; }
    public required FoxGameName Name { get; set; }
}