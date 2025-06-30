using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.User;

public class UserLimitDto
{
    public LimitType Type { get; set; }
    public int LimitId { get; set; }
    public int CurrentValue { get; set; }
    public int? ClosestThreshold { get; set; }

    public ICollection<int> Thresholds { get; set; } = [];

    public void ComputeClosestThreshold()
    {
        ClosestThreshold = Thresholds
            .Where(t => t > CurrentValue)
            .OrderBy(t => t)
            .FirstOrDefault();
    }
}
