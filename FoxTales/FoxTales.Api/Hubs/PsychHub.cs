using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.Helpers;
using Microsoft.AspNetCore.SignalR;
using GameCode = System.String;

namespace FoxTales.Api.Hubs;

public class PsychHub : Hub
{
    private static readonly Dictionary<GameCode, List<PlayerDto>> Rooms = [];

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

    public async Task JoinRoom(string gameCode, PlayerDto player)
    {
        if (FindPlayerByUserId(player.UserId) != (null, null))
        {
            await Clients.Caller.SendAsync("ReceiveError", DictHelper.Validation.YouAreAlreadyAuthenticated);
            return;
        }

        if (!Rooms.ContainsKey(gameCode)) Rooms[gameCode] = [];
        player.ConnectionId = Context.ConnectionId;
        Rooms[gameCode].Add(player);

        await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        await Clients.Group(gameCode).SendAsync("GetPlayers", GetPlayers(gameCode));
    }

    public async Task LeaveRoom(string gameCode, int playerId)
    {
        if (!Rooms.TryGetValue(gameCode, out var roomPlayers))
            throw new KeyNotFoundException($"Room with code {gameCode} not found");

        var playerToRemove = roomPlayers.FirstOrDefault(p => p.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode}");
        roomPlayers.Remove(playerToRemove);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameCode);
        await Clients.Group(gameCode).SendAsync("PlayerLeft", playerId);

        if (roomPlayers.Count == 0)
        {
            Rooms.Remove(gameCode);
        }
    }

    private static List<PlayerDto> GetPlayers(string gameCode)
    {
        return Rooms.GetValueOrDefault(gameCode)?.ToList() ?? [];
    }

    private static (string? Room, PlayerDto? Player) FindPlayerByConnectionId(string connectionId)
    {
        foreach (var room in Rooms)
        {
            var player = room.Value.FirstOrDefault(p => p.ConnectionId == connectionId);
            if (player != null)
                return (room.Key, player);
        }
        return (null, null);
    }

    private static (string? Room, PlayerDto? Player) FindPlayerByUserId(int userId)
    {
        foreach (var room in Rooms)
        {
            var player = room.Value.FirstOrDefault(p => p.UserId == userId);
            if (player != null)
                return (room.Key, player);
        }
        return (null, null);
    }
}
