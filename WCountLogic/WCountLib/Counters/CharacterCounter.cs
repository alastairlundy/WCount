/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;


// ReSharper disable RedundantIfElseBlock

namespace AlastairLundy.WCountLib.Counters
{
    public class CharacterCounter : ICharacterCounter
    {
        /// <summary>
        /// Get the number of characters in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="textEncodingType">The encoding type to use to count characters.</param>
        /// <returns>the number of characters in a string.</returns>
        protected int CountCharactersWorker(string s, Encoding textEncodingType)
        {
            int totalChars;

            byte[] bytes = textEncodingType.GetBytes(s.ToCharArray());

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

        /// <summary>
        /// Synchronously reads from the provided TextReader and counts total the number of characters in the specified Encoding.
        /// </summary>
        /// <param name="textReader">The TextReader from which to count characters.</param>
        /// <param name="textEncodingType">The Encoding type of the characters to count.</param>
        /// <returns>The total number of characters counted.</returns>
		public int CountCharacters(TextReader textReader, Encoding textEncodingType)
		{
            int charCount = 0;

			string? latestLine;

			do
			{
				latestLine = textReader.ReadLine();

				if (latestLine != null)
				{
					charCount += CountCharactersWorker(latestLine, textEncodingType);
				}
			}
			while (latestLine != null);


			return charCount;
		}

		public int CountCharacters(string text, Encoding textEncodingType)
		{
			
		}

		/// <summary>
		/// Asynchronously reads from the provided TextReader and counts the total number of characters in the specified Encoding.
		/// </summary>
		/// <param name="textReader">The TextReader from which to count characters.</param>
		/// <param name="textEncodingType">The Encoding type of the characters to count.</param>
		/// <returns>The total number of characters counted.</returns>
		public async Task<int> CountCharactersAsync(TextReader textReader, Encoding textEncodingType)
		{
			int charCount = 0;

			string? latestLine;

			do
			{
				latestLine = await textReader.ReadLineAsync();

				if (latestLine != null)
				{
                    charCount += CountCharactersWorker(latestLine, textEncodingType);
				}
			}
			while (latestLine != null);


			return await new ValueTask<int>(charCount);
		}

		public Task<int> CountCharactersAsync(string text, Encoding textEncodingType)
		{
			
		}
    }
}