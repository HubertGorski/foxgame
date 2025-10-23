using FoxTales.Application.DTOs.Psych;
using Microsoft.AspNetCore.SignalR;
using GameCode = System.String;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Api.Enums;

namespace FoxTales.Api.Hubs;

public class PsychHub(IRoomService roomService, IRoundService roundService, ILogger<PsychHub> logger) : Hub
{

    private readonly IRoomService _roomService = roomService;
    private readonly IRoundService _roundService = roundService;
    private readonly ILogger<PsychHub> _logger = logger;

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Connected: {ConnectionId} for user {UserIdentifier}", Context.ConnectionId, Context.UserIdentifier);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var (gameCode, player) = _roomService.FindPlayerByConnectionId(Context.ConnectionId);

        _logger.LogInformation("Disconnected: {ConnectionId}", Context.ConnectionId);
        if (gameCode == null || player == null)
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        _logger.LogInformation("Disconnected: {ConnectionId} for user {Player}, {GameCode}", Context.ConnectionId, player, gameCode);
        await _roomService.LeaveRoom(gameCode, player.UserId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task CreateRoom(RoomDto room)
    {
        room.Owner.ConnectionId = Context.ConnectionId;
        await _roomService.CreateRoom(room);
    }

    public async Task EditRoom(RoomDto room)
    {
        await _roomService.EditRoom(room);
    }

    public async Task SetStatus(string gameCode, int playerId, bool status)
    {
        await _roomService.SetStatus(gameCode, playerId, status);
    }

    public async Task StartGame(string gameCode)
    {
        await _roomService.StartGame(gameCode, Context.ConnectionId);
    }

    public async Task GoToJoinGameView()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, CommonGroup.JOIN_GAME_VIEW.ToString());
        await _roomService.RefreshPublicRoomsList();
    }

    public async Task LeaveRoom(string gameCode, int playerId)
    {
        await _roomService.LeaveRoom(gameCode, playerId);
    }

    public async Task AddQuestionsToGame(string gameCode, int playerId, List<QuestionDto> questions)
    {
        await _roomService.AddQuestionsToGame(gameCode, playerId, questions);
    }

    public async Task SetNewRound(string gameCode)
    {
        var room = _roomService.GetRoomByCode(gameCode);
        await _roundService.SetNewRound(room, Context.ConnectionId);
    }

    public async Task MarkAllUsersUnready(string gameCode)
    {
        var room = _roomService.GetRoomByCode(gameCode);
        await _roundService.MarkAllUsersUnreadyIfOwner(room, Context.ConnectionId);
    }

    public async Task ChooseAnswer(string gameCode, int playerId, int selectedAnswerUserId)
    {
        var room = _roomService.GetRoomByCode(gameCode);
        await _roundService.ChooseAnswer(room, playerId, selectedAnswerUserId);
    }

    public async Task AddAnswer(string gameCode, AnswerDto answer)
    {
        var room = _roomService.GetRoomByCode(gameCode);
        await _roundService.AddAnswer(room, answer);
    }

    public async Task JoinRoom(string gameCode, PlayerDto player, string? password, int? ownerId)
    {
        player.ConnectionId = Context.ConnectionId;
        await _roomService.JoinRoom(player, gameCode, password, ownerId);
    }

}
