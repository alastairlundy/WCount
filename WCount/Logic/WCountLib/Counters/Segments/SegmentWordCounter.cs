/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using AlastairLundy.WCountLib.Abstractions.Detectors.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Counters.Segments;

/// <summary>
/// 
/// </summary>
public class SegmentWordCounter : ISegmentWordCounter
{
    private readonly ISegmentWordDetector _segmentWordDetector;

    /// <summary>
    /// Initializes a new instance of the SegmentWordCounter class using the provided segment word detector.
    /// </summary>
    /// <param name="segmentWordDetector">The segment word detector to use for counting words.</param>
    public SegmentWordCounter(ISegmentWordDetector segmentWordDetector)
    {
        _segmentWordDetector = segmentWordDetector;
    }
    
    /// <summary>
    /// Counts the number of words in a collection of string segments.
    /// </summary>
    /// <param name="segments">The collection of string segments to count.</param>
    /// <returns>The total number of words in the specified collection.</returns>
    public int CountWords(IEnumerable<StringSegment> segments)
    {
        int totalWords = 0;

        StringSegment[] stringSegments = segments as StringSegment[] ?? segments.ToArray();

        int segmentCount = stringSegments.Length;

        if (segmentCount < 100)
        {
            Parallel.ForEach(stringSegments, segment =>
            {
                if (_segmentWordDetector.IsSegmentAWord(segment, false))
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
                if (_segmentWordDetector.IsSegmentAWord(segment, false))
                {
                    Interlocked.Increment(ref totalWords);
                }
            });
        }
            
        return totalWords;
    }
}