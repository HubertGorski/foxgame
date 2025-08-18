namespace FoxTales.Application.DTOs.Psych;

public class AnswerDto
{
    public int AnswerId { get; set; }
    public int QuestionId { get; set; }
    public int OwnerId { get; set; }
    public required string Answer { get; set; }
    public int VotersCount { get; set; } = 0;
}
