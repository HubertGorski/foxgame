using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.Dylematy;

public class CreateDylematyCardDto
{
    public required string Text { get; set; }
    public DylematyCardType Type { get; set; }
    public required int OwnerId { get; set; }
}
