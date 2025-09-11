using FoxTales.Api.Hubs;
using FoxTales.Application.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace FoxTales.Api.Handlers;

public class RefreshRoomHandler(IHubContext<PsychHub> hubContext) : INotificationHandler<RefreshRoomEvent>
{
    private readonly IHubContext<PsychHub> _hubContext = hubContext;

    public async Task Handle(RefreshRoomEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Room.Code == null) throw new InvalidOperationException("Code is empty!");
        await _hubContext.Clients.Group(notification.Room.Code).SendAsync("LoadRoom", notification.Room, cancellationToken);
    }
}
