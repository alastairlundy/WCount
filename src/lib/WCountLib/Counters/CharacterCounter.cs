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
public class CharacterCounter : ICharacterCounter
{
	/// <summary>
	/// Get the number of characters in a string.
	/// </summary>
	/// <param name="text">The string to be searched.</param>
	/// <param name="textEncodingType">The encoding type to use to count characters.</param>
	/// <returns>The number of characters in the string.</returns>
	public int CountCharacters(string text, Encoding textEncodingType)
	{
		ArgumentNullException.ThrowIfNull(text);
		ArgumentNullException.ThrowIfNull(textEncodingType);
		
		byte[] bytes = textEncodingType.GetBytes(text.ToCharArray());

		return textEncodingType.GetCharCount(bytes);
	}
}