namespace FoxTales.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public ICollection<DylematyCard> Cards { get; set; } = [];
}
