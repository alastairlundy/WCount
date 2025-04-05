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
            return totalWords;
#else
        return Convert.ToUInt64(totalWords);            
#endif
    }

}