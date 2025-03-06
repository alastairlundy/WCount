/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Detectors;

/// <summary>
/// A word detecting service that uses String Segments instead of Strings for potentially better performance.
/// </summary>
public interface ISegmentWordDetector
{
    /// <summary>
    /// Determines whether a string is a word or not.
    /// </summary>
    /// <param name="segment">The segment to be searched for a word.</param>
    /// <param name="countStringsWithSpacesAsWords">Whether to count strings that contain spaces as words. Set to false by default.</param>
    /// <returns>True if the string is a word; false otherwise.</returns>
    bool IsStringAWord(StringSegment segment, bool countStringsWithSpacesAsWords = false);

    /// <summary>
    /// Determines whether a string segment is a word or not.
    /// </summary>
    /// <param name="segment">The segment to be searched for a word.</param>
    /// <param name="delimitersToExclude">Delimiters that valid words should not contain.</param>
    /// <param name="countStringsWithSpacesAsWords">Whether to count strings that contain spaces as words. Set to false by default.</param>
    /// <returns>True if the string is a word; false otherwise.</returns>
    bool IsStringAWord(StringSegment segment, IEnumerable<char> delimitersToExclude, bool countStringsWithSpacesAsWords = false);
}