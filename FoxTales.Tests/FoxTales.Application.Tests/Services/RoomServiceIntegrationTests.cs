
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

        room.Code.Should().NotBeNull();
        RoomService.GetRoomsForTest()[room.Code].Users.Should().HaveCount(1);

        // Dołączenie drugiego gracza przez kod
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        RoomService.GetRoomsForTest()[room.Code].Users.Should().HaveCount(1);
        await _roomService.JoinRoom(user, room.Code, null, null);
        RoomService.GetRoomsForTest()[room.Code].Users.Should().HaveCount(2);

        // Ustawienie pokoju na publiczny
        await _roomService.EditRoom(new()
        {
            Owner = owner,
            Users = [owner, user],
            Code = room.Code,
            IsPublic = true
        });

        // Dołączenie trzeciego gracza do publicznego pokoju bez hasła
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);
        await _roomService.JoinRoom(user_2, null, null, OwnerId);
        RoomService.GetRoomsForTest()[room.Code].Users.Should().HaveCount(3);

        // Ustawienie pokoju na publiczny z hasłem
        await _roomService.EditRoom(new()
        {
            Owner = owner,
            Users = [owner, user, user_2],
            Code = room.Code,
            IsPublic = true,
            Password = "password"
        });

        // Dołączenie czwartego gracza do publicznego pokoju z hasłem
        PlayerDto user_3 = CreateTestPlayer(UserId_3, UserName_3, UserConnectionId_3);
        await _roomService.JoinRoom(user_3, null, "password", OwnerId);
        RoomService.GetRoomsForTest()[room.Code].Users.Should().HaveCount(4);

        // Dodanie pytań
        List<QuestionDto> ownerQuestions = [Library["ownerQuestion"], Library["ownerQuestion_2"]];
        await _roomService.AddQuestionsToGame(room.Code, OwnerId, ownerQuestions);
        RoomService.GetRoomsForTest()[room.Code].Questions.Should().Equal(ownerQuestions);

        await _roomService.AddQuestionsToGame(room.Code, UserId, [Library["userQuestion"]]);
        RoomService.GetRoomsForTest()[room.Code].Questions.Should().Equal(ownerQuestions.Append(Library["userQuestion"]));

        // start gry
        await _roomService.StartGame(room.Code, OwnerConnectionId);
        RoomService.GetRoomsForTest()[room.Code].IsGameStarted.Should().BeTrue();

        // Opuszczenie pokoju przez właściciela powoduje zamknięcie
        await _roomService.LeaveRoom(room.Code, OwnerId);
        RoomService.GetRoomsForTest().Should().NotContainKey(room.Code);
    }
}

