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
    public interface ISegmentCharacterCounter
    {
        /// <summary>
        /// Returns the total number of characters in all segments. </summary>
        /// <param name="segments">The segments to count characters from.</param>
        /// <returns>The total number of characters in all segments.</returns>
        int CountCharacters(IEnumerable<StringSegment> segments);
        
        /// <summary>
        /// Returns the total number of characters (in units of 64-bit unsigned integers) in all segments. This method is asynchronous to allow for non-blocking operations and efficient use of threads. </summary>
        /// <param name="segments">The segments to count characters from.</param>
        /// <returns>The total number of characters (in units of 64-bit unsigned integers) in all segments.</returns>
        Task<UInt64> CountCharactersAsync(IEnumerable<StringSegment> segments);
    }
}