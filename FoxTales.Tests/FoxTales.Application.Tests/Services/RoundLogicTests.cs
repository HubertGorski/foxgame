using FluentAssertions;
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces.Logics;
using FoxTales.Application.Services.Logics;
using FoxTales.Application.Tests.Common;

namespace FoxTales.Application.Tests.Services;

public class RoundLogicTests : BaseTest
{
    private readonly IRoundLogic _logic;

    public RoundLogicTests()
    {
        _logic = new RoundLogic();
    }

    [Fact]
    public void GetNewCurrentQuestionWithSelectedPlayer_ShouldThrow_WhenRoomIsEmpty()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();
        room.Questions = [CreateTestQuestion()];
        room.Users = [];

        // WHEN
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _logic.GetNewCurrentQuestionWithSelectedPlayer(room));

        // THEN
        ex.Message.Should().Contain($"Room '{room.Code}' is empty. (GetNewCurrentQuestionWithSelectedPlayer)");
    }

    [Fact]
    public void GetNewCurrentQuestionWithSelectedPlayer_ShouldThrow_WhenQuestionsPoolIsEmpty()
    {
        // GIVEN
        RoomDto room = CreateTestRoom();

        // WHEN
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _logic.GetNewCurrentQuestionWithSelectedPlayer(room));

        // THEN
        ex.Message.Should().Contain($"Question does not exist in room '{room.Code}'. (GetNewCurrentQuestionWithSelectedPlayer)");
    }

    [Fact]
    public void GetNewCurrentQuestionWithSelectedPlayer_ShouldSetCurrentUser_WhenCalled()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        RoomDto room = CreateTestRoom();
        room.Users = [owner, user];
        room.Questions = [CreateTestQuestion()];

        // WHEN
        QuestionDto result = _logic.GetNewCurrentQuestionWithSelectedPlayer(room);

        // THEN
        result.CurrentUser.Should().NotBeNull(because:
            "CurrentUser should be assigned randomly from eligible users");

        room.Users.Should().Contain(result.CurrentUser, because:
            "selected user must be one of the room users");
    }

    [Fact]
    public void GetNewCurrentQuestionWithSelectedPlayer_ShouldUpdateTextAndSelectionCount_WhenCalled()
    {
        // GIVEN
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        RoomDto room = CreateTestRoom();
        room.Users = [user];
        room.Questions = [CreateTestQuestion()];

        // WHEN
        QuestionDto result = _logic.GetNewCurrentQuestionWithSelectedPlayer(room);

        // THEN
        result.CurrentUser.Should().NotBeNull();
        result.Text.Should().Contain(result.CurrentUser.Username, because:
            "the placeholder **** should be replaced with the selected player's username");

        result.Text.Should().Contain(user.Username, because:
            "the placeholder **** should be replaced with the selected player's username");

        user.SelectionCount.Should().Be(1, because:
            "the selected player's SelectionCount should be incremented by the method");
    }

    [Fact]
    public void GetNewCurrentQuestionWithSelectedPlayer_ShouldSelectUserWithMinimalSelectionCount_WhenMultipleUsersPresent()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        owner.SelectionCount = 2;
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.SelectionCount = 1;
        PlayerDto user2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        user2.SelectionCount = 1;

        RoomDto room = CreateTestRoom();
        room.Users = [owner, user, user2];
        room.Questions = [CreateTestQuestion()];

        // WHEN
        QuestionDto result = _logic.GetNewCurrentQuestionWithSelectedPlayer(room);

        // THEN
        result.CurrentUser.Should().NotBeNull(because:
            "CurrentUser should be assigned randomly from eligible users");

        result.CurrentUser.SelectionCount.Should().BeGreaterThan(1, because:
            "the selected user's SelectionCount should have been incremented from minimal value");

        new List<int> { UserId, UserId_2 }.Should().Contain(result.CurrentUser.UserId, because:
            "only users with minimal SelectionCount should be eligible for selection");
    }

    [Fact]
    public void UpdateVotePool_ShouldTrackVotesCorrectly_WhenMultiplePlayersVote()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        PlayerDto user_3 = CreateTestPlayer(UserId_3, UserName_3, UserConnectionId_3);

        owner.Answer = CreateTestAnswer();
        user.Answer = CreateTestAnswer();
        user_2.Answer = CreateTestAnswer();
        user_3.Answer = CreateTestAnswer();

        user.VotesReceived = [new(OwnerId, 1)];
        user.VotesGiven = [new(OwnerId, 1)];

        // WHEN
        _logic.UpdateVotePool(user, owner);
        _logic.UpdateVotePool(user_2, owner);
        _logic.UpdateVotePool(owner, user);
        _logic.UpdateVotePool(user_3, owner);

        // THEN
        owner.Answer.VotersCount.Should().Be(3,
            "because three players voted for owner");

        user.Answer.VotersCount.Should().Be(1,
            "because only owner voted for user");

        user_2.Answer.VotersCount.Should().Be(0,
            "because no one voted for user_2");

        user_3.Answer.VotersCount.Should().Be(0,
            "because no one voted for user_3");

        owner.VotersIdsForHisAnswer.Should().BeEquivalentTo([UserId, UserId_2, UserId_3],
            "because user, user_2 and user_3 voted for owner");

        owner.VotesReceived.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(UserId, 1), new(UserId_2, 1), new(UserId_3, 1) },
            "because each voter gave exactly 1 vote to owner");

        owner.VotesGiven.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(UserId, 1) },
            "because owner gave 1 vote to user");

        user.VotersIdsForHisAnswer.Should().BeEquivalentTo([OwnerId],
            "because only owner voted for user");

        user.VotesReceived.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(OwnerId, 2) },
            "because owner gave 1 vote to user in this round, adding to the existing vote from previous round");

        user.VotesGiven.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(OwnerId, 2) },
            "because user gave 1 vote to owner in this round, adding to the existing vote from previous round");

        user_2.VotersIdsForHisAnswer.Should().BeEmpty(
            "because no one voted for user_2");

        user_2.VotesGiven.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(OwnerId, 1) },
            "because user_2 gave 1 vote to owner");

        user_2.VotesReceived.Should().BeEmpty(
            "because no one voted for user_2");

        user_3.VotersIdsForHisAnswer.Should().BeEmpty(
            "because no one voted for user_3"
        );
        user_3.VotesGiven.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(OwnerId, 1) },
            "because user_3 gave 1 vote to owner");

        user_3.VotesReceived.Should().BeEmpty(
            "because no one voted for user_3"
        );
    }

    [Fact]
    public void UpdateVotePool_ShouldThrowException_WhenOwnerHasNoAnswer()
    {
        // GIVEN
        PlayerDto voter = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);

        // WHEN
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _logic.UpdateVotePool(voter, owner));

        // THEN
        ex.Message.Should().Be($"Player {owner.UserId} not provide answer. (UpdateVotePool)");
    }

    [Fact]
    public void UpdateVotePool_ShouldDoNothing_WhenVoterAlreadyVotedForOwner()
    {
        // GIVEN
        PlayerDto voter = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        owner.Answer = CreateTestAnswer();

        owner.VotersIdsForHisAnswer.Add(voter.UserId);
        owner.VotesReceived.Add(new KeyValuePair<int, int>(voter.UserId, 1));
        voter.VotesGiven.Add(new KeyValuePair<int, int>(owner.UserId, 1));
        owner.Answer.VotersCount = 1;

        // WHEN
        _logic.UpdateVotePool(voter, owner);

        // THEN
        owner.Answer.VotersCount.Should().Be(1, "because voter already voted for owner");
        owner.VotesReceived.Should().HaveCount(1, "because no new vote should be added");
        voter.VotesGiven.Should().HaveCount(1, "because no new vote should be added");
        owner.VotesReceived.Should().ContainSingle(kv => kv.Key == voter.UserId && kv.Value == 1);
        voter.VotesGiven.Should().ContainSingle(kv => kv.Key == owner.UserId && kv.Value == 1);
    }

    [Fact]
    public void AssignPoints_ByPsychRules()
    {
        // GIVEN
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        RoomDto room = CreateTestRoom();

        // WHEN
        _logic.AssignPoints(room, user);

        // THEN
        user.PointsInGame.Should().Be(10, "because voter selected users answer");
    }

    [Theory]
    [
        InlineData(new int[] { UserId }, new int[] { UserId_2, UserId_3 }, new int[] { }, new int[] { OwnerId }, new int[] { 10, 0, 0, 0 },
        "When subject has zero votes and chooses someone else's answer, points should be awarded only to players who guessed subject's answer, subject receives no points, and points are awarded to player chosen by subject"),
        InlineData(new int[] { UserId }, new int[] { UserId_3 }, new int[] { UserId_2 }, new int[] { OwnerId }, new int[] { 0, 0, 0, 0 },
        "When subject has zero votes and chooses their own answer, points should be awarded only to players who guessed subject's answer, and subject receives no points"),
        InlineData(new int[] { }, new int[] { }, new int[] { OwnerId, UserId, UserId_2, UserId_3 }, new int[] { }, new int[] { 10, 10, 0, 10 },
        "When subject receives all votes and chooses their own answer, points should be awarded only to players who guessed subject's answer, and subject receives no points"),
        InlineData(new int[] { }, new int[] { }, new int[] { OwnerId, UserId, UserId_3 }, new int[] { UserId_2 }, new int[] { 10, 10, 0, 20 },
        "When subject receives all votes and chooses someone else's answer, points should be awarded only to players who guessed subject's answer, subject receives no points, and points are awarded to player chosen by subject"),
        InlineData(new int[] { }, new int[] { }, new int[] { OwnerId, UserId_2, UserId_3 }, new int[] { UserId, }, new int[] { 0, 10, 20, 10 },
        "When subject receives some votes and chooses their own answer, points should be awarded to players who guessed subject's answer, and subject also receives points"),
        InlineData(new int[] { UserId_2 }, new int[] { }, new int[] { OwnerId, UserId_3 }, new int[] { UserId, }, new int[] { 0, 20, 20, 10 },
        "When subject receives some votes and chooses someone else's answer, points should be awarded to players who guessed subject's answer, subject receives points, and points are awarded to player chosen by subject"),
    ]
    public void AssignPoints_DixitRules(int[] votersIdsForOwnerAnswer, int[] votersIdsForUserAnswer, int[] votersIdsForUser2Answer, int[] votersIdsForUser3Answer, int[] results, string testTitle)
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        PlayerDto user_3 = CreateTestPlayer(UserId_3, UserName_3, UserConnectionId_3);

        RoomDto room = CreateTestRoom();
        room.UseDixitRules = true;
        room.Users = [user, owner, user_2, user_3];

        QuestionDto currentQuestion = CreateTestQuestion();
        currentQuestion.CurrentUser = user_2;
        room.CurrentQuestion = currentQuestion;

        owner.VotersIdsForHisAnswer = [.. votersIdsForOwnerAnswer];
        user.VotersIdsForHisAnswer = [.. votersIdsForUserAnswer];
        user_2.VotersIdsForHisAnswer = [.. votersIdsForUser2Answer];
        user_3.VotersIdsForHisAnswer = [.. votersIdsForUser3Answer];

        room.Users.ForEach(u => u.IsReady = true);

        // WHEN
        _logic.AssignPoints(room, user_3);

        // THEN
        room.Users.Select(u => u.PointsInGame).Should().Equal(results, testTitle);
    }

    [Fact]
    public void AssignPoints_DixitRules_WhenUsersNotReady_ThenNoActionTaken()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        PlayerDto user_3 = CreateTestPlayer(UserId_3, UserName_3, UserConnectionId_3);

        RoomDto room = CreateTestRoom();
        room.UseDixitRules = true;
        room.Users = [user, owner, user_2, user_3];

        // WHEN
        _logic.AssignPoints(room, user_3);

        // THEN
        owner.PointsInGame.Should().Be(0);
        user.PointsInGame.Should().Be(0);
        user_2.PointsInGame.Should().Be(0);
        user_3.PointsInGame.Should().Be(0);
    }

    [Fact]
    public async Task AssignPoints_DixitRules_WhenCurrentQuestionIsNull_ThenThrowsException()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        PlayerDto user_3 = CreateTestPlayer(UserId_3, UserName_3, UserConnectionId_3);

        RoomDto room = CreateTestRoom();
        room.UseDixitRules = true;
        room.Users = [user, owner, user_2, user_3];
        room.Users.ForEach(u => u.IsReady = true);

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => _logic.AssignPoints(room, user_3));

        // THEN
        ex.Message.Should().Contain($"Question does not exist in room '{room.Code}'.");
    }

    [Fact]
    public async Task AssignPoints_DixitRules_WhenSubjectUserNotAssociatedWithQuestion_ThenThrowsException()
    {
        // GIVEN
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        PlayerDto user_3 = CreateTestPlayer(UserId_3, UserName_3, UserConnectionId_3);

        RoomDto room = CreateTestRoom();
        room.UseDixitRules = true;
        room.Users = [user, owner, user_2, user_3];

        QuestionDto currentQuestion = CreateTestQuestion();
        room.CurrentQuestion = currentQuestion;
        room.Users.ForEach(u => u.IsReady = true);

        // WHEN
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => _logic.AssignPoints(room, user_3));

        // THEN
        ex.Message.Should().Contain($"Question does not exist in room '{room.Code}'.");
    }
}
