using System.Net;
using FoxTales.Application.Exceptions;

namespace FoxTales.Api.Middleware
{
    public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NotFoundException e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                _logger.LogError(e, e.Message);
                await context.Response.WriteAsync(e.Message);
            }
            catch (UnauthorizedException e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                _logger.LogError(e, e.Message);
                await context.Response.WriteAsync(e.Message);
            }
            catch (ConflictException e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                _logger.LogWarning(e, e.Message);
                await context.Response.WriteAsync(e.Message);
            }
            catch (ConfigException e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogWarning(e, e.Message);
                await context.Response.WriteAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}