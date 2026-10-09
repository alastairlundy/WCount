/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountCli.Logic;

/// <summary>
/// When the Total row is printed, using the GNU wc WHEN modes: auto (the default),
/// always, only, or never. Auto means Total-only-for-multi-input runs.
/// </summary>
public enum TotalMode
{
    Auto,
    Always,
    Only,
    Never
}

/// <summary>
/// One input to count: a labelled stream, or a stream that could not be opened.
/// </summary>
public sealed class CountInput
{
    private CountInput(string label, Stream? stream, string? openError)
    {
        Label = label;
        Stream = stream;
        OpenError = openError;
    }

    /// <summary>
    /// The row label: the path for a file, and the empty string for standard
    /// input, whose row prints as counts with no label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// The stream to count; <c>null</c> when the input could not be opened.
    /// </summary>
    public Stream? Stream { get; }

    /// <summary>
    /// Why the input could not be opened; <c>null</c> when it could.
    /// </summary>
    public string? OpenError { get; }

    public static CountInput FromStream(string label, Stream stream) => new(label, stream, null);

    public static CountInput FailedToOpen(string label, string error) => new(label, null, error);
}

/// <summary>
/// One row of the output table: a label and the counts for that input.
/// </summary>
public sealed record ResultRow(string Label, CountResult Result);

/// <summary>
/// Runs the Counting Engine once per input and prints the contract-true table.
/// </summary>
public static class CountRunner
{
    /// <summary>
    /// Counts every input, then prints the whole table and, when the total mode
    /// asks for it, the Total row. An input that fails — open or count — prints
    /// one error line to <paramref name="error"/> and the remaining inputs are
    /// still counted; any failure makes the run exit 1.
    /// </summary>
    public static async Task<int> RunAsync(
        CountingEngine engine,
        CountRequest request,
        TotalMode totalMode,
        IReadOnlyList<CountInput> inputs,
        TextWriter output,
        TextWriter error,
        bool verbose,
        CancellationToken ct = default)
    {
        List<ResultRow> rows = [];
        bool anyFailed = false;

        long totalLines = 0;
        long totalWords = 0;
        long totalChars = 0;
        long totalBytes = 0;
        long totalMaxLineLength = 0;

        foreach (CountInput input in inputs)
        {
            if (input.Stream is null)
            {
                await error.WriteLineAsync(OpenErrorLine(input));
                anyFailed = true;
                continue;
            }

            try
            {
                CountResult result = await engine.CountAsync(input.Stream, request, ct);
                rows.Add(new ResultRow(input.Label, result));

                if (request.Lines) totalLines += result.Lines;
                if (request.Words) totalWords += result.Words;
                if (request.Characters) totalChars += result.Characters;
                if (request.Bytes) totalBytes += result.Bytes;
                // GNU's total for -L is the maximum across inputs, not the sum.
                if (request.MaximumLineLength) totalMaxLineLength = Math.Max(totalMaxLineLength, result.MaximumLineLength);
            }
            catch (Exception exception)
            {
                await error.WriteLineAsync(CountErrorLine(input.Label, exception));

                if (verbose)
                    await error.WriteLineAsync($"Exception Details: {exception.Message}");

                anyFailed = true;
            }
            finally
            {
                await input.Stream.DisposeAsync();
            }
        }

        // Auto is the Total-only-for-multi-input rule; only prints the Total row
        // and nothing else, unlabelled, exactly as GNU does.
        bool printTotal = totalMode switch
        {
            TotalMode.Never => false,
            TotalMode.Always or TotalMode.Only => true,
            _ => inputs.Count > 1
        };

        List<ResultRow> printedRows = totalMode == TotalMode.Only ? [] : rows;
        if (printTotal)
        {
            printedRows.Add(new ResultRow(
                totalMode == TotalMode.Only ? string.Empty : Resources.Output_Labels_Total,
                new CountResult(totalWords, totalLines, totalBytes, totalChars, totalMaxLineLength)));
        }

        await ResultPrintingHelper.PrintTableAsync(output, request, printedRows);

        return anyFailed ? 1 : 0;
    }

    // Errors name their input the way GNU does: a file is quoted by path. The
    // one input with no row label is standard input when it is being counted;
    // an empty file-list entry has no name to quote at all, so its message
    // stands alone.
    private static string OpenErrorLine(CountInput input) =>
        input.Label.Length == 0
            ? $"wcount: {input.OpenError}"
            : $"wcount: '{input.Label}': {input.OpenError}";

    private static string CountErrorLine(string label, Exception exception) =>
        label.Length == 0
            ? $"wcount: standard input: {exception.Message}"
            : $"wcount: '{label}': {exception.Message}";
}
