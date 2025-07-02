using System.ComponentModel.DataAnnotations;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class FoxGame
{

    [Key]
    public int FoxGameId { get; set; }
    public required FoxGameName Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
