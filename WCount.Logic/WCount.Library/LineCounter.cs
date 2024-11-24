/*
    BasisBox - WCount Library
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
using System.Threading.Tasks;

using WCountLib.Localizations;

using WCountLib.Abstractions;

namespace WCountLib
{
    public class LineCounter : ILineCounter
    {
        /// <summary>
        /// Gets the number of lines in a file.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <returns>the number of lines in a file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file could not be located.</exception>
        public int CountLinesInFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] contents = File.ReadAllLines(filePath);
                return CountLines(contents);
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }

        /// <summary>
        /// Gets the number of lines in a file asynchronously.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <returns>the number of lines in a file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file could not be located.</exception>
        public async Task<int> CountLinesInFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] contents = await File.ReadAllLinesAsync(filePath);

                return CountLines(contents);
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }

        /// <summary>
        /// Gets the number of lines in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <returns>the number of lines in a string.</returns>
        public int CountLines(string s)
        {
            int totalCount = 0;

            foreach (char c in s)
            {
                if (c.Equals('\n') || c.Equals(char.Parse("\r\n")) || c.ToString().Equals(Environment.NewLine))
                {
                    totalCount++;
                }
            }

            return totalCount;
        }

        /// <summary>
        /// Gets the number of lines in an IEnumerable of strings.
        /// </summary>
        /// <param name="enumerable">The IEnumerable to be searched.</param>
        /// <returns>the number of lines in the specified IEnumerable.</returns>
        public int CountLines(IEnumerable<string> enumerable)
        {
            int totalCount = 0;

            foreach (string s in enumerable)
            {
                totalCount += CountLines(s);
            }

            return totalCount;
        }
    }
}