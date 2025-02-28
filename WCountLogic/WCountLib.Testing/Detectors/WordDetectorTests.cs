using System;
using System.Linq;

using AlastairLundy.Extensions.System.Strings;

using Bogus;
using Bogus.DataSets;

using WCountLib.Detectors;
using WCountLib.Testing.TestData;

namespace WCountLib.Testing.Detectors;

public class WordDetectorTests
{
    private readonly WordDetector _detector = new();
    private readonly Lorem lorem = new Lorem();
    private Randomizer _randomizer = new();

    private char[] _specialChars = CharacterConstants.SpecialCharacters;

    [Theory]
    [ClassData(typeof(FakeWordsTestData))]
    public void FakeWordDetection(string fakeWord)
    {
        bool actual = _detector.IsStringAWord(fakeWord);
        
        Assert.False(actual);
    }
    
    [Theory]
    [ClassData(typeof(NormalCharactersTestData))]
    private void TestRealWord(string realWord)
    {
        bool actual = _detector.IsStringAWord(realWord);
        
        Assert.True(actual);
    }
}