/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using Microsoft.Extensions.Primitives;

namespace WCountLib.Abstractions.Detectors.Segments;

/// <summary>
/// An interface for a word detecting service that uses <see cref="StringSegment"/> instead of <see cref="string"/> for potentially better performance.
/// </summary>
public interface ISegmentWordDetector
{
    /// <summary>
    /// Determines whether a StringSegment is a word or not.
    /// </summary>
    /// <param name="segment">The segment to be searched for a word.</param>
    /// <param name="countSegmentsWithSpacesAsWords">Whether to count StringSegments that contain spaces as words. Set to false by default.</param>
    /// <returns>True if the StringSegment is a word; false otherwise.</returns>
    bool IsSegmentAWord(StringSegment segment, bool countSegmentsWithSpacesAsWords = false);

    /// <summary>
    /// Determines whether a string segment contains one or more words.
    /// </summary>
    /// <param name="segment">The <see cref="StringSegment"/> to look for.</param>
    /// <param name="wordSeparator">The separator char to look for between words.</param>
    /// <param name="countSegmentsWithSpacesAsWords">Whether to count StringSegments that contain spaces as words. Set to false by default.</param>
    /// <returns>True if one or more words was found in the string segment, false otherwise.</returns>
    bool DoesSegmentContainWords(StringSegment segment, char wordSeparator, bool countSegmentsWithSpacesAsWords = false);
}