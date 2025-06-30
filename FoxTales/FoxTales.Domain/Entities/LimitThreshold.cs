using System.ComponentModel.DataAnnotations;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class LimitThreshold
{
    [Key]
    public int Id { get; set; }

    public LimitType Type { get; set; }
    public int LimitId { get; set; }
    public int ThresholdValue { get; set; }

    public virtual LimitDefinition LimitDefinition { get; set; } = null!;
}
