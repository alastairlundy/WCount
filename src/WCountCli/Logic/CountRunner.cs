/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountCli.Logic;

public static class CountRunner
{
    public static async Task<int> RunAsync(
        CountingEngine engine,
        CountRequest request,
        IReadOnlyList<string> files,
        Stream standardInput,
        TextWriter output,
        TextWriter error,
        bool verbose,
        CancellationToken ct = default)
    {
        try
        {
            long totalLines = 0;
            long totalWords = 0;
            long totalChars = 0;
            long totalBytes = 0;
            long totalMaxLineLength = 0;

            bool readFromStandardInput = files.Count == 0;

            IEnumerable<string> sources = readFromStandardInput
                ? [string.Empty]
                : files.Select(Path.GetFullPath);

            foreach (string source in sources)
            {
                CountResult result = readFromStandardInput
                    ? await engine.CountAsync(standardInput, request, ct)
                    : await CountFileAsync(engine, source, request, ct);

                await ResultPrintingHelper.PrintRow(source, output, request,
                    result.Lines, result.Words, result.Characters, result.Bytes, result.MaximumLineLength);

                if (request.Lines) totalLines += result.Lines;
                if (request.Words) totalWords += result.Words;
                if (request.Characters) totalChars += result.Characters;
                if (request.Bytes) totalBytes += result.Bytes;
                // GNU's total for -L is the maximum across files, not the sum.
                if (request.MaximumLineLength) totalMaxLineLength = Math.Max(totalMaxLineLength, result.MaximumLineLength);
            }

            if (files.Count > 1)
            {
                await ResultPrintingHelper.PrintRow(Resources.Output_Labels_Total, output, request,
                    totalLines, totalWords, totalChars, totalBytes, totalMaxLineLength);
            }

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

    private static async Task<CountResult> CountFileAsync(
        CountingEngine engine, string path, CountRequest request, CancellationToken ct)
    {
        await using FileStream stream = File.OpenRead(path);
        return await engine.CountAsync(stream, request, ct);
    }
}
