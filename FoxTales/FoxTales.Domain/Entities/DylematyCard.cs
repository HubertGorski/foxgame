using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class DylematyCard
{
    public Guid CardId { get; set; } = Guid.NewGuid();
    public required string Text { get; set; }
    public DylematyCardType Type { get; set; }

    public required Guid OwnerId { get; set; }
    public required virtual User Owner { get; set; }
}
