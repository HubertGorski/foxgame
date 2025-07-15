using System.ComponentModel.DataAnnotations;

namespace FoxTales.Domain.Entities;

public class CatalogType
{
    [Key]
    public int? CatalogTypeId { get; set; }
    public required string Name { get; set; }
    public required int Size { get; set; }
}
