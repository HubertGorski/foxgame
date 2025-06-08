using NLog;
using NLog.Web;

namespace FoxTales.Api.Platform;

public static class LoggingConfigurator
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        LogManager.Configuration.Variables["logDir"] = builder.Configuration["Logging:LogDir"];
        LogManager.Setup().LoadConfigurationFromFile("NLog.config");

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        builder.Host.UseNLog();
        return builder;
    }
}
