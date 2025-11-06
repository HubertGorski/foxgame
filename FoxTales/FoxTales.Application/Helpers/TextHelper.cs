using System.Globalization;

namespace FoxTales.Application.Helpers;

public class TextHelper
{
    public static string PreprocessAnswer(string answer)
    {
        return answer.Trim()
            .ToLowerInvariant()
            .Replace("\"", "")
            .Replace("'", "")
            .Replace("”", "")
            .Replace("“", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace("  ", " ")
            .Trim();
    }

    public static double CalculateLevenshteinSimilarity(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            return 0.0;

        int[,] distances = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++)
            distances[i, 0] = i;

        for (int j = 0; j <= b.Length; j++)
            distances[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;

                distances[i, j] = Math.Min(
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost);
            }
        }

        int maxLength = Math.Max(a.Length, b.Length);
        if (maxLength == 0)
            return 1.0;

        return 1.0 - (double)distances[a.Length, b.Length] / maxLength;
    }

    public static bool TryParseAndCompareNumbers(string a, string b, out bool areEqual)
    {
        areEqual = false;
        if (double.TryParse(a, NumberStyles.Any, CultureInfo.InvariantCulture, out double num1) &&
            double.TryParse(b, NumberStyles.Any, CultureInfo.InvariantCulture, out double num2))
        {
            areEqual = Math.Abs(num1 - num2) < 0.0001;
            return true;
        }

        return false;
    }

    public static string RemoveArticlesAndPrepositions(string text)
    {
        var wordsToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
        // English
        "a", "an", "the", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by", "as", "from",
        
        // Polish
        "i", "oraz", "a", "ale", "lecz", "czy", "ani", "niż", "w", "we", "z", "ze", "o", "obok", "pod", "nad",
        "przed", "za", "między", "przez", "na", "po", "do", "od", "ku", "dla", "bez", "przy", "u", "o", "wobec",
        "naprzeciw", "pomimo", "mimo", "spod", "znad", "spośród", "według", "wzdłuż", "wśród", "wbrew", "blisko",
        "daleko", "blisko", "wokół", "dookoła", "około", "wciągu", "podczas", "wczasie", "wraz", "łącznie", "oprócz",
        "zamiast", "jako", "nic", "coś", "ktoś", "jakiś", "czyjś", "gdzieś", "kiedyś", "jakoś", "dlaczego", "czemu",
        "ten", "ta", "to", "ci", "te", "tamten", "tamta", "tamto", "ów", "owa", "owo", "jaki", "jaka", "jakie",
        "którzy", "które", "która", "czyj", "czyja", "czyje", "ile", "który", "jeden", "dwa", "trzy", "kilka",
        
        // German
        "der", "die", "das", "dem", "den", "des", "ein", "eine", "einer", "einem", "einen", "und", "oder", "aber",
        "in", "an", "auf", "aus", "bei", "mit", "nach", "von", "zu", "durch", "für", "gegen", "um", "bis", "ohne",
        "aus", "außer", "bei", "entlang", "gegenüber", "hinter", "in", "neben", "über", "unter", "vor", "zwischen",
        "während", "trotz", "wegen", "statt", "anstatt", "als", "wie", "weil", "da", "obwohl", "wenn", "falls",
        "damit", "dass", "bis", "seit", "bevor", "nachdem", "sobald", "solange", "soweit", "sowohl", "als auch",
        "weder", "noch", "entweder", "oder", "nicht", "kein", "keine", "keiner", "etwas", "nichts", "jemand", "niemand",
        "alle", "manche", "viele", "wenige", "einige", "jeder", "jede", "jedes", "dieser", "diese", "dieses", "jener",
        "jene", "jenes", "welcher", "welche", "welches", "irgendein", "irgendwelche", "irgendwas", "irgendwer"
    };

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var filteredWords = words.Where(word => !wordsToRemove.Contains(word));

        return string.Join(" ", filteredWords);
    }

    public static bool AreAcronymsSimilar(string a, string b)
    {
        if (a.Length <= 4 && b.Length <= 4 && a.Length == b.Length)
        {
            int differences = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    differences++;

                if (differences > 1)
                    return false;
            }
            return true;
        }

        return false;
    }
}
