using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class LimitDefinition
{
    public LimitType Type { get; set; }
    public int LimitId { get; set; }

    public virtual ICollection<LimitThreshold> Thresholds { get; set; } = [];
}