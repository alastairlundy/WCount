/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Threading.Tasks;

namespace AlastairLundy.WCountLib.Abstractions.Counters
{
    /// <summary>
    /// Represents a service that counts the number of lines in TextReaders.
    /// </summary>
    /// <remarks>
    /// <para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for line counting.</para>
    /// </remarks>
    public interface ILineCounter
    {
        int CountLinesInt32(string source);
        /// <summary>
        /// Synchronously reads from the provided TextReader and counts total the number of lines.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count lines.</param>
        /// <returns>The total number of lines counted.</returns>
        int CountLinesInt32(TextReader textReader);

        UInt64 CountLinesUInt64(string source);
        UInt64 CountLinesUInt64(TextReader textReader);
        /// <summary>
        /// Asynchronously reads from the provided TextReader and counts the total number of lines.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count lines.</param>
        /// <returns>The total number of lines counted.</returns>
        Task<int> CountLinesInt32Async(TextReader textReader);
        Task<int> CountLinesInt32Async(string source);
        Task<UInt64> CountLinesUInt64Async(TextReader textReader);
        Task<UInt64> CountLinesUInt64Async(string source);
    }
}