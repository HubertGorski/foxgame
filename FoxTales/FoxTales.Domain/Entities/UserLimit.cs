using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class UserLimit
{
    public Guid Id { get; set; }

    public int UserId { get; set; }
    public required User User { get; set; }

    public required LimitType Type { get; set; }

    public required string LimitName { get; set; }

    public int CurrentValue { get; set; }
}
