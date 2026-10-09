/*
    WCountLib
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace WCountLib.Logic;

/// <summary>
/// The Counting Engine: reads raw bytes from a stream and computes only the counts
/// the given <see cref="CountRequest"/> asks for.
/// </summary>
/// <remarks>
/// The engine owns byte-order mark handling and encoding detection, so its single
/// entry takes no encoding parameter. Raw bytes are counted straight from the stream
/// with the byte-order mark included and are never re-encoded. Lines follow the GNU
/// definition: a newline character only — no final-unterminated line and no lone
/// carriage return.
/// </remarks>
public sealed class CountingEngine
{
    private const int BufferSize = 8192;

    /// <summary>
    /// Counts raw bytes from <paramref name="stream"/> and computes only the requested counts.
    /// </summary>
    /// <param name="stream">
    /// The stream to read raw bytes from. The engine counts the bytes as they come,
    /// byte-order mark included, and resolves the text encoding itself from the stream's
    /// leading byte-order mark (defaulting to UTF-8 when none is present).
    /// </param>
    /// <param name="request">The counts to compute.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>
    /// A <see cref="CountResult"/> whose requested fields carry the counted value
    /// (never negative) and whose unrequested fields carry <c>-1</c>.
    /// </returns>
    public async Task<CountResult> CountAsync(Stream stream, CountRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!request.Words && !request.Lines && !request.Bytes && !request.Characters)
        {
            // Nothing was asked for: nothing is read and nothing is computed.
            return new CountResult(-1, -1, -1, -1);
        }

        byte[] byteBuffer = new byte[BufferSize];
        char[] charBuffer = new char[BufferSize * 2];

        long byteCount = 0;
        long lineCount = 0;
        long wordCount = 0;
        long characterCount = 0;
        bool inWord = false;

        bool needsText = request.Words || request.Lines || request.Characters;

        // Read a small prelude first so a trickling stream still exposes its whole
        // byte-order mark before decoding starts.
        int prelude = 0;
        while (prelude < 4)
        {
            int read = await stream.ReadAsync(byteBuffer.AsMemory(prelude), ct);
            if (read == 0)
                break;
            prelude += read;
        }

        if (request.Bytes)
            byteCount = prelude;

        Decoder? decoder = null;
        int byteOrderMarkLength = 0;
        if (needsText)
        {
            Encoding encoding = DetectEncoding(byteBuffer.AsSpan(0, prelude), out byteOrderMarkLength);
            decoder = encoding.GetDecoder();

            // The byte-order mark bytes are counted but never decoded.
            if (prelude > byteOrderMarkLength)
                CountDecoded(decoder, byteBuffer.AsSpan(byteOrderMarkLength, prelude - byteOrderMarkLength), flush: false);
        }

        while (true)
        {
            int read = await stream.ReadAsync(byteBuffer.AsMemory(), ct);
            if (read == 0)
                break;

            // Raw bytes as they come: no re-encode round-trip, byte-order mark included.
            if (request.Bytes)
                byteCount += read;

            if (needsText)
                CountDecoded(decoder!, byteBuffer.AsSpan(0, read), flush: false);
        }

        if (needsText)
        {
            // Flush any trailing decoder state so the final bytes contribute their characters.
            CountDecoded(decoder!, ReadOnlySpan<byte>.Empty, flush: true);
        }

        return new CountResult(
            request.Words ? wordCount : -1,
            request.Lines ? lineCount : -1,
            request.Bytes ? byteCount : -1,
            request.Characters ? characterCount : -1);

        // Decodes a chunk of raw bytes and runs the requested counts over the text it produces.
        void CountDecoded(Decoder activeDecoder, ReadOnlySpan<byte> bytes, bool flush)
        {
            int charsProduced = activeDecoder.GetChars(bytes, charBuffer.AsSpan(), flush);
            if (charsProduced == 0)
                return;

            ReadOnlySpan<char> text = charBuffer.AsSpan(0, charsProduced);

            // Characters are the decoded text length.
            if (request.Characters)
                characterCount += text.Length;

            if (request.Lines)
                lineCount += CountLinesChunk(text);

            if (request.Words)
                wordCount += CountWordsChunk(text, ref inWord);
        }
    }

    /// <summary>
    /// Counts lines in a decoded chunk using the GNU line definition: a newline
    /// character only — a lone carriage return counts for nothing, and text left
    /// after the final newline is not a line of its own.
    /// </summary>
    private static long CountLinesChunk(ReadOnlySpan<char> text)
    {
        long lines = 0;

        foreach (char c in text)
        {
            if (c == '\n')
                lines++;
        }

        return lines;
    }

    /// <summary>
    /// Counts whitespace-separated tokens in a decoded chunk, carrying word state across
    /// chunk boundaries so a word straddling two chunks is counted once.
    /// </summary>
    private static long CountWordsChunk(ReadOnlySpan<char> text, ref bool inWord)
    {
        if (text.IsEmpty)
            return 0;

        string[] tokens = text.ToString().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        long words = tokens.Length;

        // When the previous chunk ended inside a word, this chunk's leading token is
        // that word's continuation rather than a new word.
        if (inWord && !char.IsWhiteSpace(text[0]) && words > 0)
            words -= 1;

        inWord = !char.IsWhiteSpace(text[^1]);
        return words;
    }

    /// <summary>
    /// Resolves the encoding of a stream from its leading bytes, reporting how many
    /// byte-order mark bytes to skip. Streams without a byte-order mark decode as UTF-8.
    /// </summary>
    private static Encoding DetectEncoding(ReadOnlySpan<byte> head, out int byteOrderMarkLength)
    {
        byteOrderMarkLength = 0;

        // UTF-32 little endian must be tested before UTF-16 little endian: the
        // UTF-16 mark is a two-byte prefix of the UTF-32 one.
        if (head.Length >= 4 && head[0] == 0xFF && head[1] == 0xFE && head[2] == 0x00 && head[3] == 0x00)
        {
            byteOrderMarkLength = 4;
            return Encoding.UTF32;
        }

        if (head.Length >= 4 && head[0] == 0x00 && head[1] == 0x00 && head[2] == 0xFE && head[3] == 0xFF)
        {
            byteOrderMarkLength = 4;
            return new UTF32Encoding(bigEndian: true, byteOrderMark: false);
        }

        if (head.Length >= 3 && head[0] == 0xEF && head[1] == 0xBB && head[2] == 0xBF)
        {
            byteOrderMarkLength = 3;
            return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        }

        if (head.Length >= 2 && head[0] == 0xFF && head[1] == 0xFE)
        {
            byteOrderMarkLength = 2;
            return Encoding.Unicode;
        }

        if (head.Length >= 2 && head[0] == 0xFE && head[1] == 0xFF)
        {
            byteOrderMarkLength = 2;
            return Encoding.BigEndianUnicode;
        }

        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}
