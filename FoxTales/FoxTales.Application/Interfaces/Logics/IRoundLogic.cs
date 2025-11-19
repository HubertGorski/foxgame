using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Interfaces.Logics;

public interface IRoundLogic
{
    QuestionDto GetNewCurrentQuestionWithSelectedPlayer(RoomDto room);
    void UpdateVotePool(PlayerDto voter, PlayerDto owner);
    void AssignPoints(RoomDto room, PlayerDto answerOwner);
}
