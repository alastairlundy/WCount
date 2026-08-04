namespace WCountLib.Testing.TestData;

public class EscapeCharactersTestData : IEnumerable<string>
{
    private static readonly string[] EscapeCharacters = [
        "\a", "\b", "\f", "\n", "\r", "\t", "\v",
        "\\", "\0", "\'", "\""
    ];

    public IEnumerator<string> GetEnumerator()
    {
        foreach (string s in EscapeCharacters)
        {
            yield return s;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}