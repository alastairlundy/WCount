/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Concurrent;
using WCountLib.Abstractions.Counters.Segments;
using WCountLib.Abstractions.Detectors.Segments;

namespace WCountLib.Counters.Segments;

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
        ArgumentNullException.ThrowIfNull(segments);
        
        int totalWords = 0;
        
        OrderablePartitioner<StringSegment> partitioner =
            Partitioner.Create(segments, EnumerablePartitionerOptions.NoBuffering);

        Parallel.ForEach(partitioner, segment =>
        {
            if (_segmentWordDetector.IsSegmentAWord(segment))
            {
                Interlocked.Increment(ref totalWords);
            }
        });

        return totalWords;
    }
}