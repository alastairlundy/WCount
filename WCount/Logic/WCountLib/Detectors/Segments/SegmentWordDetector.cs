/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Linq;


using AlastairLundy.DotExtensions.Strings;
using AlastairLundy.DotExtensions.MsExtensions.StringSegments;

using AlastairLundy.EnhancedLinq.MsExtensions.StringSegments.Deferred;
using AlastairLundy.EnhancedLinq.MsExtensions.StringSegments.Immediate;

using AlastairLundy.WCountLib.Abstractions.Detectors.Segments;

using Microsoft.Extensions.Primitives;
// ReSharper disable RedundantBoolCompare
// ReSharper disable ConvertClosureToMethodGroup

namespace AlastairLundy.WCountLib.Detectors.Segments;

public class SegmentWordDetector : ISegmentWordDetector
{
    
    /// <summary>
    /// Checks if a string segment represents a single word.
    /// </summary>
    /// <param name="segment">The string segment to check.</param>
    /// <param name="countStringsWithSpacesAsWords">Optional flag indicating whether strings with spaces should be considered as words. Defaults to false if not provided.</param>
    /// <returns>True if the string segment represents a single word, false otherwise.</returns>
    public bool IsSegmentAWord(StringSegment segment, bool countStringsWithSpacesAsWords = false)
    {
        if (StringSegment.IsNullOrEmpty(segment) || segment.All(c => char.IsWhiteSpace(c)))
            return false;
        
        if (segment.Length == 1)
        {
            return segment[0].IsSpecialCharacter() == false;
        }

        int separatorCount = 0;
        int specialCharCount = 0;

        bool charValidity = false;

        foreach (char c in EnhancedLinqSegmentDeferred.AsEnumerable(segment))
        {
            if(char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsAsciiLetter(c) ||
               char.IsSymbol(c))
                charValidity = true;

            if (char.IsSeparator(c))
                separatorCount++;
            
            if(char.IsPunctuation(c))
                specialCharCount++;
        }
        
        if (separatorCount == segment.Length || specialCharCount == segment.Length)
            return false;

        bool containsSpaceSeparatedSubStrings = segment.Contains(' ') == false;

        if (countStringsWithSpacesAsWords && containsSpaceSeparatedSubStrings && charValidity)
            return true;

        return charValidity;
    }

    /// <summary>
    /// Determines whether a string segment contains one or more words.
    /// </summary>
    /// <param name="segment">The <see cref="StringSegment"/> to look for.</param>
    /// <param name="wordSeparator">The separator char to look for between words.</param>
    /// <param name="countSegmentsWithSpacesAsWords">Whether to count StringSegments that contain spaces as words. Set to false by default.</param>
    /// <returns>True if one or more words was found in the string segment, false otherwise.</returns>
    public bool DoesSegmentContainWords(StringSegment segment, char wordSeparator, bool countSegmentsWithSpacesAsWords = false)
    {
        if (StringSegment.IsNullOrEmpty(segment) || segment.All(c => char.IsWhiteSpace(c)))
            return false;

        StringSegment[] possibleWords = EnhancedLinqSegmentImmediate.Split(segment, wordSeparator);
        
        bool foundWords = possibleWords.Length > 0;

        if (foundWords)
        {
            return possibleWords.Any(x => IsSegmentAWord(x));
        }

        return IsSegmentAWord(segment, countSegmentsWithSpacesAsWords);
    }
}