namespace WCountLib.Testing.Logic;

/// <summary>
/// Drives the engine's full encoding-detection matrix through its single entry:
/// every byte-order-mark path, the no-mark UTF-8 default, and the bytes-only
/// path that never decodes at all. Byte counts always include the mark; text
/// counts never do.
/// </summary>
public class EncodingMatrixTests
{
    private readonly CountingEngine _engine = new();

    private static CountRequest Everything => new(Words: true, Lines: true, Bytes: true, Characters: true);

    private static CountRequest BytesOnly => new(Words: false, Lines: false, Bytes: true, Characters: false);

    private static CountRequest CharactersAndBytes =>
        new(Words: false, Lines: false, Bytes: true, Characters: true);

    private static CancellationToken Ct => CancellationToken.None;

    // "café crème\n": 12 characters, 2 words, 1 line, 13 UTF-8 bytes.
    private const string Text = "café crème\n";

    private static long Utf8ByteCount => Encoding.UTF8.GetByteCount(Text);

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Utf8_WithAndWithoutByteOrderMark_CountsIdenticallyExceptTheMark(bool withMark)
    {
        byte[] payload = withMark
            ? [0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes(Text)]
            : Encoding.UTF8.GetBytes(Text);

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo(Utf8ByteCount + (withMark ? 3 : 0));
        await Assert.That(result.Characters).IsEqualTo((long)Text.Length);
        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task Utf16LittleEndianByteOrderMark_IsDetectedInsideTheEngine()
    {
        byte[] payload = [0xFF, 0xFE, .. Encoding.Unicode.GetBytes(Text)];

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo(payload.Length);
        await Assert.That(result.Characters).IsEqualTo((long)Text.Length);
        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task Utf16BigEndianByteOrderMark_IsDetectedInsideTheEngine()
    {
        byte[] payload = [0xFE, 0xFF, .. Encoding.BigEndianUnicode.GetBytes(Text)];

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo(payload.Length);
        await Assert.That(result.Characters).IsEqualTo((long)Text.Length);
        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task Utf32LittleEndianByteOrderMark_IsDetectedInsideTheEngine()
    {
        // The UTF-32 LE mark is four bytes, and its two-byte UTF-16 prefix must
        // not win the detection order.
        byte[] payload = [0xFF, 0xFE, 0x00, 0x00, .. Encoding.UTF32.GetBytes(Text)];

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo(payload.Length);
        await Assert.That(result.Characters).IsEqualTo((long)Text.Length);
        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task Utf32BigEndianByteOrderMark_IsDetectedInsideTheEngine()
    {
        byte[] text = new UTF32Encoding(bigEndian: true, byteOrderMark: false).GetBytes(Text);
        byte[] payload = [0x00, 0x00, 0xFE, 0xFF, .. text];

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo(payload.Length);
        await Assert.That(result.Characters).IsEqualTo((long)Text.Length);
        await Assert.That(result.Words).IsEqualTo(2L);
        await Assert.That(result.Lines).IsEqualTo(1L);
    }

    [Test]
    public async Task BytesOnly_NeverDecodesAndCountsRawBytes()
    {
        // The bytes-only path skips encoding detection entirely; a UTF-16
        // payload still counts its raw bytes.
        byte[] payload = [0xFF, 0xFE, .. Encoding.Unicode.GetBytes(Text)];

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, BytesOnly, Ct);

        await Assert.That(result.Bytes).IsEqualTo(payload.Length);
        await Assert.That(result.Characters).IsEqualTo(-1L);
        await Assert.That(result.Words).IsEqualTo(-1L);
        await Assert.That(result.Lines).IsEqualTo(-1L);
    }

    [Test]
    public async Task ByteOrderMarkAlone_CountsAsBytesOnly()
    {
        // A stream that is nothing but a mark: bytes match the mark length and
        // every text count is zero — the mark bytes are never decoded.
        byte[] payload = [0xEF, 0xBB, 0xBF];

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        await Assert.That(result.Bytes).IsEqualTo(3L);
        await Assert.That(result.Characters).IsEqualTo(0L);
        await Assert.That(result.Words).IsEqualTo(0L);
        await Assert.That(result.Lines).IsEqualTo(0L);
    }

    [Test]
    public async Task CharactersAndBytes_OnUtf16_BothCountsAreCorrect()
    {
        // A selective text request on a multi-byte encoding: the character
        // count is decoded characters, the byte count is raw encoded bytes.
        byte[] payload = [0xFF, 0xFE, .. Encoding.Unicode.GetBytes(Text)];

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, CharactersAndBytes, Ct);

        await Assert.That(result.Characters).IsEqualTo((long)Text.Length);
        await Assert.That(result.Bytes).IsEqualTo(payload.Length);
        await Assert.That(result.Words).IsEqualTo(-1L);
        await Assert.That(result.Lines).IsEqualTo(-1L);
    }

    [Test]
    public async Task Utf16LittleEndian_ByteBackedStream_CountsLikeMemoryStream()
    {
        // The same UTF-16 payload through a byte-backed file stream: encoding
        // detection must not depend on the stream being seekable or in memory.
        string path = Path.Combine(Path.GetTempPath(), $"wcount-encoding-{Guid.NewGuid():N}.txt");
        try
        {
            byte[] payload = [0xFF, 0xFE, .. Encoding.Unicode.GetBytes(Text)];
            await File.WriteAllBytesAsync(path, payload);

            await using FileStream stream = File.OpenRead(path);
            CountResult result = await _engine.CountAsync(stream, Everything, Ct);

            await Assert.That(result.Bytes).IsEqualTo(payload.Length);
            await Assert.That(result.Characters).IsEqualTo((long)Text.Length);
            await Assert.That(result.Words).IsEqualTo(2L);
            await Assert.That(result.Lines).IsEqualTo(1L);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task TruncatedByteOrderMark_DoesNotCrashOrMiscountBytes()
    {
        // A stream cut off inside its mark exercises the partial-prelude path.
        byte[] payload = [0xFF];

        using Stream stream = new MemoryStream(payload);
        CountResult result = await _engine.CountAsync(stream, Everything, Ct);

        // One byte cannot be valid UTF-8 text; the byte count is exact either way.
        await Assert.That(result.Bytes).IsEqualTo(1L);
    }
}
