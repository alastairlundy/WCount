/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Linq;

using AlastairLundy.DotExtensions.MsExtensions.StringSegments;
using AlastairLundy.DotExtensions.Strings;

using AlastairLundy.WCountLib.Abstractions.Detectors.Segments;

using Microsoft.Extensions.Primitives;
// ReSharper disable RedundantBoolCompare

namespace AlastairLundy.WCountLib.Detectors.Segments;

public class SegmentWordDetector : ISegmentWordDetector
{
    
    /// <summary>
    /// Checks if a string segment represents a single word.
    /// </summary>
    /// <param name="segment">The string segment to check.</param>
    /// <param name="countStringsWithSpacesAsWords">Optional flag indicating whether strings with spaces should be considered as words. Defaults to false if not provided.</param>
    /// <returns>True if the string segment represents a single word, false otherwise.</returns>
    public bool IsStringAWord(StringSegment segment, bool countStringsWithSpacesAsWords = false)
    {
        bool containsSpaceSeparatedSubStrings = segment.Contains(' ') == false;

        bool output = containsSpaceSeparatedSubStrings;
        
        if (StringSegment.IsNullOrEmpty(segment) == true || 
            containsSpaceSeparatedSubStrings && countStringsWithSpacesAsWords == false)
        {
            output = false;
        }
        
        if(segment.ToCharArray().All(c => c.IsSpecialCharacter() == true))
        {
            output = false;
        }

        if (containsSpaceSeparatedSubStrings && countStringsWithSpacesAsWords == false)
        {
            output = true;
        }
        
        return output;
    }

    /// <summary>
    /// Checks if a string segment represents a single word.
    /// </summary>
    /// <param name="segment">The string segment to check.</param>
    /// <param name="delimitersToExclude">Optional delimiters that should be ignored when checking for words. If not provided, default delimiters will be used.</param>
    /// <param name="countStringsWithSpacesAsWords">Optional flag indicating whether strings with spaces should be considered as words. Defaults to false if not provided.</param>
    /// <returns>True if the string segment represents a single word, false otherwise.</returns>
    public bool IsStringAWord(StringSegment segment, IEnumerable<char> delimitersToExclude, bool countStringsWithSpacesAsWords = false)
    {
        bool containsSpaceSeparatedSubstrings = segment.Contains(' ');
        bool output = false;
            
        if (StringSegment.IsNullOrEmpty(segment) == false ||
            containsSpaceSeparatedSubstrings && countStringsWithSpacesAsWords == false)
        {
            output = false;    
        }

        if (delimitersToExclude.Select(x => segment.Contains(x)).Any())
        {
            output = false;
        }

        return output;
    }
}