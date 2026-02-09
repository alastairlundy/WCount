/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountLib.Counters;

/// <summary>
/// 
/// </summary>
public class ByteCounter : IByteCounter
{
    /// <summary>
    /// Gets the number of bytes in a string.
    /// </summary>
    /// <param name="text">The string to be searched.</param>
    /// <param name="encoding">The type of encoding to use to decode the bytes.</param>
    /// <returns>the number of bytes in the string.</returns>
    public int CountBytes(string text, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(encoding);
        
        int byteCount = encoding.GetByteCount(text);
        
        return byteCount;
    }
}