
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Events;
using FoxTales.Application.Helpers;
using FoxTales.Application.Interfaces.Logics;
using FoxTales.Application.Interfaces.Psych;
using MediatR;

namespace FoxTales.Application.Services.Psych;

public class RoundService(IMediator mediator, IRoundLogic roundLogic) : IRoundService
{
    private readonly IMediator _mediator = mediator;
    private readonly IRoundLogic _roundLogic = roundLogic;

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

        QuestionDto question = _roundLogic.GetNewCurrentQuestionWithSelectedPlayer(room);
        if (question.CurrentUser == null) throw new InvalidOperationException($"Player not found in room {room.Code}. (SetNewRound)");

        room.Round += 1;
        room.Users.ForEach(u => u.VotersIdsForHisAnswer = []);
        room.CurrentQuestion = question;
        room.Questions.Remove(question);
        await MarkAllUsersUnreadyIfOwner(room, connectionId);
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

        _roundLogic.UpdateVotePool(voter, owner);

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
}
