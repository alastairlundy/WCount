using System;
using System.Collections;
using System.Collections.Generic;
using AlastairLundy.Extensions.System.Strings;
using Bogus.DataSets;

namespace WCountLib.Testing.TestData;

public class FakeWordsTestData : IEnumerable<object[]>
{
    private readonly Lorem _lorem = new Lorem();
    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (char c in CharacterConstants.SpecialCharacters)
        {
            yield return new object[] { c.ToString() };
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}