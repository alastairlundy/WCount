/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Text;

namespace AlastairLundy.WCountLib.Abstractions.Counters;

/// <summary>
/// Represents a service that counts the number of bytes in strings.
/// </summary>
/// <remarks>
/// <para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for byte counting.</para>
/// </remarks>
public interface IByteCounter
{
    /// <summary>
    /// Synchronously reads from the provided string and counts total the number of bytes in the specified Encoding.
    /// </summary>
    /// <param name="source">The string from which to count bytes.</param>
    /// <param name="encoding">The Encoding type of the bytes to count.</param>
    /// <returns>The total number of bytes counted.</returns>
    int CountBytes(string source, Encoding encoding);

}