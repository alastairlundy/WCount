using WCountLib.Counters;
using System.Threading.Tasks;

namespace WCountLib.Testing.Counters;

public class WordCounterTest
{
    private readonly WordCounter _counter = new(new WordDetector());

    [Test]
    [ClassDataSource(typeof(RealWordsTestData))]
    public async Task CountWords(string words, int expected)
    {
        int actual = _counter.CountWords(words);
        
        await Assert.That(actual).IsEqualTo(expected);
    }

    /*[Theory]
    [ClassData(typeof(FakeWordsTestData))]
    public void DontCountFakeWords(string words)
    {
        int actual = _counter.CountWords(words);
        
        Assert.Equal(0, actual);
    }  */ 
}