using FoxTales.Application.Services.Psych;
using MediatR;
using FluentAssertions;
using Moq;
using FoxTales.Application.Services.Logics;
using FoxTales.Application.Tests.Common;
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Tests.Services;

public class RoomServiceIntegrationTests : BaseTest
{
    private readonly RoomService _roomService;
    private readonly RoundService _roundService;
    private readonly RoundLogic _roundLogic;
    private readonly Mock<IMediator> _mediatorMock;

    private const string TestPassword = "password";

    public RoomServiceIntegrationTests()
    {
        RoomService.ClearRoomsForTest();
        _mediatorMock = new Mock<IMediator>();
        _roundLogic = new RoundLogic();
        _roundService = new RoundService(_mediatorMock.Object, _roundLogic);
        _roomService = new RoomService(_mediatorMock.Object, _roundService);
    }

    [Fact]
    public async Task FullRoomFlow_Should_WorkCorrectly()
    {
        // Create room
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        RoomDto room = new()
        {
            Owner = owner,
            Users = []
        };

        await _roomService.CreateRoom(room);

        string? code = room.Code;
        code.Should().NotBeNull();
        RoomService.GetRoomsForTest()[code].Users.Should().HaveCount(1);

        // Join second player by code
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        await _roomService.JoinRoom(user, code, null, null);
        RoomService.GetRoomsForTest()[code].Users.Should().HaveCount(2);

        // Set room public
        await _roomService.EditRoom(new RoomDto
        {
            Owner = owner,
            Users = [owner, user],
            Code = code,
            IsPublic = true
        });

        // Join third player to public room without password
        PlayerDto user2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        await _roomService.JoinRoom(user2, null, null, OwnerId);
        RoomService.GetRoomsForTest()[code].Users.Should().HaveCount(3);

        // Set room public with password
        await _roomService.EditRoom(new RoomDto
        {
            Owner = owner,
            Users = [owner, user, user2],
            Code = code,
            IsPublic = true,
            Password = TestPassword
        });

        // Join fourth player with password
        PlayerDto user3 = CreateTestPlayer(UserId_3, UserName_3, UserConnectionId_3);
        await _roomService.JoinRoom(user3, null, TestPassword, OwnerId);
        RoomService.GetRoomsForTest()[code].Users.Should().HaveCount(4);

        // Add questions
        List<QuestionDto> ownerQuestions =
        [
            Library["ownerQuestion"],
            Library["ownerQuestion_2"]
        ];
        await _roomService.AddQuestionsToGame(code, OwnerId, ownerQuestions);
        RoomService.GetRoomsForTest()[code].Questions.Should().Equal(ownerQuestions);

        await _roomService.AddQuestionsToGame(code, UserId, [Library["userQuestion"]]);
        RoomService.GetRoomsForTest()[code].Questions.Should().Equal(ownerQuestions.Append(Library["userQuestion"]));

        // Players mark ready
        await _roomService.SetStatus(code, UserId, true);
        await _roomService.SetStatus(code, UserId_2, true);
        await _roomService.SetStatus(code, UserId_3, true);

        // Start game
        await _roomService.StartGame(code, OwnerConnectionId);
        RoomDto activeRoom = RoomService.GetRoomsForTest()[code];
        activeRoom.IsGameStarted.Should().BeTrue();
        activeRoom.Users.Should().OnlyContain(u => !u.IsReady);

        // Players submit answers
        await _roundService.AddAnswer(activeRoom, CreateTestAnswer(UserId));
        await _roundService.AddAnswer(activeRoom, CreateTestAnswer(UserId_2));
        await _roundService.AddAnswer(activeRoom, CreateTestAnswer(UserId_3));
        await _roundService.AddAnswer(activeRoom, CreateTestAnswer(OwnerId));
        activeRoom.Users.Should().OnlyContain(u => u.IsReady);

        // Start voting phase
        await _roundService.MarkAllUsersUnreadyIfOwner(activeRoom, OwnerConnectionId);
        activeRoom.Users.Should().OnlyContain(u => !u.IsReady);

        // Players vote
        await _roundService.ChooseAnswer(activeRoom, OwnerId, UserId);
        await _roundService.ChooseAnswer(activeRoom, UserId, UserId_2);
        await _roundService.ChooseAnswer(activeRoom, UserId_2, UserId);
        await _roundService.ChooseAnswer(activeRoom, UserId_3, OwnerId);
        activeRoom.Users.Should().OnlyContain(u => u.IsReady);

        // Reveal results
        await _roundService.MarkAllUsersUnreadyIfOwner(activeRoom, OwnerConnectionId);
        var users = RoomService.GetRoomsForTest()[code].Users;

        users.Should().OnlyContain(u => !u.IsReady);

        users.First(u => u.UserId == OwnerId).PointsInGame.Should().Be(10);
        users.First(u => u.UserId == UserId).PointsInGame.Should().Be(20);
        users.First(u => u.UserId == UserId_2).PointsInGame.Should().Be(10);
        users.First(u => u.UserId == UserId_3).PointsInGame.Should().Be(0);

        // Verify votes distribution
        users.First(u => u.UserId == OwnerId).Answer!.VotersCount.Should().Be(1);
        users.First(u => u.UserId == UserId).Answer!.VotersCount.Should().Be(2);
        users.First(u => u.UserId == UserId_2).Answer!.VotersCount.Should().Be(1);
        users.First(u => u.UserId == UserId_3).Answer!.VotersCount.Should().Be(0);

        users.First(u => u.UserId == OwnerId).VotesReceived.Should()
            .BeEquivalentTo([new KeyValuePair<int, int>(UserId_3, 1)]);
        users.First(u => u.UserId == OwnerId).VotesGiven.Should()
            .BeEquivalentTo([new KeyValuePair<int, int>(UserId, 1)]);

        users.First(u => u.UserId == UserId).VotesReceived.Should()
            .BeEquivalentTo([new KeyValuePair<int, int>(UserId_2, 1), new(OwnerId, 1)]);
        users.First(u => u.UserId == UserId).VotesGiven.Should()
            .BeEquivalentTo([new KeyValuePair<int, int>(UserId_2, 1)]);

        users.First(u => u.UserId == UserId_2).VotesReceived.Should()
            .BeEquivalentTo([new KeyValuePair<int, int>(UserId, 1)]);
        users.First(u => u.UserId == UserId_2).VotesGiven.Should()
            .BeEquivalentTo([new KeyValuePair<int, int>(UserId, 1)]);

        users.First(u => u.UserId == UserId_3).VotesReceived.Should().BeEmpty();
        users.First(u => u.UserId == UserId_3).VotesGiven.Should()
            .BeEquivalentTo([new KeyValuePair<int, int>(OwnerId, 1)]);

        // Prepare for new round
        await _roomService.SetStatus(code, OwnerId, true);
        await _roomService.SetStatus(code, UserId, true);
        await _roomService.SetStatus(code, UserId_2, true);
        await _roomService.SetStatus(code, UserId_3, true);

        await _roundService.SetNewRound(activeRoom, OwnerConnectionId);
        RoomService.GetRoomsForTest()[code].Users.Should().OnlyContain(u => !u.IsReady);

        // Owner leaves -> room closes
        await _roomService.LeaveRoom(code, OwnerId);
        RoomService.GetRoomsForTest().Should().NotContainKey(code);
    }
}
