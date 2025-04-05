﻿/*
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
	/// Represents a service that counts the number of words in TextReaders.
	/// </summary>
	/// <remarks><para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for word counting.</para>
	/// <para>Implementers are responsible for determining how to handle punctuation, special characters, how to detect what a word is, and other text processing details.</para>
	/// <para>This interface does not impose any rules on word boundaries, allowing for flexibility in implementation.</para>
	/// </remarks>
    public interface IWordCounter
    {

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="source"></param>
	    /// <returns></returns>
	    int CountWordsInt32(string source);

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="source"></param>
	    /// <returns></returns>
	    UInt64 CountWordsUInt64(string source);
	    
	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="textReader"></param>
	    /// <returns></returns>
	    int CountWordsInt32(TextReader textReader);
	    
        /// <summary>
        /// Synchronously reads from the provided TextReader and counts total the number of words.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count words.</param>
        /// <returns>The total number of words counted.</returns>
        UInt64 CountWordsUInt64(TextReader textReader);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
        Task<int> CountWordsInt32Async(string source);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
	    Task<int> CountWordsInt32Async(TextReader textReader);
        
		/// <summary>
		/// Asynchronously reads from the provided TextReader and counts the total number of words.
		/// </summary>
		/// <param name="textReader">The TextReader from which to count words.</param>
		/// <returns>The total number of words counted.</returns>
		Task<UInt64> CountWordsUInt64Async(TextReader textReader);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		Task<UInt64> CountWordsUInt64Async(string source);

	}
}