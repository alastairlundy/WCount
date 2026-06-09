namespace WCountCli.Logic;

public interface ITextReaderLogic
{
    Task<WCountInfo> ReadStandardInputAsync(TextReader reader, bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, CancellationToken ct = default);

    Task<WCountInfo> ReadFileAsync(string file, bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, CancellationToken ct = default);
}