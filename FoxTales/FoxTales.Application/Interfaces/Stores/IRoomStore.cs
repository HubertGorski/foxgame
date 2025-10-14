using System.Collections.Concurrent;
using FoxTales.Application.DTOs.Psych;

namespace FoxTales.Application.Interfaces.Stores;

public interface IRoomStore
{
    RoomDto GetRoomByCode(string gameCode);
    IEnumerable<RoomDto> GetAllRooms();
    ConcurrentDictionary<string, RoomDto> GetAllRoomsWithCodes();
    void SetRoom(string code, RoomDto room);
    void RemoveRoom(string code);
    bool RoomExists(string code);
    RoomDto? GetRoomOrDefault(string code);
}
