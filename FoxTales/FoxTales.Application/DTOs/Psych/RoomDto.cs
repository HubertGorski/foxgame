using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.DTOs.Psych;

public class RoomDto
{
    public string? Code { get; set; }
    public required PlayerDto Owner { get; set; }
    public bool IsPublic { get; set; }
    public List<PlayerDto> Users { get; set; } = [];
    public string? Password { get; set; }
    public bool UsePublicQuestions { get; set; }
    public bool UsePrivateQuestions { get; set; }
    public bool IsQuestionsFromAnotherGamesAllowed { get; set; }
    public List<QuestionDto> Questions { get; set; } = [];
    public QuestionDto? CurrentQuestion { get; set; }
    public bool IsGameStarted { get; set; } = false;
    public bool HasGameEnded { get; set; } = false;
    public int Round { get; set; } = 0;
    public bool UseDixitRules { get; set; } = false;
    public bool IsTeamModeEnabled { get; set; } = false;
    public int? SelectedPublicCatalogId { get; set; }
}
