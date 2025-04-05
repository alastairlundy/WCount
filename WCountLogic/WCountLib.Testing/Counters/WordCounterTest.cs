using System.IO;
using AlastairLundy.WCountLib.Counters;
using AlastairLundy.WCountLib.Detectors;

using WCountLib.Testing.TestData;

namespace WCountLib.Testing.Counters;

public class WordCounterTest
{
    private readonly WordCounter _counter = new(new WordDetector());

    [Theory]
    [ClassData(typeof(RealWordsTestData))]
    public void CountWords(string words, ulong expected)
    {
        ulong actual = _counter.CountWords(new StringReader(words));
        
        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(FakeWordsTestData))]
    public void DontCountFakeWords(string words)
    {
        ulong actual = _counter.CountWords(new StringReader(words));
        
        Assert.Equal(ulong.Parse("0"), actual);
    }   
}