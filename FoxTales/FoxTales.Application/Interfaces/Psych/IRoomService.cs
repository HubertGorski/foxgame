using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Interfaces.Psych;

public interface IRoomService
{
    RoomDto GetRoomByCode(string gameCode);
    Task CreateRoom(RoomDto room);
    Task EditRoom(RoomDto room);
    Task SetStatus(string gameCode, int playerId, bool status);
    Task StartGame(string gameCode, string connectionId);
    Task RefreshPublicRoomsList();
    Task LeaveRoom(string gameCode, int playerId);
    Task AddPrivateQuestionsToGame(string gameCode, int playerId, List<QuestionDto> privateQuestions);
    Task JoinRoom(PlayerDto player, string? gameCode, string? password, int? ownerId);
    (string? RoomCode, PlayerDto? Player) FindPlayerByConnectionId(string connectionId);
    Task SuspendUserInRoom(string gameCode, int playerId);
    Task ContinuePlaying(int? userId, string connectionId);
}
