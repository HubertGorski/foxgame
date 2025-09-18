using FoxTales.Api.Hubs;
using FoxTales.Application.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace FoxTales.Api.Handlers;

public class RefreshRoomHandler(IHubContext<PsychHub> hubContext, ILogger<RefreshRoomHandler> logger) : INotificationHandler<RefreshRoomEvent>
{
    private readonly IHubContext<PsychHub> _hubContext = hubContext;
    private readonly ILogger<RefreshRoomHandler> _logger = logger;

    public async Task Handle(RefreshRoomEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hub=PsychHub Method=RefreshRoomEvent called with Args={Room}", System.Text.Json.JsonSerializer.Serialize(notification.Room));
        if (notification.Room.Code == null) throw new InvalidOperationException("Code is empty!");
        await _hubContext.Clients.Group(notification.Room.Code).SendAsync("LoadRoom", notification.Room, cancellationToken);
    }
}
