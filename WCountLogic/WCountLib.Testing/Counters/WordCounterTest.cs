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
    public void CountWords(string words, int expected)
    {
        int actual = _counter.CountWords(words);
        
        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(FakeWordsTestData))]
    public void DontCountFakeWords(string words)
    {
        int actual = _counter.CountWords(words);
        
        Assert.Equal(0, actual);
    }   
}