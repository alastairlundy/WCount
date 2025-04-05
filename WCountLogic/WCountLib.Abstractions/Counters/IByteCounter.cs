﻿/*
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

namespace AlastairLundy.WCountLib.Abstractions.Counters
{
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
        int CountBytesInt32(TextReader textReader, Encoding encoding);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        UInt64 CountBytesUInt64(TextReader textReader, Encoding encoding);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        int CountBytesInt32(string source, Encoding encoding);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        UInt64 CountBytesUInt64(string source, Encoding encoding);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<int> CountBytesInt32Async(TextReader textReader, Encoding encoding);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<int> CountBytesInt32Async(string source, Encoding encoding);
        
        /// <summary>
        /// Asynchronously reads from the provided TextReader and counts the total number of bytes in the specified Encoding.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count bytes.</param>
        /// <param name="encoding">The Encoding type of the bytes to count.</param>
        /// <returns>The total number of bytes counted.</returns>
        Task<UInt64> CountBytesUInt64Async(TextReader textReader, Encoding encoding);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<UInt64> CountBytesUInt64Async(string source, Encoding encoding);

    }
}