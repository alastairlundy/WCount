/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountCli.Helpers;

public static class ResultPrintingHelper
{
    /// <summary>
    /// Prints a single wc-style result row.
    /// Columns are emitted left-to-right in the order: lines, words, bytes, characters, maximum line length.
    /// </summary>
    public static async Task PrintRow(string file, TextWriter output, CountRequest request,
        long lineCount, long wordCount, long characterCount, long byteCount, long maxLineLength)
    {
        List<long> values = [];

        if (request.Lines)
            values.Add(lineCount);
        if (request.Words)
            values.Add(wordCount);
        if (request.Bytes)
            values.Add(byteCount);
        if (request.Characters)
            values.Add(characterCount);
        if (request.MaximumLineLength)
            values.Add(maxLineLength);

        int spacing = values.Count > 0 ? CalculateRequiredSpacing(values.ToArray()) : 0;
        StringBuilder sb = new();

        if (request.Lines)
            sb.Append(FormatOutput(lineCount.ToString(CultureInfo.CurrentCulture), spacing).TrimStart(' '));
        if (request.Words)
            sb.Append(FormatOutput(wordCount.ToString(CultureInfo.CurrentCulture), spacing));
        if (request.Bytes)
            sb.Append(FormatOutput(byteCount.ToString(CultureInfo.CurrentCulture), spacing));
        if (request.Characters)
            sb.Append(FormatOutput(characterCount.ToString(CultureInfo.CurrentCulture), spacing));
        if (request.MaximumLineLength)
            sb.Append(FormatOutput(maxLineLength.ToString(CultureInfo.CurrentCulture), spacing));

        sb.Append(' ');
        sb.Append(file);

        await output.WriteLineAsync(sb.ToString());
    }

    private static string FormatOutput(string str, int requiredSpacing)
    {
        StringBuilder sb = new();
        sb.Append(' ');

        int padding = requiredSpacing - str.Length;
        if (padding > 0)
            sb.Append(' ', padding);

        sb.Append(str);
        return sb.ToString();
    }

    private static int CalculateRequiredSpacing(long[] stats)
    {
        int maximum = 0;

        foreach (long stat in stats)
        {
            int len = stat.ToString(CultureInfo.CurrentCulture).Length;
            if (len > maximum)
                maximum = len;
        }

        return maximum;
    }
}