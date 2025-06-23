/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Segments
{
    public interface ISegmentLineCounter
    {
        /// <summary>
        /// Returns the total number of lines in all segments.
        /// </summary>
        /// <param name="segments">The segments to count lines from.</param>
        /// <returns>The total number of lines in all segments.</returns>
        int CountLines(IEnumerable<StringSegment> segments);
        
        /// <summary>
        /// Returns the total number of lines (in units of 32-bit integers) in all segments.
        /// This method is asynchronous to allow for non-blocking operations and efficient use of threads. </summary>
        /// <param name="segments">The segments to count lines from.</param>
        /// <returns>The total number of lines in all segments.</returns>
        Task<int> CountLinesAsync(IEnumerable<StringSegment> segments);
        
    }
}