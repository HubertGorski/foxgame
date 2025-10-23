using System.Collections.Concurrent;
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.Interfaces.Stores;

namespace FoxTales.Application.Services.Stores;

public class RoomStore : IRoomStore
{
    private readonly ConcurrentDictionary<string, RoomDto> _rooms = new();

    public RoomDto GetRoomByCode(string gameCode)
    {
        if (!_rooms.TryGetValue(gameCode, out RoomDto? room) || room == null)
            throw new InvalidOperationException($"Code '{gameCode}' does not exist");

        return room;
    }

    public IEnumerable<RoomDto> GetAllRooms()
    {
        return _rooms.Values;
    }

    public ConcurrentDictionary<string, RoomDto> GetAllRoomsWithCodes()
    {
        return _rooms;
    }

    public void SetRoom(string? code, RoomDto room)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Room code cannot be null or empty", nameof(code));

        ArgumentNullException.ThrowIfNull(room);
        _rooms[code] = room;
    }

    public void RemoveRoom(string code)
    {
        _rooms.TryRemove(code, out _);
    }

    public bool RoomExists(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        return _rooms.ContainsKey(code);
    }

    public RoomDto? GetRoomOrDefault(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return _rooms.TryGetValue(code, out var room) ? room : null;
    }

    public (string? RoomCode, PlayerDto? Player) FindPlayerByConnectionId(string connectionId)
    {
        foreach (var room in _rooms)
        {
            if (room.Value == null)
                return (null, null);

            var player = room.Value.Users.FirstOrDefault(p => p.ConnectionId == connectionId);

            if (player != null)
                return (room.Key, player);
        }

        return (null, null);
    }

    public (string? RoomCode, PlayerDto? Player) FindPlayerByUserId(int? userId)
    {
        foreach (var room in _rooms)
        {
            if (room.Value == null || userId == null)
                return (null, null);

            var player = room.Value.Users.FirstOrDefault(p => p.UserId == userId);

            if (player != null)
                return (room.Key, player);
        }

        return (null, null);
    }
}
