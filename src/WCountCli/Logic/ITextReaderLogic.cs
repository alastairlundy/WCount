using WCountCli.Models;

namespace WCountCli.Logic;

public interface ITextReaderLogic
{
    Task<WCountInfo> ReadStandardInputAsync(bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, bool configuredArgs);

    Task<WCountInfo> ReadFileAsync(string file, bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, bool configuredArgs);
}