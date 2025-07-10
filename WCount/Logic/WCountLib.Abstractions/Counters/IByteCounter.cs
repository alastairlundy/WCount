/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AlastairLundy.WCountLib.Abstractions.Counters;

/// <summary>
/// Represents a service that counts the number of bytes in TextReaders.
/// </summary>
/// <remarks>
/// <para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for byte counting.</para>
/// </remarks>
public interface IByteCounter
{
    /// <summary>
    /// Synchronously reads from the provided TextReader and counts total the number of bytes in the specified Encoding.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count bytes.</param>
    /// <param name="encoding">The Encoding type of the bytes to count.</param>
    /// <returns>The total number of bytes counted.</returns>
    int CountBytes(TextReader textReader, Encoding encoding);
        
    /// <summary>
    /// Synchronously reads from the provided string and counts total the number of bytes in the specified Encoding.
    /// </summary>
    /// <param name="text">The string from which to count bytes.</param>
    /// <param name="encoding">The Encoding type of the bytes to count.</param>
    /// <returns>The total number of bytes counted.</returns>
    int CountBytes(string text, Encoding encoding);
        
    /// <summary>
    /// Asynchronously reads from the provided TextReader and counts the total number of bytes in the specified Encoding.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count bytes.</param>
    /// <param name="encoding">The Encoding type of the bytes to count.</param>
    /// <returns>The total number of bytes counted.</returns>
    Task<int> CountBytesAsync(TextReader textReader, Encoding encoding);
        
    /// <summary>
    /// Asynchronously reads from the provided string and counts the total number of bytes in the specified Encoding.
    /// </summary>
    /// <param name="text">The source from which to count bytes.</param>
    /// <param name="encoding">The Encoding type of the bytes to count.</param>
    /// <returns>The total number of bytes counted.</returns>
    Task<int> CountBytesAsync(string text, Encoding encoding);

}