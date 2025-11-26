/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Linq;
using AlastairLundy.EnhancedLinq.MsExtensions.StringSegments.Immediate;

namespace AlastairLundy.WCountLib.Counters.Segments;

/// <summary>
/// 
/// </summary>
public class SegmentLineCounter : ISegmentLineCounter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    private int CountLineInt32Worker(StringSegment segment)
    {
        StringSegment environmentNewLineSegment = new StringSegment(Environment.NewLine);
        
        StringSegment[] segments = segment.Split(environmentNewLineSegment);

        return segments.Any() ? segments.Length : 0;
    }
    
    /// <summary>
    /// Counts the number of lines in a collection of string segments.
    /// </summary>
    /// <param name="segments">The collection of string segments to count.</param>
    /// <returns>The total number of lines in the specified collection.</returns>
    public int CountLines(IEnumerable<StringSegment> segments)
    {
        int lineCount = 0;
        
        Parallel.ForEach(segments, segment =>
        {
            int lines = CountLineInt32Worker(segment);

            Interlocked.Add(ref lines, lineCount);
        });

        return lineCount;
    }
}