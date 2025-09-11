/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Text;
using AlastairLundy.WCountLib.Abstractions.Counters;

// ReSharper disable RedundantIfElseBlock

namespace AlastairLundy.WCountLib.Counters;

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
        int byteCount;

        if (Equals(encoding, Encoding.Unicode))
        {
            byteCount = Encoding.Unicode.GetByteCount(text);
        }
        else if (Equals(encoding, Encoding.UTF32))
        {
            byteCount = Encoding.UTF32.GetByteCount(text);
        }
        else if (Equals(encoding, Encoding.UTF8))
        {
            byteCount = Encoding.UTF8.GetByteCount(text);
        }
        else if (Equals(encoding, Encoding.ASCII))
        {
            byteCount = Encoding.ASCII.GetByteCount(text);
        }
        else if (Equals(encoding, Encoding.BigEndianUnicode))
        {
            byteCount = Encoding.BigEndianUnicode.GetByteCount(text);
        }
#if NET8_0_OR_GREATER
        else if (Equals(encoding, Encoding.Latin1))
        {
            byteCount = Encoding.Latin1.GetByteCount(text);
        }
#endif
        else
        {
            byteCount = Encoding.Default.GetByteCount(text);
        }

        return byteCount;
    }
}