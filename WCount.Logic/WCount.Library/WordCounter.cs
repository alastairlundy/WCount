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

using WCount.Library.Interfaces;
using WCount.Library.Localizations;

namespace WCount.Library
{
    public class WordCounter : IWordCounter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<ulong> CountWordsAsync(string s)
        {
            ulong totalCount = 0;
        
            if (s.Contains(Environment.NewLine))
            {
                string[] splitStrings = s.Split(Environment.NewLine);
                int taskCount = splitStrings.Length;
            
                Task[] tasks = new Task[taskCount];
            
                for (int index = 0; index < taskCount; index++)
                {
                    tasks[index] = new Task(()=> totalCount += CountWords(splitStrings[index]));
                    tasks[index].Start();
                }

                await Task.WhenAll(tasks);
            }
            else
            {
                Task task = new Task(()=> totalCount = CountWords(s));
                task.Start();

                await task;
            }

            return totalCount;
        }

        /// <summary>
        /// Gets the number of words in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <returns>The number of words in the provided string.</returns>
        public ulong CountWords(string s)
        {
            ulong totalCount = 0;
        
            string[] words = s.Split(' ');

            if (words.Length > 0)
            {
                foreach (string word in words)
                {
                    if (string.IsNullOrWhiteSpace(word) == false)
                    {
                        totalCount += 1;
                    }
                }
            }
            else
            {
                if (s.Length > 0 && string.IsNullOrWhiteSpace(s) == false)
                {
                    totalCount = 1;
                }
                else
                {
                    totalCount = 0;
                }
            }

            return totalCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<ulong> CountWordsInFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                string text = await File.ReadAllTextAsync(filePath);

                return await CountWordsAsync(text);
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }

        /// <summary>
        /// Gets the number of words in a file.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <returns>The number of words in the file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file could not be found.</exception>
        public ulong CountWordsInFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                ulong wordCount = 0;
            
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    wordCount += CountWords(line);
                }

                return wordCount;
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }

        /// <summary>
        /// Gets the number of words in an IEnumerable of strings asynchronously.
        /// </summary>
        /// <param name="enumerable">The IEnumerable of strings to be searched.</param>
        /// <returns>the number of words in an IEnumerable of strings.</returns>
        public async Task<ulong> CountWordsAsync(IEnumerable<string> enumerable)
        {
            ulong totalCount = 0;
        
            foreach (string s in enumerable)
            {
                totalCount += await CountWordsAsync(s);
            }

            return totalCount;
        }

        /// <summary>
        /// Gets the number of words in an IEnumerable of strings.
        /// </summary>
        /// <param name="enumerable">The IEnumerable of strings to be searched.</param>
        /// <returns>the number of words in an IEnumerable of strings.</returns>
        public ulong CountWords(IEnumerable<string> enumerable)
        {
            Task<ulong> task = CountWordsAsync(enumerable);
            task.RunSynchronously();
        
            Task.WaitAll(task);
            return task.Result;
        }
    }
}