using System.Globalization;
using XenoAtom.Terminal;

namespace WCountCli.Helpers;

public static class ResultPrintingHelper
{
    public static async Task PrintCustomResultLine(string file, long? lineCount = null, long? wordCount = null,
        long? characterCount = null, long? byteCount = null)
    {
        StringBuilder stringBuilder = new();

        List<long> stats = [];

        if (lineCount is not null)
            stats.Add(lineCount.Value);

        if (wordCount is not null)
            stats.Add(wordCount.Value);

        if (characterCount is not null)
            stats.Add(characterCount.Value);

        int requiredSpacing = FormattingHelpers.CalculateRequiredSpacing(stats.ToArray());

        if (lineCount is not null)
            stringBuilder.Append(FormattingHelpers
                .FormatOutput(lineCount.Value.ToString(CultureInfo.CurrentCulture), requiredSpacing).TrimStart(' '));

        if (wordCount is not null)
            stringBuilder.Append(FormattingHelpers.FormatOutput(wordCount.Value.ToString(CultureInfo.CurrentCulture),
                requiredSpacing));

        if (byteCount is not null)
            stringBuilder.Append(FormattingHelpers.FormatOutput(byteCount.Value.ToString(CultureInfo.CurrentCulture),
                requiredSpacing));

        if (characterCount is not null)
            stringBuilder.Append(
                FormattingHelpers.FormatOutput(characterCount.Value.ToString(CultureInfo.CurrentCulture),
                    requiredSpacing));

        stringBuilder.Append(' ');
        stringBuilder.Append(file);

        await Terminal.Out.WriteLineAsync(stringBuilder.ToString());
    }


    public static async Task PrintDefaultResultLine(string file, long lineCount, long wordCount, long characterCount)
    {
        StringBuilder stringBuilder = new();

        int requiredSpacing = FormattingHelpers.CalculateRequiredSpacing([lineCount, wordCount, characterCount]);

        stringBuilder.Append(FormattingHelpers
            .FormatOutput(lineCount.ToString(CultureInfo.CurrentCulture), requiredSpacing).TrimStart(' '));

        stringBuilder.Append(FormattingHelpers.FormatOutput(wordCount.ToString(CultureInfo.CurrentCulture),
            requiredSpacing));

        stringBuilder.Append(FormattingHelpers.FormatOutput(characterCount.ToString(CultureInfo.CurrentCulture),
            requiredSpacing));

        stringBuilder.Append(' ');
        stringBuilder.Append(file);

        await Terminal.Out.WriteLineAsync(stringBuilder.ToString());
    }
}