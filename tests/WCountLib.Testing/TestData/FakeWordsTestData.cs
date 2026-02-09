/*namespace WCountLib.Testing.TestData;

public class FakeWordsTestData : IEnumerable<object[]>
{
    private readonly Faker _faker = new();
    public IEnumerator<object[]> GetEnumerator()
    {
        IList<string> fakeWords = _faker.Make(30, () => _faker.Random.);
        
        foreach (string s in fakeWords)
        {
            yield return new object[] { s };
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}*/