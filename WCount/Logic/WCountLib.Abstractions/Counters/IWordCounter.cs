/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace AlastairLundy.WCountLib.Abstractions.Counters;

/// <summary>
/// Represents a service that counts the number of words in strings.
/// </summary>
/// <remarks><para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for word counting.</para>
/// <para>Implementers are responsible for determining how to handle punctuation, special characters, how to detect what a word is, and other text processing details.</para>
/// </remarks>
public interface IWordCounter
{
	
	/// <summary>
	/// Synchronously reads from the provided string and counts total the number of words.
	/// </summary>
	/// <param name="source">The string from which to count words.</param>
	/// <returns>The total number of words counted.</returns>
	int CountWords(string source);
}