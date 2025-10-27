using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace FoxTales.Api.Filters;

public class DuplicateDetectionHubFilter(ILogger<DuplicateDetectionHubFilter> logger) : IHubFilter
{
    private readonly ILogger<DuplicateDetectionHubFilter> _logger = logger;
    private readonly ConcurrentDictionary<string, DateTime> _requestCache = new();
    private readonly TimeSpan _deduplicationWindow = TimeSpan.FromSeconds(2); // TODO: dac z configu

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var methodName = invocationContext.HubMethodName;
        var connectionId = invocationContext.Context.ConnectionId;
        var argsJson = JsonSerializer.Serialize(invocationContext.HubMethodArguments);
        var requestKey = $"{connectionId}:{methodName}:{argsJson}";

        var now = DateTime.UtcNow;

        if (_requestCache.TryGetValue(requestKey, out var lastCallTime) && now - lastCallTime < _deduplicationWindow)
        {
            var hubName = invocationContext.Hub.GetType().Name;
            _logger.LogInformation(
            "Duplicate request detected. Hub={Hub} Method = '{Method}' called with Args={ArgsJson} ", hubName, methodName, argsJson);
            return null;
        }

        _requestCache[requestKey] = now;
        return await next(invocationContext);
    }
}