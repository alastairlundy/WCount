using System.Collections;
using System.Collections.Generic;
using AlastairLundy.DotExtensions.Strings;

using Bogus;

namespace WCountLib.Testing.TestData;

public class SpecialCharactersTestData : IEnumerable<char>
{
    public IEnumerator<char> GetEnumerator()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return CharacterConstants.SpecialCharacters[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}