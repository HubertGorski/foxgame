using FoxTales.Application.Interfaces;
using FoxTales.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;

namespace FoxTales.Infrastructure.Authentication;

public class GeminiSettings(IConfiguration configuration) : IGeminiSettings
{
    public string ApiKey { get; } = EnvProvider.GetEnv(configuration, "GEMINI_API_KEY");
}
