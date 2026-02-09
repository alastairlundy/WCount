using System.Linq;

namespace WCountLib.Testing.TestData;

public class SpecialCharactersTestData : IEnumerable<char>
{
    private readonly Faker _faker = new();

    public IEnumerator<char> GetEnumerator()
    {
        var currencies = _faker.Make(30, () => _faker.Finance.Currency().Code.First());
        
        foreach (char c in currencies)
        {
            yield return c;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}