using System.ComponentModel.DataAnnotations;

namespace FoxTales.Domain.Entities;

public class Catalog
{
    [Key]
    public int? CatalogId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }

    public int? OwnerId { get; set; }
    public virtual User? Owner { get; set; } = null!;

    public ICollection<Question> Questions { get; set; } = [];
    public ICollection<CatalogType> AvailableTypes { get; set; } = [];

    public int CatalogTypeId { get; set; }
    public CatalogType CatalogType { get; set; } = null!;
}
