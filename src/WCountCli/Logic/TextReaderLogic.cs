using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WCountCli.Models;
using WCountLib.Abstractions.Counters;
using XenoAtom.Terminal;

namespace WCountCli.Logic;

public class TextReaderLogic : ITextReaderLogic
{
    private readonly IWordCounter _wordCounter;
    private readonly IByteCounter _byteCounter;
    private readonly ICharacterCounter _characterCounter;

    public TextReaderLogic(IWordCounter wordCounter,
        IByteCounter byteCounter, ICharacterCounter characterCounter)
    {
        _wordCounter = wordCounter ?? throw new ArgumentNullException(nameof(wordCounter));
        _byteCounter = byteCounter ?? throw new ArgumentNullException(nameof(byteCounter));
        _characterCounter = characterCounter ?? throw new ArgumentNullException(nameof(characterCounter));
    }

    protected WCountInfo ReadTextChunk(int chunkSize, char[] buffer, bool showWordCount,
        bool showLineCount,
        bool showCharacterCount, bool showByteCount, bool configuredArgs, ref bool hasCharWasCR)
    {
        long? totalWords = (showWordCount || !configuredArgs) ? 0L : null;
        long? totalLines = (showLineCount || !configuredArgs) ? 0L : null;
        long? totalChars = (showCharacterCount || !configuredArgs) ? 0L : null;
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
            }
            else
            {
                hasCharWasCR = false;
            }
        }

        // Copy only the valid chars into a segment array to avoid passing trailing data to the counters
        char[] segment = new char[chunkSize];
        Array.Copy(buffer, 0, segment, 0, chunkSize);

        if (totalWords is not null)
        {
            // Use a fast, robust whitespace-based word count. This avoids depending on external SplitBy behaviour
            // which has produced incorrect results for some inputs.
            int words = 0;
            bool inWord = false;
            for (int i = 0; i < segment.Length; i++)
            {
                char ch = segment[i];
                if (char.IsWhiteSpace(ch))
                {
                    inWord = false;
                }
                else
                {
                    if (!inWord)
                    {
                        words++;
                        inWord = true;
                    }
                }
            }

            totalWords += words;
        }

        if (totalChars is not null)
            totalChars += Convert.ToInt64(_characterCounter.CountCharacters(segment, Encoding.Default));

        if (totalBytes is not null)
            totalBytes += _byteCounter.CountBytes(segment, Encoding.Default);

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
        bool showCharacterCount, bool showByteCount, bool configuredArgs)
    {
        char[] buffer = new char[8192];

        long? totalWords = (showWordCount || !configuredArgs) ? 0L : null;
        long? totalLines = (showLineCount || !configuredArgs) ? 0L : null;
        long? totalChars = (showCharacterCount || !configuredArgs) ? 0L : null;
        long? totalBytes = showByteCount ? 0L : null;

        int charsRead;
        bool hasCharWasCR = false;

        while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            WCountInfo result = ReadTextChunk(charsRead, buffer, showWordCount, showLineCount,
                showCharacterCount, showByteCount, configuredArgs, ref hasCharWasCR);

            if (totalBytes is not null)
                totalBytes += result.ByteCount ?? 0;

            if (totalChars is not null)
                totalChars += result.CharCount ?? 0;

            if (totalWords is not null)
                totalWords += result.WordCount ?? 0;

            if (totalLines is not null)
                totalLines += result.LineCount ?? 0;
        }

        // If file ended with an unresolved CR, count it as a line
        if (hasCharWasCR && totalLines is not null)
            totalLines += 1;

        return new WCountInfo
        {
            WordCount = totalWords,
            LineCount = totalLines,
            CharCount = totalChars,
            ByteCount = totalBytes
        };
    }

    public async Task<WCountInfo> ReadStandardInputAsync(bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, bool configuredArgs)
    {
        using TextReader reader = Terminal.In;

        return await ReadTextReaderAsync(reader, showWordCount, showLineCount, 
            showCharacterCount, showByteCount, configuredArgs);
    }

    public async Task<WCountInfo> ReadFileAsync(string file, bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, bool configuredArgs)
    {
        using StreamReader reader = File.OpenText(file);

        return await ReadTextReaderAsync(reader, showWordCount, showLineCount, showCharacterCount, showByteCount, configuredArgs);
    }
}
