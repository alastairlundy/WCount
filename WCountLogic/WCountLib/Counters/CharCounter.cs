/*
    WCountLib
    Copyright (C) 2024 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WCountLib.Abstractions.Counters;
using WCountLib.Localizations;

// ReSharper disable RedundantIfElseBlock

namespace WCountLib.Counters
{
    public class CharCounter : ICharCounter
    {
        /// <summary>
        /// Get the number of characters in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="textEncodingType">The encoding type to use to count characters.</param>
        /// <returns>the number of characters in a string.</returns>
        public int CountCharacters(string s, Encoding textEncodingType)
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
        /// Gets the number of characters in an IEnumerable of strings.
        /// </summary>
        /// <param name="enumerable">The IEnumerable to be searched.</param>
        /// <param name="textEncodingType">The encoding type to use to count characters.</param>
        /// <returns>the number of characters in the specified IEnumerable.</returns>
        public ulong CountCharacters(IEnumerable<string> enumerable, Encoding textEncodingType)
        {
            ulong totalChars = 0;

            foreach (string s in enumerable)
            {
                totalChars += Convert.ToUInt64(CountCharacters(s, textEncodingType));
            }

            return totalChars;
        }

        /// <summary>
        /// Gets the number of characters in an IEnumerable of strings asynchronously.
        /// </summary>
        /// <param name="enumerable">The IEnumerable to be searched.</param>
        /// <param name="textEncodingType">The encoding type to use to count characters.</param>
        /// <returns>the number of characters in the specified IEnumerable.</returns>
        public async Task<ulong> CountCharactersAsync(IEnumerable<string> enumerable, Encoding textEncodingType)
        {
            ulong totalChars = 0;
            string[] array = enumerable.ToArray();
            
            Task[] tasks = new Task[array.Length];

            for (int index = 0; index < array.Length; index++)
            {
                int taskNumber = index;
                tasks[taskNumber] = new Task<ulong>(() => totalChars += Convert.ToUInt64(CountCharacters(array[taskNumber], textEncodingType)));
                tasks[taskNumber].Start();
            }

            await Task.WhenAll(tasks);

            return totalChars;
        }
    }
}