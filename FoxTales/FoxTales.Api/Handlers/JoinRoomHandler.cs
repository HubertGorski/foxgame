using FoxTales.Api.Enums;
using FoxTales.Api.Hubs;
using FoxTales.Application.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace FoxTales.Api.Handlers;

public class JoinRoomHandler(IHubContext<PsychHub> hubContext) : INotificationHandler<JoinRoomEvent>
{
    private readonly IHubContext<PsychHub> _hubContext = hubContext;

    public async Task Handle(JoinRoomEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Groups.AddToGroupAsync(notification.ConnectionId, notification.Code, cancellationToken);
        await _hubContext.Groups.RemoveFromGroupAsync(notification.ConnectionId, CommonGroup.JOIN_GAME_VIEW.ToString(), cancellationToken);
    }
}
