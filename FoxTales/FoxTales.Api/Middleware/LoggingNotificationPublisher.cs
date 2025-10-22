using MediatR;
using Newtonsoft.Json;

namespace FoxTales.Api.Middleware;

public class LoggingNotificationPublisher(ILogger<LoggingNotificationPublisher> logger) : INotificationPublisher
{
    private readonly ILogger<LoggingNotificationPublisher> _logger = logger;

    public async Task Publish(
        IEnumerable<NotificationHandlerExecutor> handlerExecutors,
        INotification notification,
        CancellationToken cancellationToken = default)
    {
        var eventName = notification.GetType().Name;

        string payload;

        if (eventName == "RefreshRoomEvent")
        {
            var roomProp = notification.GetType().GetProperty("Room");
            if (roomProp != null)
            {
                var roomValue = roomProp.GetValue(notification);
                var usersProp = roomValue?.GetType().GetProperty("Users");
                if (usersProp != null)
                {
                    var usersValue = usersProp.GetValue(roomValue);
                    payload = JsonConvert.SerializeObject(usersValue, Formatting.None);
                }
                else
                {
                    payload = "\"<Room.Users property not found>\"";
                }
            }
            else
            {
                payload = "\"<Room property not found>\"";
            }
        }
        else
        {
            payload = JsonConvert.SerializeObject(notification, Formatting.None);
        }

        _logger.LogInformation("Method = '{EventName}' with payload {Payload}", eventName, payload);

        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(notification, cancellationToken);
        }
    }
}
