/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlastairLundy.DotExtensions.Strings;
using Bogus;
using Bogus.DataSets;

namespace WCountLib.Testing.TestData;

public class FakeWordsTestData : IEnumerable<object[]>
{
    private readonly Faker _faker = new();
    public IEnumerator<object[]> GetEnumerator()
    {
    //   var currencies = _faker.Make(30, () => _faker.Internet.);
        
        foreach (string s in currencies)
        {
            yield return new object[] { s };
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}*/