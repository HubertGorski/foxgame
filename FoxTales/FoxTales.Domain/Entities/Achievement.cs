using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoxTales.Domain.Entities;

public class Achievement
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AchievementId { get; set; }
    public required string Title { get; set; }
    public required string Subtitle { get; set; }
    public required string Description { get; set; }
    public string? IconPath { get; set; } = null;

}
