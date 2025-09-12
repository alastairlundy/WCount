/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */


using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Segments;

/// <summary>
/// An interface for a service that counts the number of words in <see cref="StringSegment"/>s.
/// </summary>
/// <remarks>
/// <para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for word counting.</para>
/// </remarks>
public interface ISegmentWordCounter
{
    /// <summary>
    /// Counts the number of words in a list of string segments.
    /// </summary>
    /// <param name="segments">The list of string segments to count words from.</param>
    /// <returns>The total number of words found in the input segments.</returns>
    int CountWords(IEnumerable<StringSegment> segments);

}