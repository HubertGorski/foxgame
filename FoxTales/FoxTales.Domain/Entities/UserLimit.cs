using System.ComponentModel.DataAnnotations.Schema;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class UserLimit
{
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public LimitType Type { get; set; }
    public int LimitId { get; set; }

    public int CurrentValue { get; set; }

    public virtual LimitDefinition LimitDefinition { get; set; } = null!;
}
