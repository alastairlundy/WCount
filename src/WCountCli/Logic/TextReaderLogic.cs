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
        _wordCounter = wordCounter;
        _byteCounter = byteCounter;
        _characterCounter = characterCounter;
    }

    protected WCountInfo ReadTextChunk(int chunkSize, char[] buffer, bool showWordCount,
        bool showLineCount,
        bool showCharacterCount, bool showByteCount, bool configuredArgs)
    {
        long? totalWords = showWordCount || !configuredArgs? 0 : null;
        long? totalLines = showLineCount || !configuredArgs ? 0 : null;
        long? totalChars = showCharacterCount || !configuredArgs ? 0 : null;
        long? totalBytes = showByteCount ? 0 : null;

        bool hasCharWasCR = false;
        
        foreach (char c in buffer)
        {
            if (hasCharWasCR && c == '\n')
            {
                totalLines += 1;
                hasCharWasCR = false;
            }
            else if (c == '\n' && !OperatingSystem.IsWindows())
            {
                totalLines += 1;
            }
            
            if(c == '\r')
                hasCharWasCR = true;

            totalChars += 1;
            totalBytes += _byteCounter.CountBytes([c], Encoding.Default);
        }

        if (totalWords is not null)
            totalWords += Convert.ToInt64(_wordCounter.CountWords(buffer));

        if (totalChars is not null)
            totalChars += Convert.ToInt64(_characterCounter.CountCharacters(buffer, Encoding.Default));

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
        
        long? totalWords = showWordCount || !configuredArgs? 0 : null;
        long? totalLines = showLineCount || !configuredArgs ? 0 : null;
        long? totalChars = showCharacterCount || !configuredArgs ? 0 : null;
        long? totalBytes = showByteCount ? 0 : null;

        int charsRead;
        while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            WCountInfo result = ReadTextChunk(charsRead, buffer, showWordCount, showLineCount,
                showCharacterCount,showByteCount,configuredArgs);
            
            if(totalWords is not null)
                totalBytes  += result.ByteCount;
            
            if(totalChars is not null)
                totalChars += result.CharCount;

            if(totalWords is not null)
                totalWords += result.WordCount;
            
            if(totalLines is not null)
                totalLines += result.LineCount;
        }
        
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