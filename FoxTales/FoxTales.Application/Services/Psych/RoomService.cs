using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.Helpers;
using FoxTales.Application.Interfaces.Psych;
using MediatR;
using FoxTales.Application.Events;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces.Stores;
using FoxTales.Application.Interfaces.Logics;

namespace FoxTales.Application.Services.Psych;

public class RoomService(IMediator mediator, IRoundService roundService, IRoomStore roomStore, IPsychLibraryService psychLibraryService, IRoomLogic roomLogic) : IRoomService
{
    private readonly IRoundService _roundService = roundService;
    private readonly IMediator _mediator = mediator;
    private readonly IRoomStore _roomStore = roomStore;
    private readonly IRoomLogic _roomLogic = roomLogic;
    private readonly IPsychLibraryService _psychLibraryService = psychLibraryService;

    public RoomDto GetRoomByCode(string gameCode)
    {
        return _roomStore.GetRoomByCode(gameCode);
    }

    public async Task CreateRoom(RoomDto room)
    {
        room.Code = RoomCodeGenerator.GenerateUniqueCode(code => !_roomStore.RoomExists(code));
        await RemoveAllRoomsByOwnerId(room.Owner.UserId);
        await RemoveUserFromAllRooms(room.Owner.UserId);

        room.Owner.IsReady = false;
        room.Users.Add(room.Owner);
        _roomStore.SetRoom(room.Code, room);

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

        var currentRoom = GetRoomByCode(room.Code);
        if (room.Users.Count != currentRoom.Users.Count)
            throw new InvalidOperationException($"The number of players is not correct in '{room.Code}' room! (Edit Room)");

        _roomStore.SetRoom(room.Code, room);

        if (room.IsPublic)
            await RefreshPublicRoomsList();

        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task SetStatus(string gameCode, int playerId, bool status)
    {
        RoomDto room = GetRoomByCode(gameCode);
        PlayerDto user = room.Users.FirstOrDefault(u => u.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode} (SetStatus)");
        if (user.IsReady == status)
            return;

        user.IsReady = status;
        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task StartGame(string gameCode, string connectionId)
    {
        RoomDto room = GetRoomByCode(gameCode);

        await AddPublicQuestionsToGame(room);

        if (room.Questions.Count == 0) throw new InvalidOperationException($"Room '{gameCode}' does not have any questions! (StartGame)");

        _roomLogic.IsTeamSetupValid(room); // TODO: dokonczyc

        room.IsGameStarted = true;

        await _roundService.SetNewRound(room, connectionId);
        await RefreshPublicRoomsList();
    }

    public async Task RefreshPublicRoomsList()
    {
        IEnumerable<RoomDto> publicRooms = _roomStore.GetAllRooms().Where(r => r.IsPublic && !r.IsGameStarted);
        await _mediator.Publish(new RefreshPublicRoomsListEvent(publicRooms));
    }

    public async Task SuspendUserInRoom(string gameCode, int playerId)
    {
        RoomDto room = GetRoomByCode(gameCode);
        PlayerDto playerToRemove = room.Users.FirstOrDefault(p => p.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room '{gameCode}'");
        playerToRemove.ConnectionId = null;
        await _mediator.Publish(new RefreshRoomEvent(room));
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

    public async Task AddPrivateQuestionsToGame(string gameCode, int playerId, List<QuestionDto> privateQuestions)
    {
        RoomDto room = GetRoomByCode(gameCode);

        room.Questions.RemoveAll(q => q.OwnerId == playerId);
        room.Questions.AddRange(privateQuestions);

        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task AddPublicQuestionsToGame(RoomDto room)
    {
        if (room.UsePublicQuestions && !room.Questions.Any(q => q.IsPublic))
        {
            ICollection<QuestionDto> publicQuestions = await _psychLibraryService.GetPublicQuestionsByCatalogId(room.SelectedPublicCatalogId ?? 0);
            room.Questions.AddRange(publicQuestions);
        }

        if (!room.UsePublicQuestions && room.Questions.Any(q => q.IsPublic))
        {
            room.Questions.RemoveAll(q => q.IsPublic);
        }

        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task JoinRoom(PlayerDto player, string? gameCode, string? password, int? ownerId)
    {
        string connectionId = GetPlayerConnectionId(player);
        RoomDto? room = await VerifyRoomAccessGetRoom(connectionId, gameCode, password, ownerId);
        if (room == null || room.Code == null) return;

        await RemoveAllRoomsByOwnerId(player.UserId);
        await RemoveUserFromAllRooms(player.UserId);

        player.IsReady = false;
        room.Users.Add(player);

        await _mediator.Publish(new JoinRoomEvent(connectionId, room.Code));
        await _mediator.Publish(new RefreshRoomEvent(room));
        await RefreshPublicRoomsList();
    }

    public (string? RoomCode, PlayerDto? Player) FindPlayerByConnectionId(string connectionId)
    {
        return _roomStore.FindPlayerByConnectionId(connectionId);
    }

    public async Task ContinuePlaying(int? userId, string connectionId)
    {
        var (gameCode, player) = _roomStore.FindPlayerByUserId(userId);
        if (gameCode == null || player == null)
            return;

        player.ConnectionId = connectionId;
        await _mediator.Publish(new JoinRoomEvent(connectionId, gameCode));
    }

    private async Task RemoveAllRoomsByOwnerId(int ownerId)
    {
        var codes = _roomStore.GetAllRooms()
            .Where(r => r.Owner.UserId == ownerId && r.Code is not null)
            .Select(r => r.Code!)
            .ToList();

        await Task.WhenAll(codes.Select(RemoveRoom));
    }

    private async Task RemoveUserFromAllRooms(int userId)
    {
        var rooms = _roomStore.GetAllRooms()
            .Where(r => r.Users.Any(u => u.UserId == userId) && r.Code is not null)
            .ToList();

        await Task.WhenAll(rooms.Select(r => RemoveUserFromRoom(r.Code!, userId)));
    }

    private async Task RemoveRoom(string gameCode)
    {
        RoomDto room = GetRoomByCode(gameCode);
        _roomStore.RemoveRoom(gameCode);
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
        return player.ConnectionId ?? throw new InvalidOperationException($"Player {player.UserId} does not have 'ConnectionId' (GetPlayerConnectionId)");
    }

    private async Task<RoomDto?> VerifyRoomAccessGetRoom(string connectionId, string? gameCode, string? password, int? ownerId)
    {
        RoomDto? selectedRoom = ownerId != null
            ? _roomStore.GetAllRooms().FirstOrDefault(r => (r.Owner.UserId == ownerId) && (r.Password == password || r.Password == null))
            : _roomStore.GetRoomOrDefault(gameCode ?? "");

        if (selectedRoom == null)
        {
            string fieldId = ownerId != null ? "password" : "code";
            string message = ownerId != null ? DictHelper.Validation.InvalidPassword : DictHelper.Validation.RoomDoesntExist;
            await _mediator.Publish(new ReceiveErrorEvent(connectionId, message, fieldId));
        }

        return selectedRoom;
    }

}
