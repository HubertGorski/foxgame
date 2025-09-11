using FoxTales.Api.Enums;
using FoxTales.Api.Hubs;
using FoxTales.Application.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace FoxTales.Api.Handlers;

public class RefreshPublicRoomsListHandler(IHubContext<PsychHub> hubContext) : INotificationHandler<RefreshPublicRoomsListEvent>
{
    private readonly IHubContext<PsychHub> _hubContext = hubContext;

    public async Task Handle(RefreshPublicRoomsListEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group(CommonGroup.JOIN_GAME_VIEW.ToString()).SendAsync("GetPublicRooms", notification.PublicRooms, cancellationToken: cancellationToken);
    }
}
