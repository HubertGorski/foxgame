using FoxTales.Api.Enums;
using FoxTales.Api.Hubs;
using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace FoxTales.Api.Handlers;

public class RoomClosedHandler(IHubContext<PsychHub> hubContext) : INotificationHandler<RoomClosedEvent>
{
    private readonly IHubContext<PsychHub> _hubContext = hubContext;

    public async Task Handle(RoomClosedEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group(notification.Code).SendAsync("RoomClosed", cancellationToken);
        foreach (PlayerDto player in notification.PlayersInRoom)
        {
            if (player.ConnectionId != null)
                await _hubContext.Groups.RemoveFromGroupAsync(player.ConnectionId, notification.Code, cancellationToken);
        }
    }
}
