using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.DTOs.Catalog;

public class CatalogDto
{
    public int? CatalogId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public ICollection<QuestionDto> Questions { get; set; } = [];
    public ICollection<CatalogTypeDto> AvailableTypes { get; set; } = [];
    public required CatalogTypeDto CatalogType { get; set; }
}
