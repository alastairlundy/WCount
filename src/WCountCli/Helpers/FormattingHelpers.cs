/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Text;
using System.Globalization;

namespace WCountCli.Helpers;

public static class FormattingHelpers
{
    public static string FormatOutput(string str, int requiredSpacing)
    {
        // Format a single column: a leading separator space, optional padding, then the value.
        StringBuilder sb = new();
        sb.Append(' ');

        int padding = requiredSpacing - str.Length;
        if (padding > 0)
            sb.Append(' ', padding);

        sb.Append(str);
        return sb.ToString();
    }

    public static int CalculateRequiredSpacing(long[] stats)
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
