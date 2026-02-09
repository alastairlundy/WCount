/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Linq;
using AlastairLundy.DotExtensions.MsExtensions.Exceptions;
using AlastairLundy.DotExtensions.Strings;
using AlastairLundy.EnhancedLinq.MsExtensions.StringSegments.Deferred;
using AlastairLundy.EnhancedLinq.MsExtensions.StringSegments.Immediate;
using WCountLib.Abstractions.Detectors.Segments;

// ReSharper disable RedundantBoolCompare
// ReSharper disable ConvertClosureToMethodGroup

namespace WCountLib.Detectors.Segments;

/// <summary>
/// 
/// </summary>
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
        ArgumentException.ThrowIfNullOrEmpty(segment);
        
        if (segment.Length == 1)
            return !char.IsSpecialCharacter(segment[0]);
        
        int regularChars = 0;
        int separatorCount = 0;
        int specialCharCount = 0;
        int whiteSpaceCharCount = 0;

        foreach (char c in segment.AsEnumerable())
        {
            if (char.IsLetterOrDigit(c) || char.IsAsciiLetterOrDigit(c))
                regularChars++;
           
            if (char.IsWhiteSpace(c))
                whiteSpaceCharCount++;
           
            if (char.IsSeparator(c))
                separatorCount++;
            
            if(char.IsPunctuation(c))
                specialCharCount++;
            
            if (whiteSpaceCharCount > 0 && !countStringsWithSpacesAsWords ||
                separatorCount == segment.Length || specialCharCount == segment.Length)
                return false;
        }

        if (countStringsWithSpacesAsWords && whiteSpaceCharCount > 0 && regularChars > 0)
            return true;

        return regularChars > 0;
    }

    /// <summary>
    /// Determines whether a string segment contains one or more words.
    /// </summary>
    /// <param name="segment">The <see cref="StringSegment"/> to look for.</param>
    /// <param name="wordSeparator">The separator char to look for between words.</param>
    /// <param name="countSegmentsWithSpacesAsWords">Whether to count StringSegments that contain spaces as words. Set to false by default.</param>
    /// <returns>True if one or more words were found in the string segment, false otherwise.</returns>
    public bool DoesSegmentContainWords(StringSegment segment, char wordSeparator, 
        bool countSegmentsWithSpacesAsWords = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(segment);
        
        StringSegment[] possibleWords = segment.Split(wordSeparator);
        
        return possibleWords.Any(x => IsSegmentAWord(x, countSegmentsWithSpacesAsWords));
    }
}