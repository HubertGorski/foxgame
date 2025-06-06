using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.Dylematy;

public class DylematyCardDto
{
    public Guid Id { get; set; }
    public required string Text { get; set; }
    public DylematyCardType Type { get; set; }
    public required Guid OwnerId { get; set; }
}
