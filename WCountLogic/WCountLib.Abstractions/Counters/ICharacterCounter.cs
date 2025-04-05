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

namespace AlastairLundy.WCountLib.Abstractions.Counters
{
    /// <summary>
    /// Represents a service that counts the number of characters in TextReaders.
    /// </summary>
    /// <remarks>
    /// <para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for character counting.</para>
    /// </remarks>
    public interface ICharacterCounter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        int CountCharactersInt32(string text, Encoding encoding);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        UInt64 CountCharactersUInt64(string text, Encoding encoding);
        
        /// <summary>
        /// Synchronously reads from the provided TextReader and counts total the number of characters in the specified Encoding.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count characters.</param>
        /// <param name="encoding">The Encoding type of the characters to count.</param>
        /// <returns>The total number of characters counted as a 32-bit integer.</returns>
        int CountCharactersInt32(TextReader textReader, Encoding encoding);
        
        /// <summary>
        /// Synchronously reads from the provided TextReader and counts total the number of characters in the specified Encoding.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count characters.</param>
        /// <param name="encoding">The Encoding type of the characters to count.</param>
        /// <returns>The total number of characters counted as a 64-bit unsigned integer.</returns>
        UInt64 CountCharactersUInt64(TextReader textReader, Encoding encoding);
        
        /// <summary>
        /// Asynchronously reads from the provided TextReader and counts the total number of characters in the specified Encoding.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count characters.</param>
        /// <param name="encoding">The Encoding type of the characters to count.</param>
        /// <returns>The total number of characters counted as a 32-bit integer.</returns>
        Task<int> CountCharactersInt32Async(TextReader textReader, Encoding encoding);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<int> CountCharactersInt32Async(string source, Encoding encoding);
        
        /// <summary>
        /// Asynchronously reads from the provided TextReader and counts the total number of characters in the specified Encoding.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count characters.</param>
        /// <param name="encoding">The Encoding type of the characters to count.</param>
        /// <returns>The total number of characters counted as a 64-bit unsigned integer.</returns>
        Task<UInt64> CountCharactersUInt64Async(TextReader textReader, Encoding encoding);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<UInt64> CountCharactersUInt64Async(string source, Encoding encoding);

    }
}