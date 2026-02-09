/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using WCountLib.Abstractions.Counters.Segments;

namespace WCountLib.Counters.Segments;

/// <summary>
/// 
/// </summary>
public class SegmentByteCounter : ISegmentByteCounter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    private int CountBytesInt32Worker(StringSegment segment, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(segment);
        encoding ??= Encoding.Default;
        
        return encoding.GetByteCount(segment.AsSpan());
    }

    /// <summary>
    /// Returns the total number of 32-bit integers that can be represented by the bytes in all segments.
    /// </summary>
    /// <param name="segments">The segments to count bytes from.</param>
    /// <param name="encoding"></param>
    /// <returns>The total number of 32-bit integers that can be represented by the bytes in all segments.</returns>
    public int CountBytes(IEnumerable<StringSegment> segments, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(segments);
        encoding ??= Encoding.Default;
        
        int byteCount = 0;
        
        Parallel.ForEach(segments, segment =>
        {
            int bytes = CountBytesInt32Worker(segment, encoding);

            Interlocked.Add(ref bytes, byteCount);
        });

        return byteCount;
    }
}