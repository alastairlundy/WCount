using System.Collections;
using System.Collections.Generic;

using AlastairLundy.DotExtensions.Strings;

namespace WCountLib.Testing.TestData;

public class EscapeCharactersTestData : IEnumerable<string>
{
    public IEnumerator<string> GetEnumerator()
    {
        foreach (string s in CharacterConstants.EscapeCharacters)
        {
            yield return s;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}