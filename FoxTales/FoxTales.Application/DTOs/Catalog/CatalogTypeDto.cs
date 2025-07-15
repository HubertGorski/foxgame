using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.Catalog;

public class CatalogTypeDto
{
    public required int CatalogTypeId { get; set; }
    public required CatalogTypeName Name { get; set; }
    public required int Size { get; set; }
}
