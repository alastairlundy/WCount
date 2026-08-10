namespace WCountLib.Testing.TestData;

public static class EscapeCharactersTestData
{
    private static readonly string[] EscapeCharacters = [
        "\a", "\b", "\f", "\n", "\r", "\t", "\v",
        "\\", "\0", "\'", "\""
    ];

    public static IEnumerable<string> GetAllData()
    {
        foreach (string s in EscapeCharacters)
        {
            yield return s;
        }
    }
}