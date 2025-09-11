
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Events;
using FoxTales.Application.Interfaces.Psych;
using MediatR;

namespace FoxTales.Application.Services.Psych;

public class RoundService(IMediator mediator) : IRoundService
{
    private readonly IMediator _mediator = mediator;

    public async Task SetNewRound(RoomDto room, string connectionId)
    {
        var player = room.Users.FirstOrDefault(p => p.ConnectionId == connectionId);
        if (player == null || room.Owner.UserId != player.UserId) return;

        if (room.Questions.Count == 0)
        {
            room.HasGameEnded = true;
            await _mediator.Publish(new RefreshRoomEvent(room));
            return;
        }

        room.Round += 1;
        await MarkAllUsersUnreadyIfOwner(room, connectionId);
        room.Users.ForEach(u => u.VotersIdsForHisAnswer = []);

        QuestionDto question = GetNewCurrentQuestionWithSelectedPlayer(room);
        if (question.CurrentUser == null) throw new InvalidOperationException($"Player not found in room {room.Code}. (SetNewRound)");

        room.CurrentQuestion = question;
        room.Questions.Remove(question);

        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task MarkAllUsersUnreadyIfOwner(RoomDto room, string connectionId)
    {
        var player = room.Users.FirstOrDefault(p => p.ConnectionId == connectionId);
        if (player == null || room.Owner.UserId != player.UserId) return;

        room.Users.ForEach(u => u.IsReady = false);
        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task ChooseAnswer(RoomDto room, int playerId, int selectedAnswerUserId)
    {
        PlayerDto voter = room.Users.FirstOrDefault(u => u.UserId == playerId) ?? throw new InvalidOperationException($"Player {playerId} not found in room {room.Code}. (ChooseAnswer)");
        PlayerDto owner = room.Users.FirstOrDefault(u => u.UserId == selectedAnswerUserId) ?? throw new InvalidOperationException($"Player {selectedAnswerUserId} not found in room {room.Code}. (ChooseAnswer)");

        if (owner.VotersIdsForHisAnswer.Contains(voter.UserId) || owner.Answer == null) return;

        UpdateVotePool(voter, owner);

        voter.IsReady = true;
        owner.PointsInGame += 10; //TODO: zrobic sensowniejszy przydzial punktow

        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    public async Task AddAnswer(RoomDto room, AnswerDto answer)
    {
        PlayerDto user = room.Users.FirstOrDefault(u => u.UserId == answer.OwnerId) ?? throw new InvalidOperationException($"Player {answer.OwnerId} not found in room {room.Code} (AddAnswer)");

        user.IsReady = true;
        user.Answer = answer;

        await _mediator.Publish(new RefreshRoomEvent(room));
    }

    private static QuestionDto GetNewCurrentQuestionWithSelectedPlayer(RoomDto room)
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

    private static void UpdateVotePool(PlayerDto voter, PlayerDto owner)
    {
        if (owner.Answer == null)
            throw new InvalidOperationException($"Player {owner.UserId} not provide answer. (UpdateVotePool)");

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
}
