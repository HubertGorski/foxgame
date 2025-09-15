using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Events;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Application.Services.Psych;
using FoxTales.Domain.Enums;
using MediatR;
using Moq;

namespace FoxTales.Application.Tests.Services;

public class RoomServiceTests
{
    private readonly IRoomService _service;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRoundService> _roundServiceMock;

    private const string GameCode = "PIESEK1";
    private const string AnotherGameCode = "MIASTO69";
    private const string OwnerName = "Natka";
    private const string OwnerConnectionId = "1";
    private const int OwnerId = 1;
    private const string UserName = "Hubi";
    private const string UserConnectionId = "2";
    private const int UserId = 2;

    public RoomServiceTests()
    {
        RoomService.ClearRoomsForTest();

        _mediatorMock = new Mock<IMediator>();
        _roundServiceMock = new Mock<IRoundService>();
        _service = new RoomService(_mediatorMock.Object, _roundServiceMock.Object);
    }

    private static PlayerDto CreateTestPlayer(int userId, string username, string? connectionId)
    {
        return new PlayerDto
        {
            UserId = userId,
            Username = username,
            ConnectionId = connectionId,
            Avatar = new AvatarDto
            {
                AvatarId = 1,
                Name = AvatarName.Default,
                IsPremium = false
            }
        };
    }

    private static RoomDto CreateTestRoom(string? code, int ownerId, string ownerName, string? ownerConnectionId, List<PlayerDto>? users = null)
    {
        PlayerDto owner = CreateTestPlayer(ownerId, ownerName, ownerConnectionId);
        return new RoomDto
        {
            Code = code,
            Owner = owner,
            Users = users ?? [owner]
        };
    }

    [Fact]
    public void GetRoomByCode_ShouldReturnRoom_WhenCodeExists()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        RoomService.AddRoomForTest(room);

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
        Assert.Equal($"Code '{GameCode}' doesnt exist", ex.Message);
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
        Assert.True(RoomService.GetRoomsForTest().ContainsKey(room.Code));
    }

    [Fact]
    public async Task CreateRoom_ShouldRemovePreviousRoomsOfOwner()
    {
        // Given
        RoomDto oldRoom = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        RoomService.AddRoomForTest(oldRoom);
        ICollection<string> oldRooms = RoomService.GetRoomsForTest().Keys;

        RoomDto newRoom = CreateTestRoom(null, OwnerId, OwnerName, OwnerConnectionId);

        // When
        await _service.CreateRoom(newRoom);

        // Then
        ICollection<string> currentRooms = RoomService.GetRoomsForTest().Keys;
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
        RoomService.AddRoomForTest(oldRoom);

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
        Assert.Contains("doesnt have 'ConnectionId'", ex.Message);
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
    public async Task EditRoom_ShouldThrow_WhenRoomDoesntExist()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.EditRoom(room));

        // Then
        Assert.Contains($"Code '{GameCode}' doesnt exist", ex.Message);
    }

    [Fact]
    public async Task EditRoom_ShouldPublishJoinAndRefreshEvents_WhenRoomIsPublic()
    {
        // Given
        string passwordTest = "test123";
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        RoomService.AddRoomForTest(room);

        RoomDto updatedRoom = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        updatedRoom.Password = passwordTest;
        updatedRoom.IsPublic = true;

        // When
        await _service.EditRoom(updatedRoom);

        // Then
        var currentRoom = RoomService.GetRoomsForTest().First();
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
        RoomService.AddRoomForTest(room);

        RoomDto updatedRoom = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        updatedRoom.Password = passwordTest;

        // When
        await _service.EditRoom(updatedRoom);

        // Then
        var currentRoom = RoomService.GetRoomsForTest().First();
        Assert.NotNull(currentRoom.Value.Password);
        Assert.Contains(currentRoom.Value.Password, passwordTest);

        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshPublicRoomsListEvent>(), default), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == updatedRoom), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room == room), default), Times.Never);
    }

    [Fact]
    public async Task SetStatus_ShouldThrow_WhenUserDoesntExist()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        RoomService.AddRoomForTest(room);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.SetStatus(GameCode, UserId, true));

        // Then
        Assert.Contains($"Player {UserId} not found in room {GameCode}", ex.Message);
    }

    [Fact]
    public async Task SetStatus_ShouldThrow_WhenRoomDoesntExist()
    {
        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.SetStatus(GameCode, OwnerId, true));

        // Then
        Assert.Contains($"Code '{GameCode}' doesnt exist", ex.Message);
    }

    [Fact]
    public async Task SetStatus_ShouldSetReady()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerId, OwnerName, OwnerConnectionId);
        RoomService.AddRoomForTest(room);

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
        RoomService.AddRoomForTest(room);

        // When
        await _service.SetStatus(GameCode, OwnerId, false);

        // Then
        _mediatorMock.Verify(m => m.Publish(It.Is<RefreshRoomEvent>(e => e.Room.Users.All(u => !u.IsReady)), default), Times.Once);
    }
}
