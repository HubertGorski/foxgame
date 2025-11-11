using DotNetEnv;
using FoxTales.Application.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FoxTales.Infrastructure.Configuration;

public static class EnvProvider
{
    public static string GetEnv(IConfiguration configuration, string name)
    {
        bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (isDocker)
        {
            return configuration.GetValue<string>(name) ?? throw new ConfigException("Missing key");

        }
        else
        {
            Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName, ".env"));
            return Environment.GetEnvironmentVariable(name) ?? throw new ConfigException("Missing key");
        }
    }
}
