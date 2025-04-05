/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using AlastairLundy.WCountLib.Abstractions.Detectors.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Counters.Segments;

public class SegmentWordCounter : ISegmentWordCounter
{
    private readonly ISegmentWordDetector _segmentWordDetector;

    public SegmentWordCounter(ISegmentWordDetector segmentWordDetector)
    {
        _segmentWordDetector = segmentWordDetector;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public int CountWordsInt32(IEnumerable<StringSegment> segments)
    {
        int totalWords = 0;

        StringSegment[] stringSegments = segments as StringSegment[] ?? segments.ToArray();

        int segmentCount = stringSegments.Length;

        if (segmentCount < 100)
        {
            Parallel.ForEach(stringSegments, segment =>
            {
                if (_segmentWordDetector.IsStringAWord(segment, false))
                {
                    Interlocked.Increment(ref totalWords);
                }
            });
        }
        else
        {
            OrderablePartitioner<StringSegment> partitioner = Partitioner.Create(stringSegments, EnumerablePartitionerOptions.NoBuffering);
                
            Parallel.ForEach(partitioner, segment =>
            {
                if (_segmentWordDetector.IsStringAWord(segment, false))
                {
                    Interlocked.Increment(ref totalWords);
                }
            });
        }
            
#if NET5_0_OR_GREATER
            return totalWords;
#else
        return totalWords;            
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public ulong CountWordsUInt64(IEnumerable<StringSegment> segments)
    {
        long totalWords = 0;

        StringSegment[] stringSegments = segments as StringSegment[] ?? segments.ToArray();

        int segmentCount = stringSegments.Length;

        if (segmentCount < 100)
        {
            Parallel.ForEach(stringSegments, segment =>
            {
                if (_segmentWordDetector.IsStringAWord(segment, false))
                {
                    Interlocked.Increment(ref totalWords);
                }
            });
        }
        else
        {
            OrderablePartitioner<StringSegment> partitioner = Partitioner.Create(stringSegments, EnumerablePartitionerOptions.NoBuffering);
                
            Parallel.ForEach(partitioner, segment =>
            {
                if (_segmentWordDetector.IsStringAWord(segment, false))
                {
                    Interlocked.Increment(ref totalWords);
                }
            });
        }
            
#if NET5_0_OR_GREATER
            return Convert.ToUInt64(totalWords);
#else
        return Convert.ToUInt64(totalWords);            
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public async Task<int> CountWordsInt32Async(IEnumerable<StringSegment> segments)
    {
        int totalWords = 0;
            
        Task wordCountingTask = Task.Run(() =>
        {
            totalWords = CountWordsInt32(segments);
        });
            
        await wordCountingTask;
            
        return totalWords;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public async Task<ulong> CountWordsUInt64Async(IEnumerable<StringSegment> segments)
    {
        ulong totalWords = 0;
            
        Task wordCountingTask = Task.Run(() =>
        {
            totalWords = CountWordsUInt64(segments);
        });
            
        await wordCountingTask;
            
        return totalWords;
    }
}