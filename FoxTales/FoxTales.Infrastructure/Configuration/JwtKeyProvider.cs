using DotNetEnv;
using FoxTales.Application.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FoxTales.Infrastructure.Configuration;

public static class JwtKeyProvider
{
    public static string GetJwtKey(IConfiguration configuration)
    {
        bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (isDocker)
        {
            return configuration.GetValue<string>("JWT_KEY") ?? throw new ConfigException("Missing JWT key (set via environment variable JWT_KEY)");

        }
        else
        {
            Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName, ".env"));
            return Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ConfigException("Missing JWT key");
        }
    }
}
