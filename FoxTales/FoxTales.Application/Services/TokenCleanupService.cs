

using FoxTales.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FoxTales.Application.Services;

public class TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            try
            {
                await userService.CleanupInactiveTokens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TokenCleanupService]: {Message}", ex.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // TODO: z configa wziac
        }
    }
}
