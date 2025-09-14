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

    public RoomServiceTests()
    {
        RoomService.ClearRoomsForTest();

        _mediatorMock = new Mock<IMediator>();
        _roundServiceMock = new Mock<IRoundService>();
        _service = new RoomService(_mediatorMock.Object, _roundServiceMock.Object);
    }

    private static PlayerDto CreateTestPlayer(string username, string? connectionId)
    {
        return new PlayerDto
        {
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

    private static RoomDto CreateTestRoom(string code, string ownerName, string? ownerConnectionId)
    {
        PlayerDto owner = CreateTestPlayer(ownerName, ownerConnectionId);
        return new RoomDto
        {
            Code = code,
            Owner = owner,
            Users = [owner]
        };
    }

    [Fact]
    public void GetRoomByCode_ShouldReturnRoom_WhenCodeExists()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerName, OwnerConnectionId);
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
        PlayerDto owner = CreateTestPlayer(OwnerName, OwnerConnectionId);

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
        RoomDto oldRoom = CreateTestRoom(GameCode, OwnerName, OwnerConnectionId);
        RoomService.AddRoomForTest(oldRoom);
        ICollection<string> oldRooms = RoomService.GetRoomsForTest().Keys;

        RoomDto newRoom = CreateTestRoom(AnotherGameCode, OwnerName, OwnerConnectionId);

        // When
        await _service.CreateRoom(newRoom);

        // Then
        ICollection<string> currentRooms = RoomService.GetRoomsForTest().Keys;

        Assert.Contains(oldRoom.Code, oldRooms);
        Assert.DoesNotContain(oldRoom.Code, currentRooms);

        Assert.DoesNotContain(newRoom.Code, oldRooms);
        Assert.Contains(newRoom.Code, currentRooms);
    }

    [Fact]
    public async Task CreateRoom_ShouldPublishJoinAndRefreshEvents()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerName, OwnerConnectionId);

        // When
        await _service.CreateRoom(room);

        // Then
        _mediatorMock.Verify(m => m.Publish(It.IsAny<JoinRoomEvent>(), default), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<RefreshRoomEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateRoom_ShouldThrow_WhenConnectionIdIsNull()
    {
        // Given
        RoomDto room = CreateTestRoom(GameCode, OwnerName, null);

        // When
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateRoom(room));

        // Then
        Assert.Contains("doesnt have 'ConnectionId'", ex.Message);
    }

}
