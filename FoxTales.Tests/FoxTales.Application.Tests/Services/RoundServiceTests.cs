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
        room.Questions = [Library["ownerQuestion"]];
        _roundLogicMock.Setup(r => r.GetNewCurrentQuestionWithSelectedPlayer(room)).Returns(Library["ownerQuestion"]);

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.SetNewRound(room, OwnerConnectionId));

        // THEN
        Assert.Contains($"Player not found in room {room.Code}. (SetNewRound)", ex.Message);
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
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), It.IsAny<CancellationToken>()), Times.Never);
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
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), It.IsAny<CancellationToken>()), Times.Never);
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
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
