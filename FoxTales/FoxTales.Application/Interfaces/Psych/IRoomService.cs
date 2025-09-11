using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Interfaces.Psych;

public interface IRoomService
{
    RoomDto GetRoomByCode(string gameCode);
    Task<string> CreateRoomGetCode(RoomDto room);
    Task EditRoom(RoomDto room);
    Task SetStatus(string gameCode, int playerId, bool status);
    Task StartGame(string gameCode, string connectionId);
    Task RefreshPublicRoomsList();
    Task LeaveRoom(string gameCode, int playerId);
    Task AddQuestionsToGame(string gameCode, int playerId, List<QuestionDto> questions);
    Task JoinRoom(string gameCode, PlayerDto player, string? password, int? ownerId);
}
