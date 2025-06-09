using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class DylematyCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CardId { get; set; }
    public required string Text { get; set; }
    public DylematyCardType Type { get; set; }

    public required int OwnerId { get; set; }
    public required virtual User Owner { get; set; }
}
