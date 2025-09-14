using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces.Psych;
using FoxTales.Application.Services.Psych;
using FoxTales.Domain.Enums;
using MediatR;
using Moq;

namespace FoxTales.Application.Tests.Services;

public class RoomServiceTests
{
    private readonly RoomService _service;

    private const string GameCode = "PIESEK1";
    private const string OwnerName = "Natka";

    public RoomServiceTests()
    {
        RoomService.ClearRoomsForTest();

        var mediatorMock = new Mock<IMediator>();
        var roundServiceMock = new Mock<IRoundService>();
        _service = new RoomService(mediatorMock.Object, roundServiceMock.Object);
    }

    private static PlayerDto CreateTestPlayer(string username)
    {
        return new PlayerDto
        {
            Username = username,
            Avatar = new AvatarDto
            {
                AvatarId = 1,
                Name = AvatarName.Default,
                Source = "default.png",
                IsPremium = false
            }
        };
    }

    private static RoomDto CreateTestRoom(string code, string ownerName)
    {
        var owner = CreateTestPlayer(ownerName);
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
        RoomDto room = CreateTestRoom(GameCode, OwnerName);
        RoomService.AddRoomForTest(room);

        // When
        var result = _service.GetRoomByCode(GameCode);

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
}
