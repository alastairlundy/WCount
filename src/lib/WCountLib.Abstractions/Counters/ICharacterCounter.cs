/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Text;

namespace WCountLib.Abstractions.Counters;

/// <summary>
/// An interface for a service that counts the number of characters in strings.
/// </summary>
/// <remarks>
/// <para>Implementing classes should be stateless and avoid containing any properties or fields that aren't related to configurations or settings for character counting.</para>
/// </remarks>
public interface ICharacterCounter
{
    /// <summary>
    /// Synchronously reads from the provided string and counts total the number of characters in the specified Encoding.
    /// </summary>
    /// <param name="source">The string from which to count characters.</param>
    /// <param name="textEncodingType">The Encoding type of the characters to count.</param>
    /// <returns>The total number of characters counted.</returns>
    int CountCharacters(string source, Encoding textEncodingType);
}