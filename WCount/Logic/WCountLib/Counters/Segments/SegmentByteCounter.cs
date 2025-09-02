/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Counters.Segments;

/// <summary>
/// 
/// </summary>
public class SegmentByteCounter : ISegmentByteCounter
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    private int CountBytesInt32Worker(StringSegment segment)
    {
        int byteCount = Encoding.Default.GetByteCount(segment.AsSpan());

        return byteCount;
    }
    
    /// <summary>
    /// Returns the total number of 32-bit integers that can be represented by the bytes in all segments.
    /// </summary>
    /// <param name="segments">The segments to count bytes from.</param>
    /// <returns>The total number of 32-bit integers that can be represented by the bytes in all segments.</returns>
    public int CountBytes(IEnumerable<StringSegment> segments)
    {
        int byteCount = 0;
        
        Parallel.ForEach(segments, segment =>
        {
            int bytes = CountBytesInt32Worker(segment);

            Interlocked.Add(ref bytes, byteCount);
        });

        return byteCount;
    }
}