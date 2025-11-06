using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.Interfaces.Logics;

namespace FoxTales.Application.Services.Logics;

public class RoomLogic : IRoomLogic
{
    public bool IsTeamSetupValid(RoomDto room)
    {
        if (!room.IsTeamModeEnabled)
            return true;

        return room.Users?.All(u => u.TeamId != null) == true;
    }
}
