using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.Dylematy;

public class DylematyCardDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    public DylematyCardType Type { get; set; }
}
