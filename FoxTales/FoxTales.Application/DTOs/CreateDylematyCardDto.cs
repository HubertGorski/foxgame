using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs;

public class CreateDylematyCardDto
{
    public string Text { get; set; } = null!;
    public DylematyCardType Type { get; set; }
}
