using FoxTales.Application.DTOs.Psych;
using GameCode = System.String;
using FoxTales.Application.Helpers;
using FoxTales.Application.Interfaces.Psych;
using MediatR;
using FoxTales.Application.Events;
using FoxTales.Application.DTOs.User;
using System.Collections.Concurrent;

namespace FoxTales.Application.Services.Psych;

public class RoomService(IMediator mediator, IRoundService roundService) : IRoomService
{
    private readonly IRoundService _roundService = roundService;
    private readonly IMediator _mediator = mediator;
    private static readonly ConcurrentDictionary<string, RoomDto> Rooms = new(); // TODO: usunac static i trzymac te informacje w zewnetrznej bazie.

    internal static void ClearRoomsForTest() => Rooms.Clear();
    internal static void AddRoomForTest(RoomDto room)
        => Rooms[string.IsNullOrWhiteSpace(room.Code) ? throw new InvalidOperationException("Room code cannot be null or empty") : room.Code] = room;
    internal static ConcurrentDictionary<string, RoomDto> GetRoomsForTest() => Rooms;

    public RoomDto GetRoomByCode(string gameCode)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null)
            throw new InvalidOperationException($"Code '{gameCode}' doesnt exist");

        return room;
    }

    public async Task CreateRoom(RoomDto room)
    {
        room.Code = RoomCodeGenerator.GenerateUniqueCode(code => !Rooms.ContainsKey(code));
        await RemoveAllRoomsByOwnerId(room.Owner.UserId); // TODO: dodac do logow bo to niepożądana sytuacja
        await RemoveUserFromAllRooms(room.Owner.UserId); // TODO: dodac do logow bo to niepożądana sytuacja

        room.Owner.IsReady = false;
        room.Users.Add(room.Owner);
        Rooms.TryAdd(room.Code, room);

        string connectionId = GetPlayerConnectionId(room.Owner);
        await _mediator.Publish(new JoinRoomEvent(connectionId, room.Code));
        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task EditRoom(RoomDto room)
    {
        if (room.Code == null)
            throw new InvalidOperationException($"Code '{room.Code}' is invalid (Edit Room)");

        if (room.Users.Count == 0)
            throw new InvalidOperationException("Room is empty! (Edit Room)");

        var _ = GetRoomByCode(room.Code);
        Rooms[room.Code] = room;

        if (room.IsPublic)
            await RefreshPublicRoomsList();

        await _mediator.Publish(new RefreshRoomEvent(room));
    }
    public async Task SetStatus(string gameCode, int playerId, bool status)
    {
        RoomDto room = GetRoomByCode(gameCode);
        PlayerDto user = room.Users.FirstOrDefault(u => u.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode} (SetStatus)");
        user.IsReady = status;
        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task StartGame(string gameCode, string connectionId)
    {
        RoomDto room = GetRoomByCode(gameCode);
        if (room.Questions.Count == 0) throw new InvalidOperationException($"Room '{gameCode}' doesnt have any questions! (StartGame)");

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

    public async Task JoinRoom(PlayerDto player, string? gameCode, string? password, int? ownerId)
    {
        await RemoveUserFromAllRooms(player.UserId);
        string connectionId = GetPlayerConnectionId(player);

        RoomDto? room = await VerifyRoomAccessGetRoom(connectionId, gameCode, password, ownerId);
        if (room == null) return;

        await RemoveAllRoomsByOwnerId(player.UserId);
        player.IsReady = false;
        room.Users.Add(player);

        if (room.Code == null)
            throw new InvalidOperationException($"Code doesnt exist! (JoinRoom)");

        await _mediator.Publish(new JoinRoomEvent(connectionId, room.Code));
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

    private async Task RemoveUserFromAllRooms(int userId)
    {
        var rooms = Rooms.Values
            .Where(r => r.Users.Any(u => u.UserId == userId) && r.Code is not null)
            .ToList();

        await Task.WhenAll(rooms.Select(r => RemoveUserFromRoom(r.Code!, userId)));
    }

    private async Task RemoveRoom(string gameCode)
    {
        RoomDto room = GetRoomByCode(gameCode);
        Rooms.TryRemove(gameCode, out var _);
        await RefreshPublicRoomsList();
        await _mediator.Publish(new RoomClosedEvent(gameCode, [.. room.Users]));
    }

    private async Task RemoveUserFromRoom(string gameCode, int userId)
    {
        RoomDto room = GetRoomByCode(gameCode);
        var removedUsers = room.Users.Where(u => u.UserId == userId).ToList();
        foreach (var user in removedUsers)
        {
            room.Users.Remove(user);
        }

        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    private static string GetPlayerConnectionId(PlayerDto player)
    {
        return player.ConnectionId ?? throw new InvalidOperationException($"Player {player.UserId} doesnt have 'ConnectionId' (GetPlayerConnectionId)");
    }

    private async Task<RoomDto?> VerifyRoomAccessGetRoom(string connectionId, string? gameCode, string? password, int? ownerId)
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

        return selectedRoom;
    }

}
