using DotNetEnv;
using Microsoft.Extensions.Configuration;

namespace FoxTales.Infrastructure.Configuration;

public static class ConnectionStringProvider
{
    public static string GetConnectionString(IConfiguration configuration)
    {
        bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (isDocker)
        {
            return configuration.GetValue<string>("ConnectionStrings:DefaultConnection") ?? "";
        }
        else
        {
            Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName, ".env"));
            return Environment.GetEnvironmentVariable("ConnectionStringLocal") ?? "";
        }
    }
}
