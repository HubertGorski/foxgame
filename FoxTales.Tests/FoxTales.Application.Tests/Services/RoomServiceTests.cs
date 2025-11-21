using System.Collections.Concurrent;
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Events;
using FoxTales.Application.Interfaces.Logics;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Application.Services.Psych;
using FoxTales.Application.Services.Stores;
using FoxTales.Application.Tests.Common;
using MediatR;
using Moq;

namespace FoxTales.Application.Tests.Services;

public class RoomServiceTests : BaseTest
{
    private readonly IRoomService _service;
    private readonly RoomStore _store;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRoundService> _roundServiceMock;
    private readonly Mock<IRoomLogic> _roomLogicMock;
    private readonly Mock<IPsychLibraryService> _libraryServiceMock;

    public RoomServiceTests()
    {
        _store = new RoomStore();
        _mediatorMock = new Mock<IMediator>();
        _roundServiceMock = new Mock<IRoundService>();
        _roomLogicMock = new Mock<IRoomLogic>();
        _libraryServiceMock = new Mock<IPsychLibraryService>();
        _service = new RoomService(_mediatorMock.Object, _roundServiceMock.Object, _store, _libraryServiceMock.Object, _roomLogicMock.Object);
    }

    private void AddRoomForTest(RoomDto room)
    {
        _store.SetRoom(room.Code, room);
    }

    private ConcurrentDictionary<string, RoomDto> GetRoomsForTest()
    {
        return _store.GetAllRoomsWithCodes();
    }

    [Fact]
    public void GetRoomByCode_ShouldReturnRoom_WhenCodeExists()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        RoomDto result = _service.GetRoomByCode(GameCode);

