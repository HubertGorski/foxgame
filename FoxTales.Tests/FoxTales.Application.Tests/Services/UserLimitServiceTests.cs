using AutoMapper;
using FluentAssertions;
using FoxTales.Application.DTOs.FoxGame;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Services;
using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using FoxTales.Domain.Interfaces;
using Moq;

namespace FoxTales.Application.Tests.Services;

public class UserLimitServiceTests
{
    private readonly Mock<IFoxGameRepository> _foxGameRepositoryMock;
    private readonly IMapper _mapper;
    private readonly UserLimitService _service;

    public UserLimitServiceTests()
    {
        _foxGameRepositoryMock = new Mock<IFoxGameRepository>();

        MapperConfiguration config = new(cfg =>
        {
            cfg.CreateMap<FoxGame, FoxGameDto>();
        });
        _mapper = config.CreateMapper();

        _service = new UserLimitService(_foxGameRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllFoxGames_ShouldReturnMappedDtos()
    {
        // Given
        List<FoxGame> games =
        [
            new() { FoxGameId = 1, Name = FoxGameName.Dylematy },
            new() { FoxGameId = 2, Name = FoxGameName.Psych }
        ];

        _foxGameRepositoryMock.Setup(r => r.GetAllFoxGames())
            .ReturnsAsync(games);

        // When
        ICollection<FoxGameDto> result = await _service.GetAllFoxGames();

        // Then
        result.Should().HaveCount(2);
        result.Should().BeOfType<List<FoxGameDto>>();
        result.Select(g => g.FoxGameId).Should().Contain([1, 2]);
    }

    [Fact]
    public void ApplyClosestThresholds_ShouldComputeClosestThresholdForEachItem_TypicalCase()
    {
        // Given
        List<UserLimitDto> userLimits =
    [
        new() { Type = LimitType.Achievement, LimitId = 1, CurrentValue = 3, Thresholds = [1, 5, 10] },
        new() { Type = LimitType.Achievement, LimitId = 2, CurrentValue = 2, Thresholds = [2, 8, 10] },
        new() { Type = LimitType.Achievement, LimitId = 3, CurrentValue = 5, Thresholds = [2, 4, 11, 14] },
        new() { Type = LimitType.PermissionGame, LimitId = (int)FoxGameName.Psych, CurrentValue = 1, Thresholds = [1] },
        new() { Type = LimitType.PermissionGame, LimitId = (int)FoxGameName.Dylematy, CurrentValue = 0, Thresholds = [1] },
        new() { Type = LimitType.PermissionGame, LimitId = (int)FoxGameName.KillGame, CurrentValue = 0, Thresholds = [1] },
    ];

        // When
        ICollection<UserLimitDto> result = _service.ApplyClosestThresholds(userLimits);

        // Then
        result.Should().HaveCount(6);
        result.ElementAt(0).ClosestThreshold.Should().Be(5);
        result.ElementAt(1).ClosestThreshold.Should().Be(8);
        result.ElementAt(2).ClosestThreshold.Should().Be(11);
        result.ElementAt(3).ClosestThreshold.Should().Be(0);
        result.ElementAt(4).ClosestThreshold.Should().Be(1);
        result.ElementAt(5).ClosestThreshold.Should().Be(1);
    }

    [Fact]
    public void ApplyClosestThresholds_ShouldHandleEmptyThresholds()
    {
        // Given
        List<UserLimitDto> userLimits =
    [
        new() { Type = LimitType.Achievement, LimitId = 1, CurrentValue = 3, Thresholds = [1, 5, 10] },
        new() { Type = LimitType.Achievement, LimitId = 2, CurrentValue = 2, Thresholds = [] }
    ];

        // When
        ICollection<UserLimitDto> result = _service.ApplyClosestThresholds(userLimits);

        // Then
        result.Should().HaveCount(2);
        result.ElementAt(0).ClosestThreshold.Should().Be(5);
        result.ElementAt(1).ClosestThreshold.Should().Be(0);
    }

    [Fact]
    public void ApplyClosestThresholds_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        // When
        ICollection<UserLimitDto> result = _service.ApplyClosestThresholds([]);

        // Then
        result.Should().HaveCount(0);
    }

    [Fact]
    public void CreateDefaultLimitsForUser_ShouldReturnAllDefaultLimits()
    {
        // Given
        int userId = 42;

        // When
        ICollection<UserLimit> result = _service.CreateDefaultLimitsForUser(userId);

        // Then
        result.Should().NotBeNull();
        result.Should().HaveCount(5); // 3 basic info + 2 fox games
        result.Should().OnlyHaveUniqueItems(l => new { l.Type, l.LimitId });
        result.All(l => l.UserId == userId).Should().BeTrue();
    }
}