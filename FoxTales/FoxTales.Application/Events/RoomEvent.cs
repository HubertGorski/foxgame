using FoxTales.Application.DTOs.Psych;
using MediatR;
using GameCode = System.String;

namespace FoxTales.Application.Events;
public record RoomClosedEvent(GameCode Code, List<PlayerDto> PlayersInRoom) : INotification;
public record RefreshRoomEvent(RoomDto Room) : INotification;
public record RefreshPublicRoomsListEvent(IEnumerable<RoomDto> PublicRooms) : INotification;
public record PlayerLeftRoomEvent(GameCode Code, PlayerDto PlayerToRemove) : INotification;
public record ReceiveErrorEvent(string ConnectionId, string Message, string FieldId) : INotification;
public record JoinRoomEvent(string ConnectionId, GameCode Code) : INotification;
