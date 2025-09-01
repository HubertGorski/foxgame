using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.Helpers;
using Microsoft.AspNetCore.SignalR;
using GameCode = System.String;
using CommonGroup = System.String;
using FoxTales.Application.DTOs.User;

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
        await RefreshRoom(room);
        await RefreshPublicRoomsList();
    }

    public async Task SetStatus(string gameCode, int playerId, bool status)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;
        PlayerDto user = room.Users.FirstOrDefault(u => u.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode}");
        user.IsReady = status;
        await RefreshRoom(room);
    }

    private async Task AddPlayerToRoom(string gameCode, PlayerDto player)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;

        player.ConnectionId = Context.ConnectionId;
        player.IsReady = false;
        room.Users.Add(player);

        await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, JOIN_GAME_VIEW);
        await Clients.Group(gameCode).SendAsync("GetPlayers", GetPlayers(gameCode));
        await RefreshPublicRoomsList();
    }

    public async Task RefreshPublicRoomsList()
    {
        await Clients.Group(JOIN_GAME_VIEW).SendAsync("GetPublicRooms", Rooms.Where(r => r.Value.IsPublic && !r.Value.IsGameStarted).Select(r => r.Value));
    }

    private async Task RefreshRoom(RoomDto room)
    {
        if (room.Code == null) throw new InvalidOperationException($"Code is invalid");
        await Clients.Group(room.Code).SendAsync("LoadRoom", room);
    }

    public async Task AddQuestionsToGame(string gameCode, int playerId, List<QuestionDto> questions)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;
        if (room.Owner.UserId == playerId)
            room.Questions.RemoveAll(q => q.IsPublic);

        room.Questions.RemoveAll(q => q.OwnerId == playerId);
        room.Questions.AddRange(questions);
        await RefreshRoom(room);
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

        await RefreshPublicRoomsList();
    }

    public async Task GoToJoinGameView()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, JOIN_GAME_VIEW);
        await Clients.Client(Context.ConnectionId).SendAsync("GetPublicRooms", Rooms.Where(r => r.Value.IsPublic && !r.Value.IsGameStarted).Select(r => r.Value));
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
        await RefreshPublicRoomsList();
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

    public async Task StartGame(string gameCode)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null || room.Questions.Count == 0) return;

        room.IsGameStarted = true;

        await SetNewRound(gameCode);
        await RefreshPublicRoomsList();
    }
    public async Task MarkAllUsersUnready(string gameCode)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;
        var player = room.Users.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
        if (player == null || room.Owner.UserId != player.UserId) return;

        room.Users.ForEach(u => u.IsReady = false);
        await RefreshRoom(room);
    }

    public async Task AddAnswer(string gameCode, AnswerDto answer)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;
        PlayerDto user = room.Users.FirstOrDefault(u => u.UserId == answer.OwnerId) ?? throw new InvalidOperationException($"Player {answer.OwnerId} not found in room {gameCode}");

        user.IsReady = true;
        user.Answer = answer;

        await RefreshRoom(room);
    }

    public async Task ChooseAnswer(string gameCode, int playerId, int selectedAnswerUserId)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;
        PlayerDto voter = room.Users.FirstOrDefault(u => u.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {gameCode}");
        PlayerDto owner = room.Users.FirstOrDefault(u => u.UserId == selectedAnswerUserId) ?? throw new InvalidOperationException($"Player {selectedAnswerUserId} not found in room {gameCode}");

        if (owner.VotersIdsForHisAnswer.Contains(voter.UserId) || owner.Answer == null) return;

        UpdateVotePool(voter, owner);

        voter.IsReady = true;
        owner.PointsInGame += 10; //TODO: zrobic sensowniejszy przydzial punktow

        await RefreshRoom(room);
    }

    public static void UpdateVotePool(PlayerDto voter, PlayerDto owner)
    {
        owner.Answer.VotersCount++;
        if (!owner.VotersIdsForHisAnswer.Contains(voter.UserId))
            owner.VotersIdsForHisAnswer.Add(voter.UserId);

        var voterIndex = owner.VotesReceived.FindIndex(kv => kv.Key == voter.UserId);

        if (voterIndex >= 0)
            owner.VotesReceived[voterIndex] = new KeyValuePair<int, int>(voter.UserId, owner.VotesReceived[voterIndex].Value + 1);
        else
            owner.VotesReceived.Add(new KeyValuePair<int, int>(voter.UserId, 1));

        var index = voter.VotesGiven.FindIndex(kv => kv.Key == owner.UserId);

        if (index >= 0)
            voter.VotesGiven[index] = new KeyValuePair<int, int>(owner.UserId, voter.VotesGiven[index].Value + 1);
        else
            voter.VotesGiven.Add(new KeyValuePair<int, int>(owner.UserId, 1));
    }

    public async Task SetNewRound(string gameCode)
    {
        if (!Rooms.TryGetValue(gameCode, out RoomDto? room) || room == null) return;
        var player = room.Users.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
        if (player == null || room.Owner.UserId != player.UserId) return;

        if (room.Questions.Count == 0)
        {
            room.HasGameEnded = true;
            await RefreshRoom(room);
            return;
        }

        room.Round += 1;
        room.Users.ForEach(u => u.IsReady = false);
        room.Users.ForEach(u => u.VotersIdsForHisAnswer = []);

        QuestionDto question = GetNewCurrentQuestionWithSelectedPlayer(room);
        if (question.CurrentUser == null) throw new InvalidOperationException($"Player not found in room {gameCode}");

        room.CurrentQuestion = question;
        room.Questions.Remove(question);

        await RefreshRoom(room);
    }

    public static QuestionDto GetNewCurrentQuestionWithSelectedPlayer(RoomDto room)
    {
        Random rnd = new();
        QuestionDto currentQuestion = room.Questions[rnd.Next(room.Questions.Count)];

        var minSelectionCount = room.Users.Min(u => u.SelectionCount);

        var eligibleUsers = room.Users
            .Where(u => u.SelectionCount == minSelectionCount)
            .ToList();

        PlayerDto selectedPlayer = eligibleUsers[rnd.Next(eligibleUsers.Count)];

        currentQuestion.CurrentUser = selectedPlayer;
        currentQuestion.Text = currentQuestion.Text.Replace("****", selectedPlayer.Username);
        selectedPlayer.SelectionCount += 1;

        return currentQuestion;
    }
}
