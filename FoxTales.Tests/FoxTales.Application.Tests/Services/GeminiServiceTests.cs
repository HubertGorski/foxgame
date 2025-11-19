using FluentAssertions;
using FoxTales.Application.Interfaces;
using FoxTales.Application.Services;
using Moq;
using DotNetEnv;

namespace FoxTales.Application.Tests.Services;

public class GeminiServiceTests
{
    private readonly GeminiService _geminiService;
    private readonly Mock<IGeminiSettings> _settingsMock;

    public GeminiServiceTests()
    {
        Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName, ".env")); // TODO: refactor parents
        string? testApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        var httpClient = new HttpClient();
        _settingsMock = new Mock<IGeminiSettings>();
        _settingsMock.Setup(s => s.ApiKey).Returns(testApiKey ?? "");
        _geminiService = new GeminiService(httpClient, _settingsMock.Object);
    }

    [Theory(Skip = "TODO: add new game type")]
    [
        InlineData("latwo", "lato", false),
        InlineData("okno", "okoń", false),
        InlineData("zima", "lato", false),
        InlineData("u Heńka", "heniek", true),
        InlineData("w sklepie", "w markecie", false)
    ]
    public async Task Test_test(string answer1, string answer2, bool expectedResult)
    {
        // GIVEN
        var prompt = @"
            Twoim zadaniem jest porównać dwa słowa lub frazy. Odpowiedz wyłącznie 'true' lub 'false'. Nic więcej nie dodawaj.

            Zasady:
            1. Jeśli słowa lub frazy są 'dokładnie takie same', ignorując wielkość liter i polskie znaki (np. 'NieBieski' = 'niebieski'), odpowiedź to 'true'.
            2. Jeśli występują 'drobne literówki', które nie zmieniają znaczenia słowa i nie tworzą innego słowa (np. brak jednej litery w środku lub literówka typu 'niebieski' vs 'niebiesk'), odpowiedź to 'true'.
            3. Jeśli jedno słowo lub fraza jest 'zawarta' w drugim (np. 'pod stołem' w 'u Henryka pod stołem w kuchni'), odpowiedź to 'true'.
            4. Jeśli słowa lub frazy są 'zupełnie różne' lub literówki zmieniają znaczenie słowa (np. 'kot' vs 'koc', 'niebieski' vs 'czerwony'), odpowiedź to 'false'.
            5. Jeśli słowa są podobne wizualnie lub fonetycznie, ale mają różne znaczenie (np. 'latwo' vs 'lato'), odpowiedź to 'false'.
            6. Nie interpretuj kontekstu ani kategorii — liczy się tylko 'czy słowa/frazy są takie same, zawierają się w sobie lub różnią się tylko drobną literówką'.
            7. Ignoruj spacje na początku i końcu słów/fraz.

            Przykłady:
            - 'niebieski', 'niebieski' → true
            - 'niebieski', 'niebiesk' → true
            - 'NieBieski', 'niebieski' → true
            - 'niebieski', 'czerwony' → false
            - 'kot', 'koc' → false
            - 'kot', 'kot' → true
            - 'pod stołem', 'u Henryka pod stołem w kuchni' → true
            - 'latwo', 'lato' → false";

        // WHEN
        var test = await _geminiService.Test(prompt + ". Teraz oceń: '" + answer1 + "', '" + answer2 + "'");
        bool result = bool.Parse(test);

        // THEN
        result.Should().Be(expectedResult);
    }
}