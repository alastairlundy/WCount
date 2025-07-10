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

using AlastairLundy.DotExtensions.MsExtensions.System.StringSegments;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Counters.Segments;

/// <summary>
/// 
/// </summary>
public class SegmentCharacterCounter : ISegmentCharacterCounter
{

    private int CountCharactersInt32Worker(StringSegment segment)
    {
        byte[] bytes = Encoding.Default.GetBytes(segment.ToCharArray());

        return Encoding.Default.GetCharCount(bytes);
    }
    
    /// <summary>
    /// Returns the total number of characters in all segments. </summary>
    /// <param name="segments">The segments to count characters from.</param>
    /// <returns>The total number of characters in all segments.</returns>
    public int CountCharacters(IEnumerable<StringSegment> segments)
    {
        int charCount = 0;
        
        Parallel.ForEach(segments, segment =>
        {
            int bytes = CountCharactersInt32Worker(segment);

            Interlocked.Add(ref bytes, charCount);
        });

        return charCount;
    }

    /// <summary>
    /// Returns the total number of characters in all segments. This method is asynchronous to allow for non-blocking operations and efficient use of threads. </summary>
    /// <param name="segments">The segments to count characters from.</param>
    /// <returns>The total number of characters in all segments.</returns>
    public async Task<int> CountCharactersAsync(IEnumerable<StringSegment> segments)
    {
        int totalChars = 0;
            
        Task wordCountingTask = Task.Run(() =>
        {
            totalChars = CountCharacters(segments);
        });
            
        await wordCountingTask;
            
        return totalChars;
    }
}