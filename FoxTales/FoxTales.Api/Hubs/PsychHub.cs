using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.Helpers;
using Microsoft.AspNetCore.SignalR;
using GameCode = System.String;
using CommonGroup = System.String;

namespace FoxTales.Api.Hubs;

public class PsychHub : Hub
{
    private static readonly Dictionary<GameCode, RoomDto> Rooms = [];

    private static readonly CommonGroup JOIN_GAME_VIEW = "JOIN_GAME_VIEW";

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var (gameCode, player) = FindPlayerByConnectionId(Context.ConnectionId);
        if (gameCode == null || player == null)
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        await LeaveRoom(gameCode, player.UserId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task CreateRoom(RoomDto room)
    {
        await RemoveAllRoomsByOwnerId(room.Owner.UserId);
        room.Code = RoomCodeGenerator.GenerateUniqueCode(code => !Rooms.ContainsKey(code));
        Rooms.Add(room.Code, room);
        await AddPlayerToRoom(room.Code, room.Owner);
        await Clients.Client(Context.ConnectionId).SendAsync("GetGameCode", room.Code);
    }

    public async Task JoinRoom(string gameCode, PlayerDto player, string password, int? ownerId)
    {
        await RemoveAllRoomsByOwnerId(player.UserId);
        if (FindPlayerByUserId(player.UserId) != (null, null))
        {
            await Clients.Caller.SendAsync("ReceiveError", new { message = DictHelper.Validation.YouAreAlreadyAuthenticated, fieldId = "code" });
            return;
        }

        RoomDto? selectedRoom = ownerId != null
        ? Rooms.Values.FirstOrDefault(r => (r.Owner.UserId == ownerId) && (r.Password == password || r.Password == null))
        : Rooms.GetValueOrDefault(gameCode);

        if (selectedRoom == null)
        {
            string fieldId = ownerId != null ? "password" : "code";
            string message = ownerId != null ? DictHelper.Validation.InvalidPassword : DictHelper.Validation.RoomDoesntExist;
            await Clients.Caller.SendAsync("ReceiveError", new { message, fieldId });
            return;
        }

        if (selectedRoom.Code == null) return;
        await Clients.Client(Context.ConnectionId).SendAsync("LoadRoom", selectedRoom);
        await AddPlayerToRoom(selectedRoom.Code, player);
    }

    public async Task EditRoom(RoomDto room)
    {
        if (room.Code == null) return;

        Rooms[room.Code] = room;
        await Clients.Group(room.Code).SendAsync("LoadRoom", room);
        await Clients.Group(JOIN_GAME_VIEW).SendAsync("GetPublicRooms", Rooms.Where(r => r.Value.IsPublic).Select(r => r.Value));
    }

    public async Task SetStatus(string gameCode, int playerId, bool status)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;
        PlayerDto user = room.Users.FirstOrDefault(u => u.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode}");
        user.IsReady = status;
        await Clients.Group(gameCode).SendAsync("LoadRoom", room);
    }

    private async Task AddPlayerToRoom(string gameCode, PlayerDto player)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;

        player.ConnectionId = Context.ConnectionId;
        room.Users.Add(player);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, JOIN_GAME_VIEW);
        await Clients.Group(gameCode).SendAsync("GetPlayers", GetPlayers(gameCode));
        await Clients.Group(JOIN_GAME_VIEW).SendAsync("GetPublicRooms", Rooms.Where(r => r.Value.IsPublic).Select(r => r.Value));
    }

    public async Task LeaveRoom(string gameCode, int playerId)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;

        PlayerDto playerToRemove = room.Users.FirstOrDefault(p => p.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode}");
        room.Users.Remove(playerToRemove);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameCode);
        await Clients.Group(gameCode).SendAsync("PlayerLeft", playerId);

        if (room.Users.Count == 0 || room.Owner.UserId == playerId)
        {
            await RemoveRoom(gameCode);
        }
        await Clients.Group(JOIN_GAME_VIEW).SendAsync("GetPublicRooms", Rooms.Where(r => r.Value.IsPublic).Select(r => r.Value));
    }

    public async Task GoToJoinGameView()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, JOIN_GAME_VIEW);
        await Clients.Client(Context.ConnectionId).SendAsync("GetPublicRooms", Rooms.Where(r => r.Value.IsPublic).Select(r => r.Value));
    }

    public async Task RemoveRoom(string gameCode)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;

        await Clients.Group(gameCode).SendAsync("RoomClosed");
        foreach (var player in room.Users.ToList())
        {
            await Groups.RemoveFromGroupAsync(player.ConnectionId, gameCode);
        }

        Rooms.Remove(gameCode);
        await Clients.Group(JOIN_GAME_VIEW).SendAsync("GetPublicRooms", Rooms.Where(r => r.Value.IsPublic).Select(r => r.Value));
    }

    private static List<PlayerDto> GetPlayers(string gameCode)
    {
        return Rooms.GetValueOrDefault(gameCode)?.Users?.ToList() ?? [];
    }

    private static (string? Room, PlayerDto? Player) FindPlayerByConnectionId(string connectionId)
    {
        foreach (var room in Rooms)
        {
            if (room.Value == null) return (null, null);
            var player = room.Value.Users.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (player != null) return (room.Key, player);
        }

        return (null, null);
    }

    private static (string? Room, PlayerDto? Player) FindPlayerByUserId(int userId)
    {
        foreach (var room in Rooms)
        {
            if (room.Value == null) return (null, null);
            var player = room.Value.Users.FirstOrDefault(p => p.UserId == userId);
            if (player != null) return (room.Key, player);
        }

        return (null, null);
    }

    public async Task RemoveAllRoomsByOwnerId(int ownerId)
    {
        var codes = Rooms.Values
            .Where(r => r.Owner.UserId == ownerId && r.Code is not null)
            .Select(r => r.Code!)
            .ToList();

        await Task.WhenAll(codes.Select(RemoveRoom));
    }
}
