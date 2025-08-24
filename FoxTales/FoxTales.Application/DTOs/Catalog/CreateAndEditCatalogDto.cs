using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.DTOs.Catalog;

public class CreateAndEditCatalogDto
{
    public int? CatalogId { get; set; }
    public required string Title { get; set; }

    public string? Description { get; set; }

    public int OwnerId { get; set; }

    public required int CatalogTypeId { get; set; }

    public List<int> AvailableTypeIds { get; set; } = [];
    public List<int> QuestionsIds { get; set; } = [];
    public ICollection<QuestionDto> Questions { get; set; } = [];
}
