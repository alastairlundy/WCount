/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountCli.Helpers;

/// <summary>
/// Builds and prints the wc-style output table.
/// </summary>
public static class ResultPrintingHelper
{
    /// <summary>
    /// Prints the whole table at once: every row is built before anything is
    /// written, so each count column is sized once across the table — the Total
    /// row included — and the columns line up on every row.
    /// </summary>
    /// <remarks>
    /// Columns are emitted left-to-right in the order lines, words, characters,
    /// bytes, maximum line length, each right-aligned within its own column
    /// width. A row's label follows one space after its last count; a row with
    /// no label — standard input, and the Total row under <c>--total=only</c> —
    /// ends with its counts.
    /// </remarks>
    public static async Task PrintTableAsync(
        TextWriter output, CountRequest request, IReadOnlyList<ResultRow> rows)
    {
        bool[] columns = SelectedColumns(request);
        int[] widths = ColumnWidths(columns, rows);

        List<string> table = [];
        foreach (ResultRow row in rows)
        {
            table.Add(FormatRow(columns, widths, row));
        }

        foreach (string line in table)
        {
            await output.WriteLineAsync(line);
        }
    }

    /// <summary>
    /// One slot per count column, in print order.
    /// </summary>
    private static bool[] SelectedColumns(CountRequest request) =>
        [request.Lines, request.Words, request.Characters, request.Bytes, request.MaximumLineLength];

    private static long[] CountValues(CountResult result) =>
        [result.Lines, result.Words, result.Characters, result.Bytes, result.MaximumLineLength];

    /// <summary>
    /// Sizes every column once, from the widest count in that column across the
    /// whole table. Unselected columns stay at zero and are never printed.
    /// </summary>
    private static int[] ColumnWidths(bool[] columns, IReadOnlyList<ResultRow> rows)
    {
        int[] widths = new int[columns.Length];

        foreach (ResultRow row in rows)
        {
            long[] values = CountValues(row.Result);

            for (int i = 0; i < columns.Length; i++)
            {
                if (!columns[i])
                    continue;

                int length = values[i].ToString(CultureInfo.InvariantCulture).Length;
                if (length > widths[i])
                    widths[i] = length;
            }
        }

        return widths;
    }

    private static string FormatRow(bool[] columns, int[] widths, ResultRow row)
    {
        StringBuilder builder = new();
        long[] values = CountValues(row.Result);

        for (int i = 0; i < columns.Length; i++)
        {
            if (!columns[i])
                continue;

            if (builder.Length > 0)
                builder.Append(' ');

            // Right-align within the column. The first column carries no
            // separator of its own, so a single-column selection prints with no
            // leading space unless a wider row in the same table demands one.
            builder.Append(values[i].ToString(CultureInfo.InvariantCulture).PadLeft(widths[i]));
        }

        if (row.Label.Length > 0)
        {
            builder.Append(' ');
            builder.Append(row.Label);
        }

        return builder.ToString();
    }
}
