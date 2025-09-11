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
		int totalChars = 0;

		byte[] bytes = textEncodingType.GetBytes(text.ToCharArray());

		if (Equals(textEncodingType, Encoding.Unicode))
		{
			totalChars = Encoding.Unicode.GetCharCount(bytes);
		}
		else if (Equals(textEncodingType, Encoding.UTF32))
		{
			totalChars = Encoding.UTF32.GetCharCount(bytes);
		}
		else if (Equals(textEncodingType, Encoding.UTF8))
		{
			totalChars = Encoding.UTF8.GetCharCount(bytes);
		}
		else if (Equals(textEncodingType, Encoding.ASCII))
		{
			totalChars = Encoding.ASCII.GetCharCount(bytes);
		}
		else if (Equals(textEncodingType, Encoding.BigEndianUnicode))
		{
			totalChars = Encoding.BigEndianUnicode.GetCharCount(bytes);
		}
#if NET8_0_OR_GREATER
		else if (Equals(textEncodingType, Encoding.Latin1))
		{
			totalChars = Encoding.Latin1.GetCharCount(bytes);
		}
#endif
		else
		{
			totalChars = Encoding.Default.GetCharCount(bytes);
		}

		return totalChars;
	}
}