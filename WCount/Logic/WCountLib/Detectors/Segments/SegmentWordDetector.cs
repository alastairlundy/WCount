/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Linq;

using AlastairLundy.DotExtensions.MsExtensions.StringSegments;
using AlastairLundy.DotExtensions.Strings;
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
    /// 
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="wordSeparator"></param>
    /// <param name="countStringsWithSpacesAsWords"></param>
    /// <returns></returns>
    public bool DoesSegmentContainWords(StringSegment segment, char wordSeparator, bool countStringsWithSpacesAsWords = false)
    {
        if (StringSegment.IsNullOrEmpty(segment) || segment.All(c => char.IsWhiteSpace(c)))
            return false;

        StringSegment[] possibleWords = EnhancedLinqSegmentImmediate.Split(segment, wordSeparator);
        
        bool foundWords = possibleWords.Length > 0;

        if (foundWords)
        {
            return possibleWords.Any(x => IsSegmentAWord(x));
        }
        else
        {
            return IsSegmentAWord(segment, countStringsWithSpacesAsWords);
        }
    }
}