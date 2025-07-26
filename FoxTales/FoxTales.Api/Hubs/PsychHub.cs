using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.Helpers;
using Microsoft.AspNetCore.SignalR;
using GameCode = System.String;

namespace FoxTales.Api.Hubs;

public class PsychHub : Hub
{
    private static readonly Dictionary<GameCode, RoomDto> Rooms = [];

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
        await Clients.Group(room.Code).SendAsync("GetGameCode", room.Code);
    }

    public async Task JoinRoom(string gameCode, PlayerDto player)
    {
        await RemoveAllRoomsByOwnerId(player.UserId);
        if (FindPlayerByUserId(player.UserId) != (null, null))
        {
            await Clients.Caller.SendAsync("ReceiveError", DictHelper.Validation.YouAreAlreadyAuthenticated);
            return;
        }

        await Clients.Client(Context.ConnectionId).SendAsync("LoadRoom", Rooms.GetValueOrDefault(gameCode));
        await AddPlayerToRoom(gameCode, player);
    }

    public async Task EditRoom(RoomDto room)
    {
        if (room.Code == null) return;

        Rooms[room.Code] = room;
        await Clients.Group(room.Code).SendAsync("LoadRoom", room);
    }

    private async Task AddPlayerToRoom(string gameCode, PlayerDto player)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null)
        {
            await Clients.Caller.SendAsync("ReceiveError", DictHelper.Validation.RoomDoesntExist);
            return;
        }

        player.ConnectionId = Context.ConnectionId;
        room.Users.Add(player);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        await Clients.Group(gameCode).SendAsync("GetPlayers", GetPlayers(gameCode));

    }

    public async Task LeaveRoom(string gameCode, int playerId)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null)
        {
            await Clients.Caller.SendAsync("ReceiveError", DictHelper.Validation.RoomDoesntExist);
            return;
        }

        PlayerDto playerToRemove = room.Users.FirstOrDefault(p => p.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode}");
        room.Users.Remove(playerToRemove);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameCode);
        await Clients.Group(gameCode).SendAsync("PlayerLeft", playerId);

        if (room.Users.Count == 0 || room.Owner.UserId == playerId)
        {
            await RemoveRoom(gameCode);
        }
    }

    public async Task RemoveRoom(string gameCode)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null)
        {
            await Clients.Caller.SendAsync("ReceiveError", DictHelper.Validation.RoomDoesntExist);
            return;
        }

        await Clients.Group(gameCode).SendAsync("RoomClosed");
        foreach (var player in room.Users.ToList())
        {
            await Groups.RemoveFromGroupAsync(player.ConnectionId, gameCode);
        }

        Rooms.Remove(gameCode);
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
