/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Text;
using WCountLib.Abstractions.Logic;
using WCountLib.Abstractions.Models;

namespace WCountCli.Logic;

public static class CountRunner
{
    private static long? Add(long? total, long? value) => total is null ? null : total + (value ?? 0);

    private static long? Selected(bool show, long? value) => show ? value ?? 0L : null;

    public static async Task<int> RunAsync(
        ITextReaderLogic textReaderLogic,
        CountSelection selection,
        IReadOnlyList<string> files,
        TextReader standardInput,
        TextWriter output,
        TextWriter error,
        bool verbose,
        CancellationToken ct = default)
    {
        bool showLineCount = selection.HasFlag(CountSelection.Lines);
        bool showWordCount = selection.HasFlag(CountSelection.Words);
        bool showCharacterCount = selection.HasFlag(CountSelection.Characters);
        bool showByteCount = selection.HasFlag(CountSelection.Bytes);

        try
        {
            long? totalLines = showLineCount ? 0L : null;
            long? totalWords = showWordCount ? 0L : null;
            long? totalChars = showCharacterCount ? 0L : null;
            long? totalBytes = showByteCount ? 0L : null;

            bool readFromStandardInput = files.Count == 0;

            IEnumerable<string> sources = readFromStandardInput
                ? [string.Empty]
                : files.Select(Path.GetFullPath);

            Encoding? encoding = readFromStandardInput
                ? standardInput is StreamReader sr ? sr.CurrentEncoding : Console.InputEncoding
                : null;

            foreach (string source in sources)
            {
                WCountInfo info = readFromStandardInput
                    ? await textReaderLogic.ReadTextReaderAsync(standardInput, showWordCount, showLineCount,
                        showCharacterCount, showByteCount, encoding, ct)
                    : await textReaderLogic.ReadFileAsync(source, showWordCount, showLineCount,
                        showCharacterCount, showByteCount, encoding, ct);

                await ResultPrintingHelper.PrintRow(source, output, selection,
                    info.LineCount, info.WordCount, info.CharCount, info.ByteCount);

                totalLines = Add(totalLines, info.LineCount);
                totalWords = Add(totalWords, info.WordCount);
                totalChars = Add(totalChars, info.CharCount);
                totalBytes = Add(totalBytes, info.ByteCount);
            }

            if (files.Count > 1 || (files.Count == 1 && selection == CountSelection.Default))
                await ResultPrintingHelper.PrintRow(Resources.Output_Labels_Total, output, selection,
                    totalLines, totalWords, totalChars, totalBytes);

            return 0;
        }
        catch (Exception exception)
        {
            if (files.Count == 0)
                await error.WriteLineAsync("Ran into issues whilst reading standard input.");
            else
                await error.WriteLineAsync("Ran into issues whilst reading a file.");

            if (verbose)
                await error.WriteLineAsync($"Exception Details: {exception.Message}");

            return 1;
        }
    }
}
