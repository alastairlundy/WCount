using System.Collections;
using System.Collections.Generic;

using Bogus;

namespace WCountLib.Testing.TestData;

public class NormalCharactersTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (char c in Chars.UpperCase)
        {
            yield return new object[] { c };
        }

        foreach (char c in Chars.LowerCase)
        {
            yield return new object[] { c };
        }

        foreach (char c in Chars.Numbers)
        {
            yield return new object[] { c };
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}