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
    /// Builds a <see cref="CountSelection"/> bitmask from the four CLI boolean flags.
    /// </summary>
    public static CountSelection ToSelection(bool line, bool word, bool character, bool @byte)
    {
        CountSelection selection = CountSelection.None;
        if (line) selection |= CountSelection.Lines;
        if (word) selection |= CountSelection.Words;
        if (character) selection |= CountSelection.Characters;
        if (@byte) selection |= CountSelection.Bytes;
        return selection;
    }

    /// <summary>
    /// Prints a single wc-style result row.
    /// Columns are emitted left-to-right in the order: lines, words, bytes, characters.
    /// </summary>
    public static async Task PrintRow(string file, TextWriter output, CountSelection selection,
        long? lineCount, long? wordCount, long? characterCount, long? byteCount)
    {
        List<long> values = new();

        if ((selection & CountSelection.Lines) != 0)
            values.Add(lineCount ?? 0);
        if ((selection & CountSelection.Words) != 0)
            values.Add(wordCount ?? 0);
        if ((selection & CountSelection.Bytes) != 0)
            values.Add(byteCount ?? 0);
        if ((selection & CountSelection.Characters) != 0)
            values.Add(characterCount ?? 0);

        int spacing = values.Count > 0 ? CalculateRequiredSpacing(values.ToArray()) : 0;
        StringBuilder sb = new();

        if ((selection & CountSelection.Lines) != 0)
            sb.Append(FormatOutput((lineCount ?? 0).ToString(CultureInfo.CurrentCulture), spacing).TrimStart(' '));
        if ((selection & CountSelection.Words) != 0)
            sb.Append(FormatOutput((wordCount ?? 0).ToString(CultureInfo.CurrentCulture), spacing));
        if ((selection & CountSelection.Bytes) != 0)
            sb.Append(FormatOutput((byteCount ?? 0).ToString(CultureInfo.CurrentCulture), spacing));
        if ((selection & CountSelection.Characters) != 0)
            sb.Append(FormatOutput((characterCount ?? 0).ToString(CultureInfo.CurrentCulture), spacing));

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
