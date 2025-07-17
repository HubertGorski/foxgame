namespace FoxTales.Application.DTOs.Catalog;

public class CreateCatalogDto
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public int OwnerId { get; set; }

    public required int CatalogTypeId { get; set; }

    public List<int> AvailableTypeIds { get; set; } = [];
}
