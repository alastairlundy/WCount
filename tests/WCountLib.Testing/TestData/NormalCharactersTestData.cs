namespace WCountLib.Testing.TestData;

public class NormalCharactersTestData : IEnumerable<char[]>
{
    public IEnumerator<char[]> GetEnumerator()
    {
        foreach (char c in Chars.UpperCase)
        {
            yield return [c];
        }

        foreach (char c in Chars.LowerCase)
        {
            yield return [c];
        }

        foreach (char c in Chars.Numbers)
        {
            yield return [c];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}