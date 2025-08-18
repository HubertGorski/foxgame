using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.DTOs.Psych;

public class PlayerDto
{
    public int UserId { get; set; }
    public string? ConnectionId { get; set; } = null;
    public required string Username { get; set; }
    public required AvatarDto Avatar { get; set; }
    public bool IsReady { get; set; } = false;
    public int PointsInGame { get; set; }
    public AnswerDto? Answer { get; set; }
}
