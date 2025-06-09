namespace FoxTales.Domain.Entities;

public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;

    public ICollection<DylematyCard> Cards { get; set; } = [];
}
