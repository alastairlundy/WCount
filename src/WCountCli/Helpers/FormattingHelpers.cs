/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountCli.Helpers;

public static class FormattingHelpers
{
    public static string FormatOutput(string str, int requiredSpacing)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append(' ');
        
        int spacesToAdd = Math.Abs(str.Length - requiredSpacing);
    
        for (long i = 0; i < spacesToAdd; i++)
        {
            stringBuilder.Append(' ');
        }
    
        return stringBuilder.ToString();
    }

    public static int CalculateRequiredSpacing(long[] stats)
    {
        int maximum = 0;
    
        foreach (int stat in stats)
        {
            maximum = int.Max(maximum, stat);
        }

        return maximum;
    }
}