using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace FoxTales.Api.Middleware;

public class LoggingHubFilter(ILogger<LoggingHubFilter> logger) : IHubFilter
{
    private readonly ILogger<LoggingHubFilter> _logger = logger;

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var hubName = invocationContext.Hub.GetType().Name;
        var methodName = invocationContext.HubMethodName;
        var argsJson = JsonSerializer.Serialize(invocationContext.HubMethodArguments);

        _logger.LogInformation(
            "Hub={Hub} Method = '{Method}' called with Args={ArgsJson} ",
            hubName,
            methodName,
            argsJson
        );

        return await next(invocationContext);
    }
}
