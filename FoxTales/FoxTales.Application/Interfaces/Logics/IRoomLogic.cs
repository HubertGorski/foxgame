using FoxTales.Application.DTOs.Psych;

namespace FoxTales.Application.Interfaces.Logics;

public interface IRoomLogic
{
    bool IsTeamSetupValid(RoomDto room);
}
