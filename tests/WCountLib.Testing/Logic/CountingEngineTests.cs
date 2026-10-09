namespace WCountLib.Testing.Logic;

public class CountingEngineTests
{
    private const int ChunkSize = 8192;

    private readonly CountingEngine _engine = new();

    private static CountRequest Everything => new(Words: true, Lines: true, Bytes: true, Characters: true);

    private static CountRequest WordsOnly => new(Words: true, Lines: false, Bytes: false, Characters: false);

    private static CountRequest WordsAndLines => new(Words: true, Lines: true, Bytes: false, Characters: false);

    private static CancellationToken Ct => CancellationToken.None;

    private static Stream Utf8(string text) => new MemoryStream(Encoding.UTF8.GetBytes(text));

    [Test]
    public async Task EmptyInput_ReturnsZeros()
    {
        using Stream stream = Utf8(string.Empty);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Words).IsEqualTo(0L);
        await Assert.That(result.Lines).IsEqualTo(0L);
        await Assert.That(result.Bytes).IsEqualTo(0L);
        await Assert.That(result.Characters).IsEqualTo(0L);
    }

    [Test]
    public async Task EmptyRequest_ReturnsMinusOneForEveryField()
    {
        using Stream stream = Utf8("hello world\n");
        CountResult result = await _engine.CountAsync(stream, new CountRequest(false, false, false, false), Ct);

        await Assert.That(result.Words).IsEqualTo(-1L);
        await Assert.That(result.Lines).IsEqualTo(-1L);
        await Assert.That(result.Bytes).IsEqualTo(-1L);
        await Assert.That(result.Characters).IsEqualTo(-1L);
        await Assert.That(result.MaximumLineLength).IsEqualTo(-1L);
    }

    [Test]
    public async Task LfOnly_LineCount()
    {
        using Stream stream = Utf8("line1\nline2\nline3\n");
        CountResult result = await _engine.CountAsync(stream, new CountRequest(false, true, false, false), Ct);

        await Assert.That(result.Lines).IsEqualTo(3L);
    }

    [Test]
    public async Task CrlfOnly_LineCount()
    {
        using Stream stream = Utf8("line1\r\nline2\r\nline3\r\n");
        CountResult result = await _engine.CountAsync(stream, new CountRequest(false, true, false, false), Ct);

        await Assert.That(result.Lines).IsEqualTo(3L);
    }

    [Test]
    public async Task CrlfStraddlingChunkBoundary_CountsAsOneLine()
    {
        // The '\r' lands on the last byte of chunk one and the '\n' on the first
        // byte of chunk two; the newline definition makes that one line.
        string input = new string('a', ChunkSize - 1) + "\r\n";
        using Stream stream = Utf8(input);
        CountResult result = await _engine.CountAsync(stream, new CountRequest(false, true, false, false), Ct);

        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task LoneCarriageReturn_IsNotCountedAsLine()
    {
        using Stream stream = Utf8("alpha\rbeta\r");
        CountResult result = await _engine.CountAsync(stream, new CountRequest(false, true, false, false), Ct);

        await Assert.That(result.Lines).IsEqualTo(0L);
    }

    [Test]
    public async Task NoTrailingNewline_IsNotCountedAsLine()
    {
        using Stream stream = Utf8("hello world");
        CountResult result = await _engine.CountAsync(stream, new CountRequest(false, true, false, false), Ct);

        await Assert.That(result.Lines).IsEqualTo(0L);
    }

    [Test]
    public async Task WordStraddlingChunkBoundary_CountsAsOneWord()
    {
        // A single unbroken word longer than one chunk: the continuation at the
        // start of the second chunk must not be counted as a second word.
        string input = new string('a', ChunkSize + 100);
        using Stream stream = Utf8(input);
        CountResult result = await _engine.CountAsync(stream, WordsOnly, Ct);

        await Assert.That(result.Words).IsEqualTo(1L);
    }

    [Test]
    public async Task WordsAroundChunkBoundary_CountedIndependently()
    {
        // "alpha" ends before the boundary, the padding word straddles it, "omega" starts after.
        string input = "alpha " + new string('x', ChunkSize) + " omega";
        using Stream stream = Utf8(input);
        CountResult result = await _engine.CountAsync(stream, WordsOnly, Ct);

        await Assert.That(result.Words).IsEqualTo(3L);
    }

    [Test]
    public async Task MultiByteCharacterStraddlingChunkBoundary_IsDecodedOnce()
    {
        // The two UTF-8 bytes of "é" straddle the chunk boundary: the decoder must
        // carry the first byte over, and the word state must carry the continuation.
        string input = new string('a', ChunkSize - 1) + "é tail";
        using Stream stream = Utf8(input);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Characters).IsEqualTo((long)input.Length);
        await Assert.That(result.Bytes).IsEqualTo((long)Encoding.UTF8.GetByteCount(input));
        await Assert.That(result.Words).IsEqualTo(2L);
    }

    [Test]
    public async Task SelectiveRequest_WordOnly_LeavesOtherCountsAtMinusOne()
    {
        using Stream stream = Utf8("hello world");
        CountResult result = await _engine.CountAsync(stream, WordsOnly, Ct);

        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(-1L);
        await Assert.That(result.Bytes).IsEqualTo(-1L);
        await Assert.That(result.Characters).IsEqualTo(-1L);
    }

    [Test]
    public async Task RawBytes_AreCountedFromTheStreamWithoutReEncoding()
    {
        const string input = "café crème\n";
        using Stream stream = Utf8(input);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo((long)Encoding.UTF8.GetByteCount(input));
        await Assert.That(result.Characters).IsEqualTo((long)input.Length);
        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task Utf8ByteOrderMark_IsCountedAsBytesButNotAsText()
    {
        byte[] payload = [0xEF, 0xBB, 0xBF, (byte)'h', (byte)'i'];
        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo(5L);
        await Assert.That(result.Characters).IsEqualTo(2L);
        await Assert.That(result.Words).IsEqualTo(1L);
        await Assert.That(result.Lines).IsEqualTo(0L);
    }

    [Test]
    public async Task Utf16LittleEndianByteOrderMark_IsDetectedInsideTheEngine()
    {
        byte[] text = Encoding.Unicode.GetBytes("hello world\n");
        byte[] payload = [0xFF, 0xFE, .. text];
        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo((long)payload.Length);
        await Assert.That(result.Characters).IsEqualTo(12L);
        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task Utf16BigEndianByteOrderMark_IsDetectedInsideTheEngine()
    {
        byte[] text = Encoding.BigEndianUnicode.GetBytes("hello world\n");
        byte[] payload = [0xFE, 0xFF, .. text];
        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo((long)payload.Length);
        await Assert.That(result.Characters).IsEqualTo(12L);
        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task ByteBackedFileStream_CountsRawBytes()
    {
        string path = Path.Combine(Path.GetTempPath(), $"wcount-engine-{Guid.NewGuid():N}.txt");
        try
        {
            byte[] payload = Encoding.UTF8.GetBytes("one two\nthree\n");
            await File.WriteAllBytesAsync(path, payload);

            await using FileStream stream = File.OpenRead(path);
            CountResult result = await _engine.CountAsync(stream, Everything, Ct);

            await Assert.That(result.Bytes).IsEqualTo((long)payload.Length);
            await Assert.That(result.Characters).IsEqualTo(14L);
            await Assert.That(result.Words).IsEqualTo(3L);
            await Assert.That(result.Lines).IsEqualTo(2L);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task SameInstance_ReusedSequentially_DoesNotLeakStateBetweenReads()
    {
        // Per-read state must not persist: a first read ending mid-word must not
        // leave the instance flagged as "in a word", swallowing the next read's first word.
        using Stream first = Utf8("trailing");
        CountResult firstResult = await _engine.CountAsync(first, WordsAndLines, Ct);

        using Stream second = Utf8("trailing");
        CountResult secondResult = await _engine.CountAsync(second, WordsAndLines, Ct);

        await Assert.That(secondResult.Words).IsEqualTo(firstResult.Words);
        await Assert.That(secondResult.Lines).IsEqualTo(firstResult.Lines);
        await Assert.That(secondResult.Words).IsEqualTo(1L);
        await Assert.That(secondResult.Lines).IsEqualTo(0L);
    }

    [Test]
    public async Task SameInstance_ConcurrentReads_ProduceIndependentResults()
    {
        const string text = "one two three\nfour five six\n";
        CancellationToken ct = Ct;

        Task<CountResult>[] reads = Enumerable.Range(0, 8)
            .Select(_ => Task.Run(async () =>
            {
                using Stream stream = Utf8(text);
                return await _engine.CountAsync(stream, WordsAndLines, ct);
            }, ct))
            .ToArray();

        CountResult[] results = await Task.WhenAll(reads);

        foreach (CountResult result in results)
        {
            await Assert.That(result.Words).IsEqualTo(6L);
            await Assert.That(result.Lines).IsEqualTo(2L);
        }
    }
}
