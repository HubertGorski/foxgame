using FoxTales.Application.DTOs.Psych;
using GameCode = System.String;
using FoxTales.Application.Helpers;
using FoxTales.Application.Interfaces.Psych;
using MediatR;
using FoxTales.Application.Events;
using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Services.Psych;

public class RoomService(IMediator mediator, IRoundService roundService) : IRoomService
{
    private readonly IRoundService _roundService = roundService;
    private readonly IMediator _mediator = mediator;
    private static readonly Dictionary<GameCode, RoomDto> Rooms = [];

    public RoomDto GetRoomByCode(string gameCode)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null)
            throw new InvalidOperationException($"Code '{gameCode}' doesnt exist");

        return room;
    }

    public async Task<string> CreateRoomGetCode(RoomDto room)
    {
        room.Code = RoomCodeGenerator.GenerateUniqueCode(code => !Rooms.ContainsKey(code));
        await RemoveAllRoomsByOwnerId(room.Owner.UserId);
        room.Owner.IsReady = false;
        room.Users.Add(room.Owner);
        Rooms.Add(room.Code, room);
        return room.Code;
    }

    public async Task EditRoom(RoomDto room)
    {
        if (room.Code == null) return;

        Rooms[room.Code] = room;
        await RefreshPublicRoomsList();
        await _mediator.Publish(new RefreshRoomEvent(room));
    }
    public async Task SetStatus(string gameCode, int playerId, bool status)
    {
        RoomDto room = GetRoomByCode(gameCode);
        PlayerDto user = room.Users.FirstOrDefault(u => u.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode}");
        user.IsReady = status;
        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task StartGame(string gameCode, string connectionId)
    {
        RoomDto room = GetRoomByCode(gameCode);
        if (room.Questions.Count == 0) return;

        room.IsGameStarted = true;

        await _roundService.SetNewRound(room, connectionId);
        await RefreshPublicRoomsList();
    }

    public async Task RefreshPublicRoomsList()
    {
        IEnumerable<RoomDto> publicRooms = Rooms.Where(r => r.Value.IsPublic && !r.Value.IsGameStarted).Select(r => r.Value);
        await _mediator.Publish(new RefreshPublicRoomsListEvent(publicRooms));
    }

    public async Task LeaveRoom(string gameCode, int playerId)
    {
        RoomDto room = GetRoomByCode(gameCode);
        PlayerDto playerToRemove = room.Users.FirstOrDefault(p => p.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room '{gameCode}' (LeaveRoom)");
        room.Users.Remove(playerToRemove);

        await _mediator.Publish(new PlayerLeftRoomEvent(gameCode, playerToRemove));

        if (room.Users.Count == 0 || room.Owner.UserId == playerId)
        {
            await RemoveRoom(gameCode);
            return;
        }

        await RefreshPublicRoomsList();
    }

    public async Task AddQuestionsToGame(string gameCode, int playerId, List<QuestionDto> questions)
    {
        RoomDto room = GetRoomByCode(gameCode);
        if (room.Owner.UserId == playerId)
        {
            questions = [.. questions.Where(q => q.OwnerId == playerId || q.IsPublic)];
            room.Questions.RemoveAll(q => q.IsPublic);
        }

        room.Questions.RemoveAll(q => q.OwnerId == playerId);
        room.Questions.AddRange(questions);
        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task JoinRoom(string gameCode, PlayerDto player, string? password, int? ownerId)
    {
        string connectionId = GetPlayerConnectionId(player);
        if (FindPlayerByUserId(player.UserId) != null)
        {
            throw new InvalidOperationException($"Player {player.UserId} exist in this or another room! (JoinRoom)");
        }

        await VerifyRoomAccess(gameCode, connectionId, password, ownerId);
        RoomDto room = GetRoomByCode(gameCode);

        await RemoveAllRoomsByOwnerId(player.UserId);
        player.IsReady = false;
        room.Users.Add(player);

        await _mediator.Publish(new JoinRoomEvent(connectionId, gameCode));
        await _mediator.Publish(new RefreshRoomEvent(room));
        await RefreshPublicRoomsList();
    }

    private async Task RemoveAllRoomsByOwnerId(int ownerId)
    {
        var codes = Rooms.Values
            .Where(r => r.Owner.UserId == ownerId && r.Code is not null)
            .Select(r => r.Code!)
            .ToList();

        await Task.WhenAll(codes.Select(RemoveRoom));
    }

    private async Task RemoveRoom(string gameCode)
    {
        RoomDto room = GetRoomByCode(gameCode);
        Rooms.Remove(gameCode);
        await RefreshPublicRoomsList();
        await _mediator.Publish(new RoomClosedEvent(gameCode, [.. room.Users]));
    }

    private static PlayerDto? FindPlayerByUserId(int userId)
    {
        return Rooms
            .Where(room => room.Value != null)
            .SelectMany(room => room.Value.Users)
            .FirstOrDefault(player => player.UserId == userId);
    }

    private static string GetPlayerConnectionId(PlayerDto player)
    {
        return player.ConnectionId ?? throw new InvalidOperationException($"Player {player.UserId} doesnt have 'ConnectionId' (GetPlayerConnectionId)");
    }

    private async Task VerifyRoomAccess(string gameCode, string connectionId, string? password, int? ownerId)
    {
        RoomDto? selectedRoom = ownerId != null
        ? Rooms.Values.FirstOrDefault(r => (r.Owner.UserId == ownerId) && (r.Password == password || r.Password == null))
        : Rooms.GetValueOrDefault(gameCode);

        if (selectedRoom == null)
        {
            string fieldId = ownerId != null ? "password" : "code";
            string message = ownerId != null ? DictHelper.Validation.InvalidPassword : DictHelper.Validation.RoomDoesntExist;
            await _mediator.Publish(new ReceiveErrorEvent(connectionId, message, fieldId));
        }
    }

}
