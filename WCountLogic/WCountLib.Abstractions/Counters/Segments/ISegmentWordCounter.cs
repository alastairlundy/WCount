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
    
    public interface ISegmentWordCounter
    {
        /// <summary>
        /// Counts the number of words in a list of string segments.
        /// </summary>
        /// <param name="segments">The list of string segments to count words from.</param>
        /// <returns>The total number of words found in the input segments.</returns>
        int CountWords(IEnumerable<StringSegment> segments);
        
        /// <summary>
        /// Asynchronously counts the number of words in a list of string segments and returns the result.
        /// </summary>
        /// <param name="segments">The list of string segments to count words from.</param>
        /// <returns>A task that completes with the total number of words found in the input segments.</returns>
        Task<int> CountWordsAsync(IEnumerable<StringSegment> segments);

    }
}