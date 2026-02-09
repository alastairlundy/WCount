/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace WCountLib.Abstractions.Counters.Segments;

/// <summary>
/// An interface for a service that counts the number of bytes in <see cref="StringSegment"/>s.
/// </summary>
/// <remarks>
/// <para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for byte counting.</para>
/// </remarks>
public interface ISegmentByteCounter
{
    /// <summary>
    /// Returns the total number of bytes in all segments. </summary>
    /// <param name="segments">The segments to count bytes from.</param>
    /// <param name="encoding"></param>
    /// <returns>The total number of bytes in all segments.</returns>
    int CountBytes(IEnumerable<StringSegment> segments, Encoding? encoding = null);
}