namespace FoxTales.Application.Helpers;

public static class RoomCodeGenerator
{
    private static readonly Random _random = new();
    private static readonly string[] _words =
    {
        "PIES", "KOT", "DOM", "BURZA", "SUM", "LAS", "WODA",
        "OGNIA", "MIASTO", "AUTO", "GORA", "MORZE", "NOC", "DZIEN"
    };

    public static string GenerateUniqueCode(Func<string, bool> isCodeUnique)
    {
        string code;
        int attempts = 0;
        const int maxAttempts = 50;

        do
        {
            code = GenerateCode();
            attempts++;

            if (attempts >= maxAttempts)
            {
                throw new InvalidOperationException("Nie można wygenerować unikalnego kodu");
            }

        } while (!isCodeUnique(code));

        return code;
    }

    private static string GenerateCode()
    {
        var word = _words[_random.Next(_words.Length)];
        var number = _random.Next(1, 100);

        return $"{word}{number}";
    }
}