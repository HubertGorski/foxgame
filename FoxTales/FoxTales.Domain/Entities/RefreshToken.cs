namespace FoxTales.Domain.Entities;

public class RefreshToken
{
    public Guid TokenId { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;

    public required int UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