        // Then
        Assert.NotNull(result);
        Assert.Equal(GameCode, result.Code);
        Assert.Equal(OwnerName, result.Owner.Username);
    }

    [Fact]
    public void GetRoomByCode_ShouldThrow_WhenCodeDoesNotExist()
    {
        // When
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _service.GetRoomByCode(GameCode));

        // Then
        Assert.Equal($"Code '{GameCode}' does not exist", ex.Message);
    }

    [Fact]
    public async Task CreateRoom_ShouldGenerateCodeAndAddRoom()
    {
        // Given
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);

        owner.IsReady = true;
        RoomDto room = new()
        {
            Owner = owner,
            Users = []
        };

        // When
        await _service.CreateRoom(room);

        // Then
        Assert.False(owner.IsReady);
        Assert.Contains(owner, room.Users);
        Assert.NotNull(room.Code);
        Assert.True(GetRoomsForTest().ContainsKey(room.Code));
    }

    [Fact]
    public async Task CreateRoom_ShouldRemovePreviousRoomsOfOwner()
    {
        // Given
        RoomDto oldRoom = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(oldRoom);
        ICollection<string> oldRooms = GetRoomsForTest().Keys;

        RoomDto newRoom = CreateTestRoom(null, OwnerId, OwnerName, OwnerConnectionId);

        // When
        await _service.CreateRoom(newRoom);

        // Then
        ICollection<string> currentRooms = GetRoomsForTest().Keys;
        Assert.NotNull(newRoom.Code);

        Assert.Contains(oldRoom.Code, oldRooms);
        Assert.DoesNotContain(oldRoom.Code, currentRooms);

        Assert.DoesNotContain(newRoom.Code, oldRooms);
        Assert.Contains(newRoom.Code, currentRooms);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateRoom_ShouldRemoveUserFromAllOtherRooms()
    {
        // Given
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);

        RoomDto oldRoom = new()
        {
            Owner = owner,
            Users = [owner, user],
            Code = GameCode
        };
        AddRoomForTest(oldRoom);

        RoomDto newRoom = CreateTestRoom(null, UserId, UserName, UserConnectionId, []);

        // When
        await _service.CreateRoom(newRoom);

        // Then
        Assert.DoesNotContain(oldRoom.Users, u => u.UserId == user.UserId);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == oldRoom), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == newRoom), default), Times.Once);
    }

    [Fact]
    public async Task CreateRoom_ShouldPublishJoinAndRefreshEvents()
    {
        // Given
        RoomDto room = CreateTestRoom(null, OwnerId, OwnerName, OwnerConnectionId);

        // When
        await _service.CreateRoom(room);

        // Then
        _mediatorMock.Verify(m => m.Publish(It.Is<JoinRoomEvent>(e => e.ConnectionId == OwnerConnectionId && e.Code == room.Code), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task CreateRoom_ShouldThrow_WhenConnectionIdIsNull()
    {
        // Given
        RoomDto room = CreateTestRoom(null, OwnerId, OwnerName, null);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateRoom(room));

        // Then
        Assert.Contains("does not have 'ConnectionId'", ex.Message);
    }

    [Fact]
    public async Task EditRoom_ShouldThrow_WhenCodeIsNull()
    {
        // Given
        RoomDto room = CreateTestRoom(null, OwnerId, OwnerName, OwnerConnectionId);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.EditRoom(room));

        // Then
        Assert.Contains($"Code '' is invalid (Edit Room)", ex.Message);
    }

    [Fact]
    public async Task EditRoom_ShouldThrow_WhenRoomIsEmpty()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId, []);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.EditRoom(room));

        // Then
        Assert.Contains("Room is empty! (Edit Room)", ex.Message);
    }

    [Fact]
    public async Task EditRoom_ShouldThrow_WhenPlayersCountIsNotCorrect()
    {
        // Given
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId, [owner, user]);
        AddRoomForTest(room);

        RoomDto updatedRoom = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId, [owner]);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.EditRoom(updatedRoom));

        // Then
        Assert.Contains($"The number of players is not correct in '{room.Code}' room! (Edit Room)", ex.Message);
    }

    [Fact]
    public async Task EditRoom_ShouldThrow_WhenRoomDoesNotExist()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.EditRoom(room));

        // Then
        Assert.Contains($"Code '{GameCode}' does not exist", ex.Message);
    }

    [Fact]
    public async Task EditRoom_ShouldPublishJoinAndRefreshEvents_WhenRoomIsPublic()
    {
        // Given
        string passwordTest = "test123";
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        RoomDto updatedRoom = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        updatedRoom.Password = passwordTest;
        updatedRoom.IsPublic = true;

        // When
        await _service.EditRoom(updatedRoom);

        // Then
        var currentRoom = GetRoomsForTest().First();
        Assert.NotNull(currentRoom.Value.Password);
        Assert.Contains(currentRoom.Value.Password, passwordTest);

        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshPublicRoomsListEvent>(e => e.PublicRooms.Contains(updatedRoom)), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == updatedRoom), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Never);

    }

    [Fact]
    public async Task EditRoom_ShouldPublishJoinAndRefreshEvents_WhenRoomIsNotPublic()
    {
        // Given
        string passwordTest = "test123";
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        RoomDto updatedRoom = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        updatedRoom.Password = passwordTest;

        // When
        await _service.EditRoom(updatedRoom);

        // Then
        var currentRoom = GetRoomsForTest().First();
        Assert.NotNull(currentRoom.Value.Password);
        Assert.Contains(currentRoom.Value.Password, passwordTest);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshPublicRoomsListEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == updatedRoom), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Never);
    }

    [Fact]
    public async Task SetStatus_ShouldThrow_WhenUserDoesNotExist()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.SetStatus(GameCode, UserId, true));

        // Then
        Assert.Contains($"Player {UserId} not found in room {GameCode} (SetStatus)", ex.Message);
    }

    [Fact]
    public async Task SetStatus_ShouldThrow_WhenRoomDoesNotExist()
    {
        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.SetStatus(GameCode, OwnerId, true));

        // Then
        Assert.Contains($"Code '{GameCode}' does not exist", ex.Message);
    }

    [Fact]
    public async Task SetStatus_ShouldSetReady()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        await _service.SetStatus(GameCode, OwnerId, true);

        // Then
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room.Users.All(u => u.IsReady)), default), Times.Once);
    }

    [Fact]
    public async Task SetStatus_ShouldSetNotReady()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto owner = room.Users.Single(u => u.UserId == OwnerId);
        owner.IsReady = true;
        AddRoomForTest(room);

        // When
        await _service.SetStatus(GameCode, OwnerId, false);

        // Then
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room.Users.All(u => !u.IsReady)), default), Times.Once);
    }

    [Fact]
    public async Task SetStatus_ShouldNotPublishEvent_WhenStatusAlreadyTrue()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto owner = room.Users.Single(u => u.UserId == OwnerId);
        owner.IsReady = true;
        AddRoomForTest(room);

        // When
        await _service.SetStatus(GameCode, OwnerId, true);

        // Then
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task SetStatus_ShouldNotPublishEvent_WhenStatusAlreadyFalse()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto owner = room.Users.Single(u => u.UserId == OwnerId);
        owner.IsReady = false;
        AddRoomForTest(room);

        // When
        await _service.SetStatus(GameCode, OwnerId, false);

        // Then
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Never);
    }

    [Theory]
    [InlineData(false, "Code '{0}' does not exist")]
    [InlineData(true, "Room '{0}' does not have any questions! (StartGame)")]
    public async Task StartGame_ShouldThrow_WhenInvalidRoom(bool addRoom, string expectedMessage)
    {
        // Given
        if (addRoom)
        {
            RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
            AddRoomForTest(room);
        }

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.StartGame(GameCode, OwnerConnectionId));

        // Then
        Assert.Contains(string.Format(expectedMessage, GameCode), ex.Message);
    }

    [Fact]
    public async Task StartGame_ShouldInitializeGameProperly()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        room.Questions = [Library["ownerQuestion"]];
        AddRoomForTest(room);

        // When
        await _service.StartGame(GameCode, OwnerConnectionId);

        // Then
        _roundServiceMock.Verify(m => m.SetNewRound(room, OwnerConnectionId), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshPublicRoomsListEvent>(), default), Times.Once);
        Assert.True(room.IsGameStarted);
    }


    [Fact]
    public async Task RefreshPublicRoomsList_ShouldRefreshPublicList_WhenPublicRoomExists()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        room.IsPublic = true;
        AddRoomForTest(room);

        // When
        await _service.RefreshPublicRoomsList();

        // Then
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshPublicRoomsListEvent>(e => e.PublicRooms.Contains(room)), default), Times.Once);
    }

    [Fact]
    public async Task RefreshPublicRoomsList_ShouldRefreshPublicList_WhenPublicRoomDoenstExists()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        await _service.RefreshPublicRoomsList();

        // Then
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshPublicRoomsListEvent>(e => !e.PublicRooms.Contains(room)), default), Times.Once);
    }

    [Fact]
    public async Task LeaveRoom_ShouldThrow_WhenRoomDoesNotExist()
    {
        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.LeaveRoom(GameCode, OwnerId));

        // Then
        Assert.Contains($"Code '{GameCode}' does not exist", ex.Message);
    }

    [Fact]
    public async Task LeaveRoom_ShouldThrow_WhenPlayerToRemoveDoesNotExist()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.LeaveRoom(GameCode, UserId));

        // Then
        Assert.Contains($"Player {UserId} not found in room '{GameCode}' (LeaveRoom)", ex.Message);
    }

    [Fact]
    public async Task LeaveRoom_ShouldCloseRoom_WhenOwnerLeaves()
    {
        // Given
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);

        RoomDto room = new()
        {
            Owner = owner,
            Users = [owner, user],
            Code = GameCode
        };
        AddRoomForTest(room);

        // When
        await _service.LeaveRoom(GameCode, OwnerId);

        // Then
        Assert.False(GetRoomsForTest().ContainsKey(room.Code));
        _mediatorMock.Verify(m => m.Publish(It.Is<PlayerLeftRoomEvent>(e => e.Code == room.Code && e.PlayerToRemove == owner), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<PlayerLeftRoomEvent>(e => e.Code == room.Code && e.PlayerToRemove == user), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<RoomClosedEvent>(e => e.PlayersInRoom.Contains(user)), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RoomClosedEvent>(e => e.PlayersInRoom.Contains(owner)), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshPublicRoomsListEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task LeaveRoom_ShouldNotCloseRoomAndReducePlayerList_WhenPlayerLeaves()
    {
        // Given
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);

        RoomDto room = new()
        {
            Owner = owner,
            Users = [owner, user],
            Code = GameCode
        };
        AddRoomForTest(room);

        // When
        await _service.LeaveRoom(GameCode, UserId);

        // Then
        Assert.True(GetRoomsForTest().ContainsKey(room.Code));
        _mediatorMock.Verify(m => m.Publish(It.Is<PlayerLeftRoomEvent>(e => e.Code == room.Code && e.PlayerToRemove == owner), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<PlayerLeftRoomEvent>(e => e.Code == room.Code && e.PlayerToRemove == user), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshPublicRoomsListEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task AddPrivateQuestionsToGame_ShouldThrow_WhenRoomDoesNotExist()
    {
        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.AddPrivateQuestionsToGame(GameCode, OwnerId, []));

        // Then
        Assert.Contains($"Code '{GameCode}' does not exist", ex.Message);
    }

    [Fact]
    public async Task AddPrivateQuestionsToGame_WhenOwnerAddsNewQuestions_ThenOnlyOwnersQuestionsAreReplaced()
    {
        List<QuestionDto> oldQuestions = [Library["ownerQuestion"], Library["userQuestion"]];
        List<QuestionDto> newQuestions = [Library["ownerQuestion"], Library["ownerQuestion_2"]];
        List<QuestionDto> expectedQuestions = [Library["ownerQuestion"], Library["ownerQuestion_2"], Library["userQuestion"]];
        await RunAddQuestionsTest(oldQuestions, newQuestions, expectedQuestions, OwnerId);
    }

    [Fact]
    public async Task AddPrivateQuestionsToGame_WhenOwnerAddsDifferentQuestions_ThenOldOwnerQuestionsAreRemoved()
    {
        List<QuestionDto> oldQuestions = [Library["ownerQuestion"], Library["userQuestion"]];
        List<QuestionDto> newQuestions = [Library["ownerQuestion_2"]];
        List<QuestionDto> expectedQuestions = [Library["ownerQuestion_2"], Library["userQuestion"]];
        await RunAddQuestionsTest(oldQuestions, newQuestions, expectedQuestions, OwnerId);
    }

    [Fact]
    public async Task AddPrivateQuestionsToGame_WhenOwnerAddsNoQuestions_ThenRemoveOwnerQuestions()
    {
        List<QuestionDto> oldQuestions = [Library["ownerQuestion"], Library["userQuestion"]];
        List<QuestionDto> newQuestions = [];
        List<QuestionDto> expectedQuestions = [Library["userQuestion"]];
        await RunAddQuestionsTest(oldQuestions, newQuestions, expectedQuestions, OwnerId);
    }

    private async Task RunAddQuestionsTest(List<QuestionDto> oldQuestions, List<QuestionDto> newQuestions, List<QuestionDto> expectedQuestions, int playerId, bool usePublicQuestions = false)
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        room.Questions = oldQuestions;
        room.UsePublicQuestions = usePublicQuestions;
        AddRoomForTest(room);
        bool hasNoPublicQuestions = !room.Questions.Any(q => q.IsPublic);

        _libraryServiceMock.Setup(r => r.GetPublicQuestionsByCatalogId(1)).ReturnsAsync([Library["publicQuestion"]]);

        // When
        await _service.AddPrivateQuestionsToGame(GameCode, playerId, newQuestions);

        // Then
        List<QuestionDto> currentQuestions = GetRoomsForTest().Values.First().Questions;

        Assert.Equal(expectedQuestions.Count, currentQuestions.Count);
        Assert.All(expectedQuestions, q => Assert.Contains(q, currentQuestions));

        if (usePublicQuestions && hasNoPublicQuestions)
        {
            _libraryServiceMock.Verify(r => r.GetPublicQuestionsByCatalogId(1), Times.Once);
        }
        else
        {
            _libraryServiceMock.Verify(r => r.GetPublicQuestionsByCatalogId(1), Times.Never);
        }

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task JoinRoom_ShouldThrow_WhenConnectionIdIsNull()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, null);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.JoinRoom(user, GameCode, null, null));

        // Then
        Assert.Contains("does not have 'ConnectionId'", ex.Message);
    }

    [Fact]
    public async Task JoinRoom_ShouldCloseOldRoom_WhenOwnerAlreadyHasRoom()
    {
        // Given
        PlayerDto owner = CreateTestPlayer(OwnerId, OwnerName, OwnerConnectionId);
        owner.IsReady = true;

        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        RoomDto anotherRoom = CreateTestRoom(AnotherGameCode, UserId, UserName, UserConnectionId);
        AddRoomForTest(anotherRoom);

        // When
        await _service.JoinRoom(owner, AnotherGameCode, null, null);

        // Then
        Assert.False(owner.IsReady);
        Assert.Contains(owner, anotherRoom.Users);
        Assert.DoesNotContain(owner, room.Users);
        Assert.NotNull(room.Code);
        Assert.NotNull(anotherRoom.Code);
        Assert.False(GetRoomsForTest().ContainsKey(room.Code));
        Assert.True(GetRoomsForTest().ContainsKey(anotherRoom.Code));

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<JoinRoomEvent>(e => e.ConnectionId == OwnerConnectionId && e.Code == AnotherGameCode), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == anotherRoom), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task JoinRoom_ShouldSwitchRooms_WhenUserInAnotherRoom()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        PlayerDto user_2 = CreateTestPlayer(UserId_2, UserName_2, UserConnectionId_2);

        user_2.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        RoomDto anotherRoom = CreateTestRoom(AnotherGameCode, UserId, UserName, UserConnectionId, [user_2, user]);
        AddRoomForTest(anotherRoom);

        // When
        await _service.JoinRoom(user_2, GameCode, null, null);

        // Then
        Assert.False(user_2.IsReady);
        Assert.Contains(user_2, room.Users);
        Assert.DoesNotContain(user_2, anotherRoom.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<JoinRoomEvent>(e => e.ConnectionId == UserConnectionId_2 && e.Code == GameCode), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == anotherRoom), default), Times.Once);
    }

    [Fact]
    public async Task JoinRoom_ShouldJoin_WhenUserProvidesCorrectCode()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        await _service.JoinRoom(user, GameCode, null, null);

        // Then
        Assert.False(user.IsReady);
        Assert.Contains(user, room.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<JoinRoomEvent>(e => e.ConnectionId == UserConnectionId && e.Code == GameCode), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once);
    }

    [Fact]
    public async Task JoinRoom_ShouldNotJoin_WhenUserProvidesWrongCode()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        await _service.JoinRoom(user, AnotherGameCode, null, null);

        // Then
        Assert.DoesNotContain(user, room.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<JoinRoomEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task JoinRoom_ShouldNotJoin_WhenUserOmitsCode()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        await _service.JoinRoom(user, null, null, null);

        // Then
        Assert.DoesNotContain(user, room.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<JoinRoomEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task JoinRoom_ShouldNotJoin_WhenUserSelectsRoomWithWrongPassword()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        room.Password = "OK PASS";
        AddRoomForTest(room);

        // When
        await _service.JoinRoom(user, null, "NOK PASS", OwnerId);

        // Then
        Assert.DoesNotContain(user, room.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<JoinRoomEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task JoinRoom_ShouldNotJoin_WhenUserSelectsRoomWithEmptyPassword()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        room.Password = "OK PASS";
        AddRoomForTest(room);

        // When
        await _service.JoinRoom(user, null, null, OwnerId);

        // Then
        Assert.DoesNotContain(user, room.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<JoinRoomEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task JoinRoom_ShouldJoin_WhenUserSelectsRoomWithCorrectPassword()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        room.Password = "OK PASS";
        AddRoomForTest(room);

        // When
        await _service.JoinRoom(user, null, "OK PASS", OwnerId);

        // Then
        Assert.False(user.IsReady);
        Assert.Contains(user, room.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<JoinRoomEvent>(e => e.ConnectionId == UserConnectionId && e.Code == GameCode), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once);
    }

    [Fact]
    public async Task JoinRoom_ShouldJoin_WhenUserSelectsRoomWithoutPassword()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        await _service.JoinRoom(user, null, null, OwnerId);

        // Then
        Assert.False(user.IsReady);
        Assert.Contains(user, room.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<JoinRoomEvent>(e => e.ConnectionId == UserConnectionId && e.Code == GameCode), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once);
    }

    [Fact]
    public async Task JoinRoom_ShouldJoin_WhenUserSelectsRoomWithoutPasswordAndEntersAnything()
    {
        // Given
        PlayerDto user = CreateTestPlayer(UserId, UserName, UserConnectionId);
        user.IsReady = true;
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        AddRoomForTest(room);

        // When
        await _service.JoinRoom(user, null, "OK PASS", OwnerId);

        // Then
        Assert.False(user.IsReady);
        Assert.Contains(user, room.Users);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RoomClosedEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<JoinRoomEvent>(e => e.ConnectionId == UserConnectionId && e.Code == GameCode), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Once);
    }
}
