/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Linq;

using AlastairLundy.DotExtensions.MsExtensions.System.StringSegments;
using AlastairLundy.DotExtensions.Strings;
using AlastairLundy.WCountLib.Abstractions.Detectors.Segments;

using Microsoft.Extensions.Primitives;
// ReSharper disable RedundantBoolCompare

namespace AlastairLundy.WCountLib.Detectors.Segments;

public class SegmentWordDetector : ISegmentWordDetector
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="countStringsWithSpacesAsWords"></param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="delimitersToExclude"></param>
    /// <param name="countStringsWithSpacesAsWords"></param>
    /// <returns></returns>
    public bool IsStringAWord(StringSegment segment, IEnumerable<char> delimitersToExclude, bool countStringsWithSpacesAsWords = false)
    {
        bool containsSpaceSeparatedSubstrings = segment.Contains(' ');
        bool output = false;
            
        if (StringSegment.IsNullOrEmpty(segment) == false ||
            containsSpaceSeparatedSubstrings && countStringsWithSpacesAsWords == false)
        {
            output = false;    
        }

        if (delimitersToExclude.Select(x => segment.ToCharArray().Contains(x)).Any() == true)
        {
            output = false;
        }

        return output;
    }
}