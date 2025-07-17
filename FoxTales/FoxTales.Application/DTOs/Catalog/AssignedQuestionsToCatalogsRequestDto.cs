namespace FoxTales.Application.DTOs.Catalog;

public class AssignedQuestionsToCatalogsRequestDto
{
    public List<int> QuestionsIds { get; set; } = [];
    public List<int> CatalogsIds { get; set; } = [];
}
