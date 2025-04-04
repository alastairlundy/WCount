/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using AlastairLundy.DotExtensions.Strings;
using AlastairLundy.WCountLib.Abstractions.Detectors.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Detectors.Segments;

public class SegmentWordDetector : ISegmentWordDetector
{
    public bool IsStringAWord(StringSegment segment, bool countStringsWithSpacesAsWords = false)
    {
        bool containsSpaceSeparatedSubStrings = segment.Split([' ']).Count() > 1 == false;

        bool output = containsSpaceSeparatedSubStrings;
        
        if (StringSegment.IsNullOrEmpty(segment) == true || 
            containsSpaceSeparatedSubStrings && countStringsWithSpacesAsWords == false)
        {
            output = false;
        }
        
        ReadOnlySpan<char> span = segment.AsSpan();
        
        if(span.ToArray().All(c => c.IsSpecialCharacter() == true))
        {
            output = false;
        }

        if (segment.Split([' ']).Count() == 1 && countStringsWithSpacesAsWords == false)
        {
            output = true;
        }

        return output;
    }

    public bool IsStringAWord(StringSegment segment, IEnumerable<char> delimitersToExclude, bool countStringsWithSpacesAsWords = false)
    {
        throw new System.NotImplementedException();
    }
}