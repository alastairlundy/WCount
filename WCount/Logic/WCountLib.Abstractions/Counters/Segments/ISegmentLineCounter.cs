/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Segments;

/// <summary>
/// 
/// </summary>
public interface ISegmentLineCounter
{
    /// <summary>
    /// Returns the total number of lines in all segments.
    /// </summary>
    /// <param name="segments">The segments to count lines from.</param>
    /// <returns>The total number of lines in all segments.</returns>
    int CountLines(IEnumerable<StringSegment> segments);
        
}