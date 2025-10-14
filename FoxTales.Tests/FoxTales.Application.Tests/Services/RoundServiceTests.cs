using FluentAssertions;
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Events;
using FoxTales.Application.Interfaces.Logics;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Application.Services.Psych;
using FoxTales.Application.Tests.Common;
using MediatR;
using Moq;

namespace FoxTales.Application.Tests.Services;

public class RoundServiceTests : BaseTest
{

    private readonly IRoundService _service;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRoundLogic> _roundLogicMock;

    public RoundServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _roundLogicMock = new Mock<IRoundLogic>();
        _service = new RoundService(_mediatorMock.Object, _roundLogicMock.Object);
    }

    [Fact]
    public async Task SetNewRound_ShouldGameEnded_WhenQuestionsPoolIsEmpty()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();

        // WHEN
        await _service.SetNewRound(room, OwnerConnectionId);

        // THEN
        room.HasGameEnded.Should().BeTrue("the game should be ended when the questions pool is empty");

        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once);
    }

    [Fact]
    public async Task SetNewRound_ShouldThrow_WhenCurrentUserIsNull()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();
        room.Questions = [CreateTestQuestion()];
        _roundLogicMock.Setup(r => r.GetNewCurrentQuestionWithSelectedPlayer(room)).Returns(CreateTestQuestion());

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.SetNewRound(room, OwnerConnectionId));

        // THEN
        ex.Message.Should().Contain($"Player not found in room {room.Code}. (SetNewRound)");
    }

    [Fact]
    public async Task SetNewRound_ShouldThrow_WhenPlayersNotReady()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        RoomDto room = CreateTestRoom();
        room.Users = [owner, user];

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.SetNewRound(room, OwnerConnectionId));

        // THEN
        ex.Message.Should().Contain($"Players are not ready in the room {room.Code}! (SetNewRound)");
    }

    [Fact]
    public async Task SetNewRound_ShouldStartNewRound_WhenQuestionsAreAvailable_AndOwnerInitiatesStart()
    {
        // GIVEN
        // Create users
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);

        // All users are ready
        user.IsReady = true;
        user_2.IsReady = true;

        // After voting, users have the votes of other players
        owner.VotersIdsForHisAnswer = [UserId_2];
        user.VotersIdsForHisAnswer = [OwnerId];
        user_2.VotersIdsForHisAnswer = [UserId];

        // Create room with users and questions
        RoomDto room = CreateTestRoom();
        room.Users = [owner, user, user_2];
        room.Questions = [Library["ownerQuestion"], Library["userQuestion"]];

        // Simulation of choosing a new question for the next round and assigning a player to it
        QuestionDto newQuestion = Library["userQuestion"];
        newQuestion.CurrentUser = user_2;
        _roundLogicMock.Setup(r => r.GetNewCurrentQuestionWithSelectedPlayer(room)).Returns(newQuestion);

        // WHEN
        await _service.SetNewRound(room, OwnerConnectionId);

        // THEN
        room.HasGameEnded.Should().BeFalse("the game should not be ended after starting a new round");

        room.Round.Should().Be(1, "the round should be set to 1");

        room.Users.Should().OnlyContain(u => u.VotersIdsForHisAnswer.Count == 0, "no player should have any votes for their answer");

        room.Users.Should().OnlyContain(u => !u.IsReady, "no player should be marked as ready after starting a new round");

        room.CurrentQuestion.Should().Be(newQuestion, "the current question should be the owner's question");

        room.Questions.Should().NotContain(newQuestion, "the owner's question should be removed from the pool");

        _roundLogicMock.Verify(r => r.GetNewCurrentQuestionWithSelectedPlayer(room), Times.Once);

        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once);
    }

    [Fact]
    public async Task SetNewRound_ShouldNotStartNewRound_WhenNonOwnerTriesToStart_AndQuestionsAreAvailable()
    {
        // GIVEN
        // Create users
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);

        // All users are ready
        owner.IsReady = true;
        user.IsReady = true;
        user_2.IsReady = true;

        // After voting, users have the votes of other players
        owner.VotersIdsForHisAnswer = [UserId_2];
        user.VotersIdsForHisAnswer = [OwnerId];
        user_2.VotersIdsForHisAnswer = [UserId];

        // Create room with users and questions
        RoomDto room = CreateTestRoom();
        room.Users = [owner, user, user_2];
        room.Questions = [Library["ownerQuestion"]];

        // WHEN
        await _service.SetNewRound(room, UserConnectionId);

        // THEN
        room.HasGameEnded.Should().BeFalse("the game should not end when a non-owner tries to start a new round");

        room.Round.Should().Be(0, "the round number should not change when a non-owner initiates the round");

        room.Users.Should().OnlyContain(u => u.VotersIdsForHisAnswer.Count == 1, "votes for answers should remain unchanged when a non-owner attempts to start the round");

        room.Users.Should().OnlyContain(u => u.IsReady, "all players should remain marked as ready because the round was not started");

        room.CurrentQuestion.Should().BeNull("no new question should be assigned when a non-owner tries to start the round");

        room.Questions.Should().Contain(Library["ownerQuestion"], "the question pool should remain unchanged when a non-owner tries to start the round");

        _roundLogicMock.Verify(r => r.GetNewCurrentQuestionWithSelectedPlayer(room), Times.Never);

        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Never);
    }

    [Fact]
    public async Task MarkAllUsersUnreadyIfOwner_ShouldDoNothing_WhenPlayerNotFound()
    {
        // GIVEN
        // Create users
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);

        // All users are ready
        owner.IsReady = true;
        user.IsReady = true;

        // Create room
        RoomDto room = CreateTestRoom();
        room.Users = [owner, user];

        // WHEN
        await _service.MarkAllUsersUnreadyIfOwner(room, UserConnectionId_2);

        // THEN
        room.Users.Should().OnlyContain(u => u.IsReady, "the player was not found, so no user's ready state should change");

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task MarkAllUsersUnreadyIfOwner_ShouldDoNothing_WhenPlayerIsNotOwner()
    {
        // GIVEN
        // Create users
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);

        // All users are ready
        owner.IsReady = true;
        user.IsReady = true;

        // Create room
        RoomDto room = CreateTestRoom();
        room.Users = [owner, user];

        // WHEN
        await _service.MarkAllUsersUnreadyIfOwner(room, UserConnectionId);

        // THEN
        room.Users.Should().OnlyContain(u => u.IsReady, "the player is not the owner, so no user's ready state should change");

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task MarkAllUsersUnreadyIfOwner_ShouldSetAllUsersUnreadyAndPublishEvent_WhenPlayerIsOwner()
    {
        // GIVEN
        // Create users
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);

        // All users are ready
        owner.IsReady = true;
        user.IsReady = true;

        // Create room
        RoomDto room = CreateTestRoom();
        room.Users = [owner, user];

        // WHEN
        await _service.MarkAllUsersUnreadyIfOwner(room, OwnerConnectionId);

        // THEN
        room.Users.Should().OnlyContain(u => !u.IsReady, "the player is the owner, so all users should be set to unready");

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task AddAnswer_SetsUserAnswerAndPublishesEvent_WhenCalled()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();
        AnswerDto answer = CreateTestAnswer(OwnerId);

        // WHEN
        await _service.AddAnswer(room, answer);

        // THEN
        PlayerDto updatedUser = room.Users.First(u => u.UserId == OwnerId);

        updatedUser.IsReady.Should().BeTrue(because: "when a user submits an answer they should be marked as ready");

        updatedUser.Answer.Should().BeEquivalentTo(answer, because: "the submitted answer should be stored on the user");

        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once, "adding an answer should trigger a room refresh event");
    }

    [Fact]
    public async Task AddAnswer_ShouldThrow_WhenUserDoesNotExistInRoom()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();
        AnswerDto answer = CreateTestAnswer(UserId);

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.AddAnswer(room, answer));

        // THEN
        ex.Message.Should().Contain($"Player {answer.OwnerId} not found in room {room.Code} (AddAnswer)");
    }

    [Fact]
    public async Task AddAnswer_ShouldThrow_WhenAnswerHasNoOwner()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();
        AnswerDto answer = CreateTestAnswer();

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.AddAnswer(room, answer));

        // THEN
        ex.Message.Should().Contain($"Player {answer.OwnerId} not found in room {room.Code} (AddAnswer)");
    }

    [Fact]
    public async Task ChooseAnswer_ShouldThrow_WhenAnswerOwnerDoesNotExist()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.ChooseAnswer(room, OwnerId, UserId));

        // THEN
        ex.Message.Should().Contain($"Player {UserId} not found in room {room.Code}. (ChooseAnswer)");
    }

    [Fact]
    public async Task ChooseAnswer_ShouldThrow_WhenVoterDoesNotExist()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();
        room.Users = [];

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.ChooseAnswer(room, OwnerId, UserId));

        // THEN
        ex.Message.Should().Contain($"Player {OwnerId} not found in room {room.Code}. (ChooseAnswer)");
    }

    [Fact]
    public async Task ChooseAnswer_ShouldThrow_WhenAnswerIsMissing()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);

        RoomDto room = CreateTestRoom();
        room.Users = [owner, user];

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.ChooseAnswer(room, OwnerId, UserId));

        // THEN
        ex.Message.Should().Contain($"Player {user.UserId} (answerOwner ) has no answer to vote on. Voter: {owner.UserId}. (ChooseAnswer)");
    }

    [Fact]
    public async Task ChooseAnswer_ShouldDoNothing_WhenUserHasAlreadyVoted()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);

        RoomDto room = CreateTestRoom();
        user.Answer = CreateTestAnswer(UserId);
        user.VotersIdsForHisAnswer = [OwnerId];

        room.Users = [owner, user];

        // WHEN
        await _service.ChooseAnswer(room, OwnerId, UserId);

        // THEN
        owner.IsReady.Should().BeFalse(because: "owner has already voted and should not be marked as ready again");

        user.PointsInGame.Should().Be(0, because: "no new votes should be added since owner already voted");

        _roundLogicMock.Verify(r => r.UpdateVotePool(owner, user),
            Times.Never, "UpdateVotePool should not be called because owner has already voted");

        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default),
            Times.Never, "no refresh event should be published if user has already voted");
    }

    [Fact]
    public async Task ChooseAnswer_ShouldUpdatePointsAndSetUsersReady_WhenCalledMultipleTimes()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);

        owner.Answer = CreateTestAnswer();
        user.Answer = CreateTestAnswer();
        user_2.Answer = CreateTestAnswer();

        RoomDto room = CreateTestRoom();
        room.Users = [owner, user, user_2];

        // WHEN
        await _service.ChooseAnswer(room, OwnerId, UserId);
        await _service.ChooseAnswer(room, UserId_2, UserId);
        await _service.ChooseAnswer(room, UserId, UserId_2);

        // THEN
        owner.IsReady.Should().BeTrue(because: "owner participated in voting and should be marked as ready");

        user.IsReady.Should().BeTrue(because: "user participated in voting and should be marked as ready");

        user_2.IsReady.Should().BeTrue(because: "user_2 participated in voting and should be marked as ready");

        owner.PointsInGame.Should().Be(0, because: "owner did not receive votes in this scenario");

        user.PointsInGame.Should().Be(20, because: "user received votes from owner and user_2");

        user_2.PointsInGame.Should().Be(10, because: "user_2 received a vote from user");

        _roundLogicMock.Verify(r => r.UpdateVotePool(owner, user), Times.Once, "owner should have voted for user exactly once");

        _roundLogicMock.Verify(r => r.UpdateVotePool(user_2, user), Times.Once, "user_2 should have voted for user exactly once");

        _roundLogicMock.Verify(r => r.UpdateVotePool(user, user_2), Times.Once, "user should have voted for user_2 exactly once");

        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default),
            Times.Exactly(3), "a refresh event should be published after each vote");
    }
}
