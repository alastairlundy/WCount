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
/// carriage return. The maximum-line-length count (<c>-L</c>) computes the GNU wc
/// maximum display width: a tab advances to the next multiple of 8, East Asian
/// wide/fullwidth characters and common emoji count 2, combining marks and
/// non-printables count 0, and <c>\r</c>/<c>\f</c> end the length line just like
/// <c>\n</c>. Rune widths come from the Wcwidth package's Unicode 16 tables.
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

        if (!request.Words && !request.Lines && !request.Bytes && !request.Characters && !request.MaximumLineLength)
        {
            // Nothing was asked for: nothing is read and nothing is computed.
            return new CountResult(-1, -1, -1, -1, -1);
        }

        byte[] byteBuffer = new byte[BufferSize];
        char[] charBuffer = new char[BufferSize * 2];

        long byteCount = 0;
        long lineCount = 0;
        long wordCount = 0;
        long characterCount = 0;
        bool inWord = false;
        bool pendingCharHighSurrogate = false;

        // Display-width state for the maximum-line-length count, carried across
        // decoded chunks so a line longer than one chunk accumulates correctly.
        long currentLineLength = 0;
        long maximumLineLength = 0;
        char pendingWidthHighSurrogate = '\0';
        bool hasPendingWidthHighSurrogate = false;

        bool needsText = request.Words || request.Lines || request.Characters || request.MaximumLineLength;

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

            // A trailing high surrogate with no low half is a lone character.
            if (request.Characters && pendingCharHighSurrogate)
                characterCount += 1;

            // An unterminated final line still has a display length.
            // (A pending width high surrogate is lone and adds no width.)
            if (request.MaximumLineLength)
                CloseLine();
        }

        return new CountResult(
            request.Words ? wordCount : -1,
            request.Lines ? lineCount : -1,
            request.Bytes ? byteCount : -1,
            request.Characters ? characterCount : -1,
            request.MaximumLineLength ? maximumLineLength : -1);

        // Decodes a chunk of raw bytes and runs the requested counts over the text it produces.
        void CountDecoded(Decoder activeDecoder, ReadOnlySpan<byte> bytes, bool flush)
        {
            int charsProduced = activeDecoder.GetChars(bytes, charBuffer.AsSpan(), flush);
            if (charsProduced == 0)
                return;

            ReadOnlySpan<char> text = charBuffer.AsSpan(0, charsProduced);

            // Characters are Unicode scalar values: a surrogate pair counts once,
            // a lone surrogate counts once.
            if (request.Characters)
                characterCount += CountCharactersChunk(text, ref pendingCharHighSurrogate);

            if (request.Lines)
                lineCount += CountLinesChunk(text);

            if (request.Words)
                wordCount += CountWordsChunk(text, ref inWord);

            if (request.MaximumLineLength)
                AccumulateLineWidths(text);
        }

        // Runs the GNU display-width rules over a decoded chunk, carrying the current
        // line's length across chunk boundaries. A high surrogate split across two
        // chunks is reunited with its low half before its width is measured.
        void AccumulateLineWidths(ReadOnlySpan<char> text)
        {
            int i = 0;

            if (hasPendingWidthHighSurrogate)
            {
                hasPendingWidthHighSurrogate = false;

                if (!text.IsEmpty && char.IsLowSurrogate(text[0]))
                {
                    currentLineLength += RuneWidth(new Rune(pendingWidthHighSurrogate, text[0]));
                    i = 1;
                }
                // Else the stashed high was lone and adds no width.
            }

            for (; i < text.Length; i++)
            {
                char c = text[i];

                switch (c)
                {
                    // '\r' and '\f' end the length line just like '\n' does.
                    case '\n':
                    case '\r':
                    case '\f':
                        CloseLine();
                        break;
                    case '\t':
                        // Tab advances to the next multiple of 8.
                        currentLineLength += 8 - (currentLineLength % 8);
                        break;
                    case ' ':
                        currentLineLength++;
                        break;
                    case '\v':
                        // Vertical tab is a separator of display width 0.
                        break;
                    default:
                        if (char.IsHighSurrogate(c))
                        {
                            if (i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
                            {
                                currentLineLength += RuneWidth(new Rune(c, text[i + 1]));
                                i++;
                            }
                            else if (i + 1 == text.Length)
                            {
                                // Possible split pair: defer until the next chunk
                                // decides pair versus lone surrogate.
                                pendingWidthHighSurrogate = c;
                                hasPendingWidthHighSurrogate = true;
                            }
                            // Else a lone high surrogate: no display width.
                        }
                        else if (char.IsSurrogate(c))
                        {
                            // A lone surrogate has no display width.
                        }
                        else
                        {
                            currentLineLength += RuneWidth(new Rune(c));
                        }

                        break;
                }
            }
        }

        // Ends the current length line: its width wins if it is the largest so far.
        void CloseLine()
        {
            if (currentLineLength > maximumLineLength)
                maximumLineLength = currentLineLength;

            currentLineLength = 0;
        }
    }

    /// <summary>
    /// The display width of a single rune under the GNU wc rules, delegated to the
    /// Wcwidth package's Unicode 16 East Asian Width tables. Wcwidth returns -1 for
    /// control characters (other than NUL, which it returns as 0); those add no
    /// display width, so they clamp to 0 here. Combining marks and other zero-width
    /// characters return 0, wide/fullwidth characters and emoji return 2, and
    /// everything else returns 1.
    /// </summary>
    private static int RuneWidth(Rune rune)
    {
        int width = UnicodeCalculator.GetWidth(rune);
        return width < 0 ? 0 : width;
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
    /// Counts Unicode scalar values in a decoded chunk: a high/low surrogate pair
    /// counts once, a lone surrogate counts once. A trailing high surrogate is
    /// deferred via <paramref name="pendingHighSurrogate"/> until the next chunk
    /// reveals whether it pairs or stands alone.
    /// </summary>
    private static long CountCharactersChunk(ReadOnlySpan<char> text, ref bool pendingHighSurrogate)
    {
        if (text.IsEmpty)
            return 0;

        long count = 0;
        int i = 0;

        if (pendingHighSurrogate)
        {
            pendingHighSurrogate = false;

            if (char.IsLowSurrogate(text[0]))
            {
                // Pair split across chunks: one character total.
                count += 1;
                i = 1;
            }
            else
            {
                // The deferred high was lone: one character.
                count += 1;
            }
        }

        while (i < text.Length)
        {
            char c = text[i];

            if (char.IsHighSurrogate(c))
            {
                if (i + 1 < text.Length)
                {
                    // In-chunk pair counts once; a high followed by a
                    // non-low is a lone surrogate and counts once.
                    count += 1;
                    i += char.IsLowSurrogate(text[i + 1]) ? 2 : 1;
                }
                else
                {
                    // Possible split pair: wait for the next chunk.
                    pendingHighSurrogate = true;
                    i += 1;
                }
            }
            else
            {
                // BMP character or lone low surrogate: one character.
                count += 1;
                i += 1;
            }
        }

        return count;
    }

    /// <summary>
    /// Counts whitespace-separated words in a decoded chunk with a single
    /// allocation-free scan, carrying word state across chunk boundaries so a
    /// word straddling two chunks is counted once.
    /// </summary>
    private static long CountWordsChunk(ReadOnlySpan<char> text, ref bool inWord)
    {
        long words = 0;
        bool current = inWord;

        foreach (char c in text)
        {
            if (char.IsWhiteSpace(c))
            {
                current = false;
            }
            else if (!current)
            {
                words += 1;
                current = true;
            }
        }

        inWord = current;
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
