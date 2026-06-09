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
    private bool _isInWord;
    private bool _hasPendingNonNewline;
    private Encoding? _currentEncoding;

    public TextReaderLogic(IWordCounter wordCounter,
        IByteCounter byteCounter, ICharacterCounter characterCounter)
    {
        _wordCounter = wordCounter ?? throw new ArgumentNullException(nameof(wordCounter));
        _byteCounter = byteCounter ?? throw new ArgumentNullException(nameof(byteCounter));
        _characterCounter = characterCounter ?? throw new ArgumentNullException(nameof(characterCounter));
    }

    protected WCountInfo ReadTextChunk(int chunkSize, char[] buffer, bool showWordCount,
    bool showLineCount,
    bool showCharacterCount, bool showByteCount, ref bool hasCharWasCR)
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
            if (hasCharWasCR)
            {
                if (totalLines is not null) totalLines += 1;
                hasCharWasCR = false;
            }
            else
            {
                if (totalLines is not null) totalLines += 1;
            }
            _hasPendingNonNewline = false;
        }
        else if (c == '\r')
        {
            // If next char in the same chunk is '\n', defer counting until the '\n' is processed.
            if (i + 1 < chunkSize && buffer[i + 1] == '\n')
            {
                hasCharWasCR = true;
            }
            else if (i + 1 == chunkSize)
            {
                // trailing CR at end of chunk; let the caller preserve the flag so next chunk can complete the pair
                hasCharWasCR = true;
            }
            else
            {
                // CR not followed by LF -> count as a line terminator now
                if (totalLines is not null) totalLines += 1;
                hasCharWasCR = false;
            }
            _hasPendingNonNewline = false;
        }
        else
        {
            hasCharWasCR = false;
            _hasPendingNonNewline = true;
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
        if (_isInWord && segment.Length > 0 && !char.IsWhiteSpace(segment[0]) && words > 0)
        {
            words -= 1;
        }

        // Update in-word state for next chunk (true if last char is non-whitespace)
        _isInWord = (segment.Length > 0) && !char.IsWhiteSpace(segment[segment.Length - 1]);


        totalWords += words;
    }

    if (totalChars is not null)
        totalChars += Convert.ToInt64(_characterCounter.CountCharacters(segment, _currentEncoding ?? Encoding.UTF8));

    if (totalBytes is not null)
        totalBytes += _byteCounter.CountBytes(segment, _currentEncoding ?? Encoding.UTF8);

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
        bool hasCharWasCR = false;
        // Initialise chunk state used across ReadTextChunk calls
        _isInWord = false;
        _hasPendingNonNewline = false;
        _currentEncoding = (reader is StreamReader sr) ? sr.CurrentEncoding : (Console.InputEncoding ?? Encoding.UTF8);

        while ((charsRead = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
        {
            WCountInfo result = ReadTextChunk(charsRead, buffer, showWordCount, showLineCount,
                showCharacterCount, showByteCount, ref hasCharWasCR);


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
        else if (_hasPendingNonNewline && totalLines is not null)
            totalLines += 1;

        // Reset encoding cache
        _currentEncoding = null;

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