namespace WCountLib.Testing.Logic;

/// <summary>
/// Pins wc-token word semantics through the engine entry: word counting is token
/// counting (whitespace-separated runs) with no pluggable word-definition seam,
/// per ADR-0001 and the counter fold-in (T018).
/// </summary>
public class WordCountContractTests
{
    private static CountRequest WordsOnly => new(Words: true, Lines: false, Bytes: false, Characters: false);

    private static async Task<long> CountWordsAsync(string text)
    {
        using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        CountResult result = await new CountingEngine().CountAsync(stream, WordsOnly);
        return result.Words;
    }

    [Test]
    public async Task WhitespaceSeparatedTokens_CountedAsWords()
    {
        await Assert.That(await CountWordsAsync("hello world")).IsEqualTo(2L);
    }

    [Test]
    public async Task PunctuationOnlyToken_CountsAsOneWord()
    {
        // wc counts any non-whitespace run as a word; "---" is one token.
        await Assert.That(await CountWordsAsync("---")).IsEqualTo(1L);
    }

    [Test]
    public async Task ApostropheToken_CountsAsOneWord()
    {
        await Assert.That(await CountWordsAsync("don't!")).IsEqualTo(1L);
    }

    [Test]
    public async Task CommaSeparated_CountsAsOneWord()
    {
        await Assert.That(await CountWordsAsync("a,b,c")).IsEqualTo(1L);
    }

    [Test]
    public async Task MultipleSpaces_Collapsed()
    {
        await Assert.That(await CountWordsAsync("multiple   spaces   here")).IsEqualTo(3L);
    }

    [Test]
    public async Task EmptyString_IsZero()
    {
        await Assert.That(await CountWordsAsync("")).IsEqualTo(0L);
    }

    [Test]
    public async Task WhitespaceOnly_IsZero()
    {
        await Assert.That(await CountWordsAsync("   ")).IsEqualTo(0L);
    }

    [Test]
    public async Task TabSeparated_CountedAsWords()
    {
        await Assert.That(await CountWordsAsync("tab\tseparated")).IsEqualTo(2L);
    }

    [Test]
    [MethodDataSource<RealWordsTestData>(nameof(RealWordsTestData.GetAllData))]
    public async Task RealWordRuns_CountedAsTokens(string words, int expected)
    {
        await Assert.That(await CountWordsAsync(words)).IsEqualTo((long)expected);
    }
}
