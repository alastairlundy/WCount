/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.DotExtensions.MsExtensions.System.StringSegments;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Counters.Segments;

public class SegmentLineCounter : ISegmentLineCounter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    private int CountLineInt32Worker(StringSegment segment)
    {
        int lineCount = 0;

        foreach (char c in segment.ToCharArray())
        {
            if (c.ToString().Equals(Environment.NewLine))
            {
                lineCount++;
            }
        }

        return lineCount;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public int CountLinesInt32(IEnumerable<StringSegment> segments)
    {
        int lineCount = 0;
        
        Parallel.ForEach(segments, segment =>
        {
            int lines = CountLineInt32Worker(segment);

            Interlocked.Add(ref lines, lineCount);
        });

        return lineCount;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public ulong CountLinesUInt64(IEnumerable<StringSegment> segments)
    {
        long charCount = 0;
        
        Parallel.ForEach(segments, segment =>
        {
            long bytes = Convert.ToInt64(CountLineInt32Worker(segment));

            Interlocked.Add(ref bytes, charCount);
        });

        return Convert.ToUInt64(charCount);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public async Task<int> CountLinesInt32Async(IEnumerable<StringSegment> segments)
    {
        int totalLines = 0;
            
        Task wordCountingTask = Task.Run(() =>
        {
            totalLines = CountLinesInt32(segments);
        });
            
        await wordCountingTask;
            
        return totalLines;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public async Task<ulong> CountLinesUInt64Async(IEnumerable<StringSegment> segments)
    {
        ulong totalLines = 0;
            
        Task wordCountingTask = Task.Run(() =>
        {
            totalLines = CountLinesUInt64(segments);
        });
            
        await wordCountingTask;
            
        return totalLines;
    }
}