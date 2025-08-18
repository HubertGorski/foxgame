using FoxTales.Application.DTOs.Psych;
using FoxTales.Domain.Enums;

namespace FoxTales.Application.DTOs.User;

public class QuestionDto
{
    public int? Id { get; set; }
    public required string Text { get; set; }
    public bool IsPublic { get; set; }
    public required Language Language { get; set; }
    public int OwnerId { get; set; }
    public List<int> CatalogIds { get; set; } = [];
    public PlayerDto? CurrentUser { get; set; }
}