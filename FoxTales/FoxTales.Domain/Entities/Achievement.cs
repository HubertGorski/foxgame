namespace FoxTales.Domain.Entities;

public class Achievement
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Subtitle { get; set; }
    public required string Description { get; set; }
    public string? IconPath { get; set; } = null;

}
