namespace Hub.Identity.Entities;

public class Achievement
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = null!;
    public string Subtitle { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? IconPath { get; set; } = null;

}
