using FoxTales.Api.Hubs;
using FoxTales.Application.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace FoxTales.Api.Handlers;

public class ReceiveErrorHandler(IHubContext<PsychHub> hubContext) : INotificationHandler<ReceiveErrorEvent>
{
    private readonly IHubContext<PsychHub> _hubContext = hubContext;

    public async Task Handle(ReceiveErrorEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Client(notification.ConnectionId).SendAsync("ReceiveError", new { message = notification.Message, notification.FieldId });
    }
}