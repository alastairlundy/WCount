using System;

namespace WCountLib.Testing.TestData;

public class RealWordsTestData : IEnumerable<object[]>
{
    private readonly Lorem _lorem = new Lorem();
    
    public IEnumerator<object[]> GetEnumerator()
    {
        for (int i = 0; i < 10; i++)
        {
            string words = string.Join(' ', _lorem.Words(Random.Shared.Next(2, 50)));
            
            yield return new object[] {words, words.Split(' ').Length};
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}