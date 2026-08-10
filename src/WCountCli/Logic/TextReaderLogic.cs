using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WCountCli.Models;
using WCountLib.Abstractions.Counters;

namespace WCountCli.Logic;

public class TextReaderLogic : ITextReaderLogic
{
    private readonly IWordCounter _wordCounter;
    private readonly IByteCounter _byteCounter;
    private readonly ICharacterCounter _characterCounter;

    protected ref struct ChunkState
    {
        public bool IsInWord;
        public bool HasPendingNonNewline;
        public Encoding? CurrentEncoding;
        public bool HasCharWasCR;
    }

    public TextReaderLogic(IWordCounter wordCounter,
        IByteCounter byteCounter, ICharacterCounter characterCounter)
    {
        _wordCounter = wordCounter ?? throw new ArgumentNullException(nameof(wordCounter));
        _byteCounter = byteCounter ?? throw new ArgumentNullException(nameof(byteCounter));
        _characterCounter = characterCounter ?? throw new ArgumentNullException(nameof(characterCounter));
    }

    private static Encoding ResolveDefaultEncoding()
    {
        try
        {
            return Encoding.Default;
        }
        catch (NotSupportedException)
        {
            return Encoding.UTF8;
        }
        catch (ArgumentException)
        {
            return Encoding.UTF8;
        }
    }

    protected WCountInfo ReadTextChunk(int chunkSize, char[] buffer, bool showWordCount,
    bool showLineCount,
    bool showCharacterCount, bool showByteCount, ref ChunkState chunkState)
{
    long? totalWords = showWordCount ? 0L : null;
    long? totalLines = showLineCount ? 0L : null;
    long? totalChars = showCharacterCount ? 0L : null;
    long? totalBytes = showByteCount ? 0L : null;

    // Scan only the valid portion of the buffer
    for (int i = 0; i < chunkSize; i++)
    {
        char c = buffer[i];

        if (c == '\n')
        {
            if (chunkState.HasCharWasCR)
            {
                if (totalLines is not null) totalLines += 1;
                chunkState.HasCharWasCR = false;
            }
            else
            {
                if (totalLines is not null) totalLines += 1;
            }
            chunkState.HasPendingNonNewline = false;
        }
        else if (c == '\r')
        {
            // If next char in the same chunk is '\n', defer counting until the '\n' is processed.
            if (i + 1 < chunkSize && buffer[i + 1] == '\n')
            {
                chunkState.HasCharWasCR = true;
            }
            else if (i + 1 == chunkSize)
            {
                // trailing CR at end of chunk; let the caller preserve the flag so next chunk can complete the pair
                chunkState.HasCharWasCR = true;
            }
            else
            {
                // CR not followed by LF -> count as a line terminator now
                if (totalLines is not null) totalLines += 1;
                chunkState.HasCharWasCR = false;
            }
            chunkState.HasPendingNonNewline = false;
        }
        else
        {
            chunkState.HasCharWasCR = false;
            chunkState.HasPendingNonNewline = true;
        }
    }

    // Copy only the valid chars into a segment array to avoid passing trailing data to the counters
    char[] segment = new char[chunkSize];
    Array.Copy(buffer, 0, segment, 0, chunkSize);

    if (totalWords is not null)
    {
        // Use the injected word counter for segment counting and adjust for chunk boundaries
        int rawWords = _wordCounter.CountWords(segment);
        int words = rawWords;

        // If previous chunk ended inside a word and this segment begins with a non-whitespace,
        // the word counter will have counted the continuation as a new word; subtract one.
        if (chunkState.IsInWord && segment.Length > 0 && !char.IsWhiteSpace(segment[0]) && words > 0)
        {
            words -= 1;
        }

        // Update in-word state for next chunk (true if last char is non-whitespace)
        chunkState.IsInWord = (segment.Length > 0) && !char.IsWhiteSpace(segment[segment.Length - 1]);


        totalWords += words;
    }

    if (totalChars is not null)
        totalChars += Convert.ToInt64(_characterCounter.CountCharacters(segment, chunkState.CurrentEncoding ?? ResolveDefaultEncoding()));

    if (totalBytes is not null)
        totalBytes += _byteCounter.CountBytes(segment, chunkState.CurrentEncoding ?? ResolveDefaultEncoding());

    return new WCountInfo
    {
        WordCount = totalWords,
        LineCount = totalLines,
        CharCount = totalChars,
        ByteCount = totalBytes
    };
}
    protected async Task<WCountInfo> ReadTextReaderAsync(TextReader reader, bool showWordCount,
        bool showLineCount,
        bool showCharacterCount, bool showByteCount, CancellationToken ct = default)
    {
        char[] buffer = new char[8192];

        long? totalWords = showWordCount ? 0L : null;
        long? totalLines = showLineCount ? 0L : null;
        long? totalChars = showCharacterCount ? 0L : null;
        long? totalBytes = showByteCount ? 0L : null;

        int charsRead;
        // Per-chunk state kept in plain locals (ref struct can't cross await)
        bool isInWord = false;
        bool hasPendingNonNewline = false;
        bool hasCharWasCR = false;
        // Initialise chunk state used across ReadTextChunk calls
        _isInWord = false;
        _hasPendingNonNewline = false;
        _currentEncoding = (reader is StreamReader sr) ? sr.CurrentEncoding : ResolveDefaultEncoding();

        while ((charsRead = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
        {
            ChunkState chunkState = new ChunkState
            {
                IsInWord = isInWord,
                HasPendingNonNewline = hasPendingNonNewline,
                CurrentEncoding = _currentEncoding,
                HasCharWasCR = hasCharWasCR
            };

            WCountInfo result = ReadTextChunk(charsRead, buffer, showWordCount, showLineCount,
                showCharacterCount, showByteCount, ref chunkState);

            isInWord = chunkState.IsInWord;
            hasCharWasCR = chunkState.HasCharWasCR;
            hasPendingNonNewline = chunkState.HasPendingNonNewline;

            if (totalBytes is not null)
                totalBytes += result.ByteCount ?? 0;

            if (totalChars is not null)
                totalChars += result.CharCount ?? 0;

            if (totalWords is not null)
                totalWords += result.WordCount ?? 0;

            if (totalLines is not null)
                totalLines += result.LineCount ?? 0;
        }

        // If file ended with an unresolved CR or pending non-newline, count it as a line
        if (hasCharWasCR && totalLines is not null)
            totalLines += 1;
        else if (hasPendingNonNewline && totalLines is not null)
            totalLines += 1;

        return new WCountInfo
        {
            WordCount = totalWords,
            LineCount = totalLines,
            CharCount = totalChars,
            ByteCount = totalBytes
        };
    }

    public async Task<WCountInfo> ReadStandardInputAsync(TextReader reader, bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, CancellationToken ct = default)
    {
        return await ReadTextReaderAsync(reader, showWordCount, showLineCount, 
            showCharacterCount, showByteCount, ct);
    }

    public async Task<WCountInfo> ReadFileAsync(string file, bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, CancellationToken ct = default)
    {
        using StreamReader reader = File.OpenText(file);

        return await ReadTextReaderAsync(reader, showWordCount, showLineCount, showCharacterCount, showByteCount, ct);
    }
}