using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.Dylematy;

public class CreateDylematyCardDto
{
    public string Text { get; set; } = null!;
    public DylematyCardType Type { get; set; }
}
