using System.Text;
using System.Text.Json;
using FoxTales.Application.Interfaces;

namespace FoxTales.Application.Services;

public class GeminiService(HttpClient httpClient, IGeminiSettings geminiSettings)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = geminiSettings.ApiKey;

    public async Task<string> Test(string text)
    {
        var requestBody = new
        {
            contents = new[]
            {
                new {
                    role = "user",
                    parts = new[] { new { text } }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={_apiKey}";

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(resultJson);
        var reply = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return reply!;
    }
}
