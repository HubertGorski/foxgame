using FoxTales.Api.Hubs;
using FoxTales.Application.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace FoxTales.Api.Handlers;

public class PlayerLeftRoomHandler(IHubContext<PsychHub> hubContext) : INotificationHandler<PlayerLeftRoomEvent>
{
    private readonly IHubContext<PsychHub> _hubContext = hubContext;

    public async Task Handle(PlayerLeftRoomEvent notification, CancellationToken cancellationToken)
    {
        if (notification.PlayerToRemove.ConnectionId == null)
            throw new InvalidOperationException($"Player {notification.PlayerToRemove.UserId} not found in room '{notification.Code}' (PlayerLeft)");

        await _hubContext.Groups.RemoveFromGroupAsync(notification.PlayerToRemove.ConnectionId, notification.Code, cancellationToken);
        await _hubContext.Clients.Group(notification.Code).SendAsync("PlayerLeft", notification.PlayerToRemove.UserId, cancellationToken: cancellationToken);
    }
}
