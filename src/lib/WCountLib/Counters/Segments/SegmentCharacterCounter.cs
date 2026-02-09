/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using DotExtensions.MsExtensions.StringSegments;

namespace WCountLib.Counters.Segments;

/// <summary>
/// 
/// </summary>
public class SegmentCharacterCounter : ISegmentCharacterCounter
{
    private int CountCharactersInt32Worker(StringSegment segment, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;

        byte[] bytes = encoding.GetBytes(segment.ToCharArray());
        
        return encoding.GetCharCount(bytes);
    }

    /// <summary>
    /// Returns the total number of characters in all segments. </summary>
    /// <param name="segments">The segments to count characters from.</param>
    /// <param name="encoding"></param>
    /// <returns>The total number of characters in all segments.</returns>
    public int CountCharacters(IEnumerable<StringSegment> segments, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(segments);
        
        int charCount = 0;

        Parallel.ForEach(segments, segment =>
        {
            int localCharCount = CountCharactersInt32Worker(segment, encoding);

            Interlocked.Add(ref localCharCount, charCount);
        });

        return charCount;
    }
}