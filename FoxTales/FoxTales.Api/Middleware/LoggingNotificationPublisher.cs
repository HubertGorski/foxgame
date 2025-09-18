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

        var payload = JsonConvert.SerializeObject(notification, Formatting.None);

        _logger.LogInformation(
            "Publishing event {EventName} with payload {Payload}", 
            eventName, 
            payload
        );

        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(notification, cancellationToken);
        }
    }
}
