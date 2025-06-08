using System.Diagnostics;

namespace FoxTales.Api.Middleware;

public class RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger) : IMiddleware
{
    private readonly ILogger<RequestTimeMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next.Invoke(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            bool isGreaterThan4Seconds = elapsedMilliseconds / 1000 > 4;
            if (isGreaterThan4Seconds)
            {
                _logger.LogWarning("Request [{Method}] at {Path} took {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMilliseconds);
            }
        }
    }
}