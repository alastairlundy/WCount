/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Counters.Segments;

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
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public int CountBytesInt32(IEnumerable<StringSegment> segments)
    {
        int byteCount = 0;
        
        Parallel.ForEach(segments, segment =>
        {
            int bytes = CountBytesInt32Worker(segment);

            Interlocked.Add(ref bytes, byteCount);
        });

        return byteCount;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public ulong CountBytesUInt64(IEnumerable<StringSegment> segments)
    {
        long byteCount = 0;
        
        Parallel.ForEach(segments, segment =>
        {
            long bytes = Convert.ToInt64(CountBytesInt32Worker(segment));

            Interlocked.Add(ref bytes, byteCount);
        });

        return Convert.ToUInt64(byteCount);
    }

    public async Task<int> CountBytesInt32Async(IEnumerable<StringSegment> segments)
    {
        int byteCount = 0;

        
    }

    public async Task<ulong> CountBytesUInt64Async(IEnumerable<StringSegment> segments)
    {
        long byteCount = 0;
        
        
        
        
    }
}