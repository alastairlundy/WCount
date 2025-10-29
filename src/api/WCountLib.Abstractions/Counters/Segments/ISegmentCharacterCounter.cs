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
/// An interface for a service that counts the number of characters in <see cref="StringSegment"/>s.
/// </summary>
/// <remarks>
/// <para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for character counting.</para>
/// </remarks>
public interface ISegmentCharacterCounter
{
    /// <summary>
    /// Returns the total number of characters in all segments. </summary>
    /// <param name="segments">The segments to count characters from.</param>
    /// <returns>The total number of characters in all segments.</returns>
    int CountCharacters(IEnumerable<StringSegment> segments);
}