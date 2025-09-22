
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
        // Tworzenie pokoju
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        RoomDto room = new()
        {
            Owner = owner,
            Users = []
        };
        await _roomService.CreateRoom(room);
        string? Code = room.Code;
        Code.Should().NotBeNull();
        RoomService.GetRoomsForTest()[Code].Users.Should().HaveCount(1);

        // Dołączenie drugiego gracza przez kod
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        RoomService.GetRoomsForTest()[Code].Users.Should().HaveCount(1);
        await _roomService.JoinRoom(user, Code, null, null);
        RoomService.GetRoomsForTest()[Code].Users.Should().HaveCount(2);

        // Ustawienie pokoju na publiczny
        await _roomService.EditRoom(new()
        {
            Owner = owner,
            Users = [owner, user],
            Code = Code,
            IsPublic = true
        });

        // Dołączenie trzeciego gracza do publicznego pokoju bez hasła
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        await _roomService.JoinRoom(user_2, null, null, OwnerId);
        RoomService.GetRoomsForTest()[Code].Users.Should().HaveCount(3);

        // Ustawienie pokoju na publiczny z hasłem
        await _roomService.EditRoom(new()
        {
            Owner = owner,
            Users = [owner, user, user_2],
            Code = Code,
            IsPublic = true,
            Password = "password"
        });

        // Dołączenie czwartego gracza do publicznego pokoju z hasłem
        PlayerDto user_3 = CreateTestPlayer(UserId_3, UserName_3, UserConnectionId_3);
        await _roomService.JoinRoom(user_3, null, "password", OwnerId);
        RoomService.GetRoomsForTest()[Code].Users.Should().HaveCount(4);

        // Dodanie pytań
        List<QuestionDto> ownerQuestions = [Library["ownerQuestion"], Library["ownerQuestion_2"]];
        await _roomService.AddQuestionsToGame(Code, OwnerId, ownerQuestions);
        RoomService.GetRoomsForTest()[Code].Questions.Should().Equal(ownerQuestions);

        await _roomService.AddQuestionsToGame(Code, UserId, [Library["userQuestion"]]);
        RoomService.GetRoomsForTest()[Code].Questions.Should().Equal(ownerQuestions.Append(Library["userQuestion"]));

        // Zgłaszanie gotowości do gry
        await _roomService.SetStatus(Code, UserId, true);
        await _roomService.SetStatus(Code, UserId_2, true);
        await _roomService.SetStatus(Code, UserId_3, true);

        // start gry
        await _roomService.StartGame(Code, OwnerConnectionId);
        RoomService.GetRoomsForTest()[Code].IsGameStarted.Should().BeTrue();
        RoomService.GetRoomsForTest()[Code].Users.All(u => u.IsReady).Should().BeFalse();

        // gracze dodaja odpowiedzi
        room = RoomService.GetRoomsForTest()[Code];
        await _roundService.AddAnswer(room, CreateTestAnswer(UserId));
        await _roundService.AddAnswer(room, CreateTestAnswer(UserId_2));
        await _roundService.AddAnswer(room, CreateTestAnswer(UserId_3));
        await _roundService.AddAnswer(room, CreateTestAnswer(OwnerId));
        RoomService.GetRoomsForTest()[Code].Users.All(u => u.IsReady).Should().BeTrue();

        // Rozpoczęcie fazy głosowania
        await _roundService.MarkAllUsersUnreadyIfOwner(room, OwnerConnectionId);
        RoomService.GetRoomsForTest()[Code].Users.All(u => u.IsReady).Should().BeFalse();

        // gracze głosują
        await _roundService.ChooseAnswer(room, OwnerId, UserId);
        await _roundService.ChooseAnswer(room, UserId, UserId_2);
        await _roundService.ChooseAnswer(room, UserId_2, UserId);
        await _roundService.ChooseAnswer(room, UserId_3, OwnerId);
        RoomService.GetRoomsForTest()[Code].Users.All(u => u.IsReady).Should().BeTrue();

        // Rozpoczęcie ujawnienie wyników
        await _roundService.MarkAllUsersUnreadyIfOwner(room, OwnerConnectionId);
        List<PlayerDto> usersInRoom = RoomService.GetRoomsForTest()[Code].Users;
        usersInRoom.All(u => u.IsReady).Should().BeFalse();
        usersInRoom.First(u => u.UserId == OwnerId).PointsInGame.Should().Be(10);
        usersInRoom.First(u => u.UserId == UserId).PointsInGame.Should().Be(20);
        usersInRoom.First(u => u.UserId == UserId_2).PointsInGame.Should().Be(10);
        usersInRoom.First(u => u.UserId == UserId_3).PointsInGame.Should().Be(0);

        usersInRoom.First(u => u.UserId == OwnerId).Answer!.VotersCount.Should().Be(1);
        usersInRoom.First(u => u.UserId == UserId).Answer!.VotersCount.Should().Be(2);
        usersInRoom.First(u => u.UserId == UserId_2).Answer!.VotersCount.Should().Be(1);
        usersInRoom.First(u => u.UserId == UserId_3).Answer!.VotersCount.Should().Be(0);

        usersInRoom.First(u => u.UserId == OwnerId).VotesReceived.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(UserId_3, 1) });
        usersInRoom.First(u => u.UserId == OwnerId).VotesGiven.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(UserId, 1) });

        usersInRoom.First(u => u.UserId == UserId).VotesReceived.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(UserId_2, 1), new(OwnerId, 1) });
        usersInRoom.First(u => u.UserId == UserId).VotesGiven.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(UserId_2, 1) });

        usersInRoom.First(u => u.UserId == UserId_2).VotesReceived.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(UserId, 1) });
        usersInRoom.First(u => u.UserId == UserId_2).VotesGiven.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(UserId, 1) });

        usersInRoom.First(u => u.UserId == UserId_3).VotesReceived.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { });
        usersInRoom.First(u => u.UserId == UserId_3).VotesGiven.Should().BeEquivalentTo(new List<KeyValuePair<int, int>> { new(OwnerId, 1) });

        await _roomService.SetStatus(Code, OwnerId, true);
        await _roomService.SetStatus(Code, UserId, true);
        await _roomService.SetStatus(Code, UserId_2, true);
        await _roomService.SetStatus(Code, UserId_3, true);

        // Rozpoczęcie nowej rundy
        await _roundService.SetNewRound(room, OwnerConnectionId);
        RoomService.GetRoomsForTest()[Code].Users.All(u => u.IsReady).Should().BeFalse();

        // Opuszczenie pokoju przez właściciela powoduje zamknięcie
        await _roomService.LeaveRoom(Code, OwnerId);
        RoomService.GetRoomsForTest().Should().NotContainKey(Code);
    }
}

