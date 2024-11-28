/*
    WCountLib
    Copyright (C) 2024 Alastair Lundy

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, version 3 of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WCountLib.Localizations;

using WCountLib.Abstractions;
// ReSharper disable RedundantIfElseBlock

namespace WCountLib
{
    public class CharCounter : ICharCounter
    {
        /// <summary>
        /// Get the number of characters in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <returns>the number of characters in a string.</returns>
        public ulong CountCharacters(string s)
        {
            return Convert.ToUInt64(CountCharacters(s, Encoding.Default));
        }

        /// <summary>
        /// Get the number of characters in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="textEncodingType"></param>
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
        /// Gets the number of characters in a file.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <returns>the number of characters in the file specified.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file specified could not be found.</exception>
        public ulong CountCharactersInFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                return CountCharacters(lines);
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }

        /// <summary>
        /// Gets the number of characters in a file asynchronously.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <returns>the number of characters in the file specified.</returns>
        public async Task<ulong> CountCharactersInFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] lines = await File.ReadAllLinesAsync(filePath);

                return await CountCharactersAsync(lines);
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }

        /// <summary>
        /// Gets the number of characters in an IEnumerable of strings.
        /// </summary>
        /// <param name="enumerable">The IEnumerable to be searched.</param>
        /// <returns>the number of characters in the specified IEnumerable.</returns>
        public ulong CountCharacters(IEnumerable<string> enumerable)
        {
            ulong totalChars = 0;

            foreach (string s in enumerable)
            {
                totalChars += CountCharacters(s);
            }

            return totalChars;
        }

        /// <summary>
        /// Gets the number of characters in an IEnumerable of strings asynchronously.
        /// </summary>
        /// <param name="enumerable">The IEnumerable to be searched.</param>
        /// <returns>the number of characters in the specified IEnumerable.</returns>
        public async Task<ulong> CountCharactersAsync(IEnumerable<string> enumerable)
        {
            ulong totalChars = 0;
            string[] array = enumerable.ToArray();
            
            Task[] tasks = new Task[array.Length];

            for (int index = 0; index < array.Length; index++)
            {
                int taskNumber = index;
                tasks[taskNumber] = new Task<ulong>(() => totalChars += CountCharacters(array[taskNumber]));
                tasks[taskNumber].Start();
            }

            await Task.WhenAll(tasks);

            return totalChars;
        }
    }
}