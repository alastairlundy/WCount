using System;

namespace WCountLib.Testing.TestData;

public class RealWordsTestData : IEnumerable<object[]>
{
    private static readonly string[] WordPool =
    [
        "the", "quick", "brown", "fox", "jumps", "over", "lazy", "dog",
        "hello", "world", "foo", "bar", "baz", "lorem", "ipsum", "dolor",
        "sit", "amet", "consectetur", "adipiscing", "elit", "sed", "do",
        "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore",
        "magna", "aliqua", "enim", "ad", "minim", "veniam", "quis",
        "nostrud", "exercitation", "ullamco", "laboris", "nisi", "aliquip",
        "ex", "ea", "commodo", "consequat", "duis", "aute", "irure",
        "in", "reprehenderit", "voluptate", "velit", "esse", "cillum",
        "fugiat", "nulla", "pariatur", "excepteur", "sint", "occaecat",
        "cupidatat", "non", "proident", "sunt", "culpa", "qui", "officia",
        "deserunt", "mollit", "anim", "id", "est", "laborum"
    ];

    public IEnumerator<object[]> GetEnumerator()
    {
        for (int i = 0; i < 10; i++)
        {
            int count = Random.Shared.Next(2, 50);
            string[] picked = new string[count];
            for (int j = 0; j < count; j++)
                picked[j] = WordPool[Random.Shared.Next(WordPool.Length)];

            string words = string.Join(' ', picked);
            yield return new object[] { words, count };
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}