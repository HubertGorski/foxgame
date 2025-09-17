using System.Collections.ObjectModel;
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Domain.Enums;

namespace FoxTales.Application.Tests.Common;

public abstract class BaseTest
{
    protected const string GameCode = "PIESEK1";
    protected const string AnotherGameCode = "MIASTO69";
    protected const string OwnerName = "Natka";
    protected const string OwnerConnectionId = "1";
    protected const int OwnerId = 1;
    protected const string UserName = "Hubi";
    protected const string UserConnectionId = "2";
    protected const int UserId = 2;
    protected const string UserName_2 = "Adam";
    protected const string UserConnectionId_2 = "3";
    protected const int UserId_2 = 3;
    protected const string UserName_3 = "Oliwia";
    protected const string UserConnectionId_3 = "4";
    protected const int UserId_3 = 4;
    protected static readonly ReadOnlyDictionary<string, QuestionDto> Library = new(new Dictionary<string, QuestionDto>
    {
        { "ownerQuestion", new() { Text = "Example owner question", Language = Language.EN, OwnerId = OwnerId } },
        { "ownerQuestion_2", new() { Text = "Example owner question 2", Language = Language.EN, OwnerId = OwnerId } },
        { "userQuestion", new() { Text = "Example user question", Language = Language.EN, OwnerId = UserId } },
        { "userQuestion_2", new() { Text = "Example user question 2", Language = Language.EN, OwnerId = UserId } },
        { "publicQuestion", new() { Text = "Example public question", Language = Language.EN, IsPublic = true } },
        { "publicQuestion_2", new() { Text = "Example public question 2", Language = Language.EN, IsPublic = true } }
    });

    protected static PlayerDto CreateTestPlayer(int userId, string username, string? connectionId)
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

    protected static RoomDto CreateTestRoom(string? code = GameCode, int ownerId = OwnerId, string ownerName = OwnerName, string? ownerConnectionId = OwnerConnectionId, List<PlayerDto>? users = null)
    {
        PlayerDto owner = CreateTestPlayer(ownerId, ownerName, ownerConnectionId);
        return new RoomDto
        {
            Code = code,
            Owner = owner,
            Users = users ?? [owner]
        };
    }

    protected static AnswerDto CreateTestAnswer(int? ownerId = null)
    {
        AnswerDto answer = new()
        {
            Answer = "Example answer",
        };

        if (ownerId != null)
            answer.OwnerId = (int)ownerId;

        return answer;
    }

    protected static QuestionDto CreateTestQuestion(int ownerId = UserId, string text = "Example **** question", Language language = Language.EN)
    {
        return new()
        {
            Text = text,
            Language = language,
            OwnerId = ownerId
        };
    }
}
