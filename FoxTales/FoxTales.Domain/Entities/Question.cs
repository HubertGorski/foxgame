using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class Question
{
    public int? Id { get; set; }
    public required string Text { get; set; }
    public bool IsPublic { get; set; }
    public required Language Language { get; set; }
    public DateTime CreatedDate { get; set; }

    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;
}