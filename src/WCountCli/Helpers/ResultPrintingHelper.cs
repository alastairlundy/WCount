/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace WCountCli.Helpers;

public static class ResultPrintingHelper
{
    public static async Task PrintCustomResultLine(string file, TextWriter output, long? lineCount = null, long? wordCount = null,
        long? characterCount = null, long? byteCount = null)
    {
        StringBuilder stringBuilder = new();

        List<long> stats = new();

        if (lineCount is not null)
            stats.Add(lineCount.Value);

        if (wordCount is not null)
            stats.Add(wordCount.Value);

        if (characterCount is not null)
            stats.Add(characterCount.Value);

        if (byteCount is not null)
            stats.Add(byteCount.Value);

        int requiredSpacing = stats.Count > 0 ? FormattingHelpers.CalculateRequiredSpacing(stats.ToArray()) : 0;

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

        await output.WriteLineAsync(stringBuilder.ToString());
    }


    public static async Task PrintDefaultResultLine(string file, TextWriter output,
        long lineCount, long wordCount, long characterCount)
    {
        StringBuilder stringBuilder = new();

        int requiredSpacing = FormattingHelpers.CalculateRequiredSpacing(new long[] { lineCount, wordCount, characterCount });

        stringBuilder.Append(FormattingHelpers
            .FormatOutput(lineCount.ToString(CultureInfo.CurrentCulture), requiredSpacing).TrimStart(' '));

        stringBuilder.Append(FormattingHelpers.FormatOutput(wordCount.ToString(CultureInfo.CurrentCulture),
            requiredSpacing));

        stringBuilder.Append(FormattingHelpers.FormatOutput(characterCount.ToString(CultureInfo.CurrentCulture),
            requiredSpacing));

        stringBuilder.Append(' ');
        stringBuilder.Append(file);

        await output.WriteLineAsync(stringBuilder.ToString());
    }
}
