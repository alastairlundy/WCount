/*
    WCountLib
    Copyright (C) 2024 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using WCountLib.Abstractions.Counters;
using WCountLib.Localizations;

// ReSharper disable RedundantIfElseBlock

namespace WCountLib.Counters
{
    public class LineCounter : ILineCounter
    {

        /// <summary>
        /// Gets the number of lines in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <returns>the number of lines in a string.</returns>
        public int CountLines(string s)
        {
            int totalCount = 0;
            foreach (char c in s)
            {
                if (c.Equals('\n') || c.Equals(char.Parse("\r\n")) || c.ToString().Equals(Environment.NewLine))
                {
                    totalCount++;
                }
            }

            return totalCount;
        }

        /// <summary>
        /// Gets the number of lines in an IEnumerable of strings.
        /// </summary>
        /// <param name="enumerable">The IEnumerable to be searched.</param>
        /// <returns>the number of lines in the specified IEnumerable.</returns>
        public int CountLines(IEnumerable<string> enumerable)
        {
            int totalCount = 0;

            foreach (string s in enumerable)
            {
                totalCount += CountLines(s);
            }

            return totalCount;
        }
    }
}