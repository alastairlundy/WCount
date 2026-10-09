namespace WCountLib.Testing.Logic;

public class MaximumLineLengthTests
{
    // The engine decodes into 16384-char chunks, so inputs beyond this exercise
    // state carried across decoded chunks.
    private const int ChunkSize = 16384;

    private readonly CountingEngine _engine = new();

    private static CountRequest MaximumLineLengthOnly => new(false, false, false, false, true);

    private static CountRequest WordsOnly => new(Words: true, Lines: false, Bytes: false, Characters: false);

    private static CancellationToken Ct => CancellationToken.None;

    private static Stream Utf8(string text) => new MemoryStream(Encoding.UTF8.GetBytes(text));

    [Test]
    public async Task Unrequested_ReturnsMinusOne()
    {
        using Stream stream = Utf8("hello world");
        CountResult result = await _engine.CountAsync(stream, WordsOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(-1L);
    }

    [Test]
    public async Task RequestedOnEmptyInput_ReturnsZero()
    {
        using Stream stream = Utf8(string.Empty);
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(0L);
    }

    [Test]
    public async Task LongestLineWins()
    {
        using Stream stream = Utf8("ab\ncdefg\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(5L);
    }

    [Test]
    public async Task UnterminatedFinalLine_Counts()
    {
        using Stream stream = Utf8("ab\ncdefg");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(5L);
    }

    [Test]
    public async Task TabAdvancesToNextTabStop()
    {
        // Pinned against GNU coreutils wc -L: tab advances to the next multiple of 8.
        using Stream stream = Utf8("a\tb\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(9L);
    }

    [Test]
    public async Task TabAtColumnSeven_AdvancesExactlyOne()
    {
        // Pinned against GNU coreutils wc -L.
        using Stream stream = Utf8("abcdefg\th\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(9L);
    }

    [Test]
    public async Task TabOnlyLine_AdvancesToEight()
    {
        // Pinned against GNU coreutils wc -L.
        using Stream stream = Utf8("\t\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(8L);
    }

    [Test]
    public async Task TabMidLine_ExpandsFromItsColumn()
    {
        // Pinned against GNU coreutils wc -L: the tab expands from its own column.
        using Stream stream = Utf8("0123456789\tx\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(17L);
    }

    [Test]
    public async Task EastAsianWideChars_CountAsTwo()
    {
        // Pinned against GNU coreutils wc -L: CJK ideographs are two columns wide.
        using Stream stream = Utf8("你好世界\nabc\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(8L);
    }

    [Test]
    public async Task MixedWideAndNarrow()
    {
        // Pinned against GNU coreutils wc -L.
        using Stream stream = Utf8("a你b\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(4L);
    }

    [Test]
    public async Task CombiningMark_CountsZero()
    {
        // Pinned against GNU coreutils wc -L: a combining mark adds no width, so
        // "e" + U+0301 + "xy" is three columns.
        using Stream stream = Utf8("e\u0301xy\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(3L);
    }

    [Test]
    public async Task CarriageReturn_EndsTheLengthLine()
    {
        using Stream stream = Utf8("abcdefg\r\r\rhi\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(7L);
    }

    [Test]
    public async Task FormFeed_EndsTheLengthLine()
    {
        using Stream stream = Utf8("abcdef\ffgh\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(6L);
    }

    [Test]
    public async Task VerticalTab_CountsZero()
    {
        using Stream stream = Utf8("ab\vc\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(3L);
    }

    [Test]
    public async Task ControlCharacters_CountZero()
    {
        // Pinned against GNU coreutils wc -L: control characters add no width.
        using Stream first = Utf8("abc\adef\n");
        CountResult firstResult = await _engine.CountAsync(first, MaximumLineLengthOnly, Ct);

        using Stream second = Utf8("ab\u0001c\n");
        CountResult secondResult = await _engine.CountAsync(second, MaximumLineLengthOnly, Ct);

        await Assert.That(firstResult.MaximumLineLength).IsEqualTo(6L);
        await Assert.That(secondResult.MaximumLineLength).IsEqualTo(3L);
    }

    [Test]
    public async Task CrLfPair_DoesNotInflateLength()
    {
        using Stream stream = Utf8("ab\r\ncdefg\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(5L);
    }

    [Test]
    public async Task Emoji_CountsAsTwo()
    {
        // Emoji are Wide (width 2): U+1F600 + "abc" = 5 columns. The Git for Windows
        // wc.exe reports byte counts here (non-UTF-8 locale), so this is pinned to the
        // Unicode East Asian Width / glibc wcwidth result the package encodes.
        using Stream stream = Utf8("\U0001F600abc\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(5L);
    }

    [Test]
    public async Task NarrowEmojiBlockStart_CountsAsOne()
    {
        // U+1F900..U+1F90B are Narrow per the official EastAsianWidth table (verified
        // against Unicode 16: U+1F900 wcwidth=1, U+1F90C onward are Wide), and the
        // Wcwidth package's data pins this where a naive "all emoji are wide" rule
        // would wrongly report two. Input "\U0001F900ab" is therefore 1+1+1 = 3.
        using Stream stream = Utf8("\U0001F900ab\n");
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(3L);
    }

    [Test]
    public async Task LineLongerThanBuffer_CarriesAcrossChunks()
    {
        string input = new string('a', ChunkSize) + "\nz\n";
        using Stream stream = Utf8(input);
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        // The longest line is exactly ChunkSize 'a's wide; the trailing "z" line is 1.
        // Verified against GNU wc -L: 16384.
        await Assert.That(result.MaximumLineLength).IsEqualTo(16384L);
    }

    [Test]
    public async Task TabStraddlingChunkBoundary_ExpandsFromCarriedColumn()
    {
        // 16384 is divisible by 8, so the tab adds 8 and the 'z' adds 1.
        string input = new string('a', ChunkSize) + "\tz\n";
        using Stream stream = Utf8(input);
        CountResult result = await _engine.CountAsync(stream, MaximumLineLengthOnly, Ct);

        await Assert.That(result.MaximumLineLength).IsEqualTo(16393L);
    }
}
