
using FoxTales.Application.DTOs.Psych;

namespace FoxTales.Application.Interfaces.Psych;

public interface IRoundService
{
    Task SetNewRound(RoomDto room, string connectionId);
    Task MarkAllUsersUnreadyIfOwner(RoomDto room, string connectionId);
    Task ChooseAnswer(RoomDto room, int playerId, int selectedAnswerUserId);
    Task AddAnswer(RoomDto room, AnswerDto answer);
}
