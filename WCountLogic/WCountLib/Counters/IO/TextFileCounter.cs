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
using WCountLib.Counters.Abstractions;

using WCountLib.Counters.Abstractions.IO.Specializations;
using WCountLib.Localizations;
// ReSharper disable RedundantBoolCompare

namespace WCountLib.Counters.IO
{
    /// <summary>
    /// A class to enable line counting, byte counting, char counting, and word counting for Text Files.
    /// </summary>
    public class TextFileCounter : ITextFileCounter
    {
        private readonly IWordCounter _wordCounter;
        private readonly ICharCounter _charCounter;
        private readonly IByteCounter _byteCounter;
        private readonly ILineCounter _lineCounter;

        public TextFileCounter(IByteCounter byteCounter, ICharCounter charCounter,
            IWordCounter wordCounter, ILineCounter lineCounter)
        {
            _byteCounter = byteCounter;
            _charCounter = charCounter;
            _wordCounter = wordCounter;
            _lineCounter = lineCounter;
        }
        
        /// <summary>
        /// Returns whether a file is a text file with an extension of .txt
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsATextFile(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".txt");
        }
        
        /// <summary>
        /// Gets the number of lines in a file.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <returns>the number of lines in a file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file could not be located.</exception>
        public int CountLinesInFile(string filePath)
        {
            if (File.Exists(filePath) && IsATextFile(filePath) == true)
            {
                string[] contents = File.ReadAllLines(filePath);
                return _lineCounter.CountLines(contents);
            }
            else if (IsATextFile(filePath) == false)
            {
                throw new ArgumentException(Resources.Exceptions_TextFileArgument_NotATextFile.Replace("{x}", filePath));
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
            if (File.Exists(filePath) && IsATextFile(filePath) == true)
            {
                string[] contents = await File.ReadAllLinesAsync(filePath);

                return _lineCounter.CountLines(contents);
            }
            else if (IsATextFile(filePath) == false)
            {
                throw new ArgumentException(Resources.Exceptions_TextFileArgument_NotATextFile.Replace("{x}", filePath));
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<ulong> CountWordsInFileAsync(string filePath)
        {
            if (File.Exists(filePath) && IsATextFile(filePath) == true)
            {
                string[] text = await File.ReadAllLinesAsync(filePath);

                return await _wordCounter.CountWordsAsync(text);
            }
            else if (IsATextFile(filePath) == false)
            {
                throw new ArgumentException(Resources.Exceptions_TextFileArgument_NotATextFile.Replace("{x}", filePath));
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
                if (IsATextFile(filePath) == false)
                {
                    throw new ArgumentException(Resources.Exceptions_TextFileArgument_NotATextFile.Replace("{x}", filePath));
                }
                
                ulong wordCount = 0;

                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    wordCount += _wordCounter.CountWords(line);
                }

                return wordCount;
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }

        /// <summary>
        /// Gets the number of characters in a file.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <param name="encoding"></param>
        /// <returns>the number of characters in the file specified.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file specified could not be found.</exception>
        public ulong CountCharactersInFile(string filePath, Encoding encoding)
        {
            if (File.Exists(filePath) && IsATextFile(filePath) == true)
            {
                string[] lines = File.ReadAllLines(filePath);

                return _charCounter.CountCharacters(lines, encoding);
            }
            else if (IsATextFile(filePath) == false)
            {
                throw new ArgumentException(Resources.Exceptions_TextFileArgument_NotATextFile.Replace("{x}", filePath));
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
        /// <param name="encoding"></param>
        /// <returns>the number of characters in the file specified.</returns>
        public async Task<ulong> CountCharactersInFileAsync(string filePath, Encoding encoding)
        {
            if (File.Exists(filePath) && IsATextFile(filePath) == true)
            {
                string[] lines = await File.ReadAllLinesAsync(filePath);

                return await _charCounter.CountCharactersAsync(lines, encoding);
            }
            else if (IsATextFile(filePath) == false)
            {
                throw new ArgumentException(Resources.Exceptions_TextFileArgument_NotATextFile.Replace("{x}", filePath));
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }
        
        /// <summary>
        /// Gets the number of bytes in a file.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <param name="textEncodingType">The type of encoding to use to decode the bytes.</param>
        /// <returns>the number of bytes in a file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file could not be located.</exception>
        public ulong CountBytesInFile(string filePath, Encoding textEncodingType)
        {
            if (File.Exists(filePath) && IsATextFile(filePath) == true)
            {
                return _byteCounter.CountBytes(File.ReadAllLines(filePath), textEncodingType);
            }
            else if (IsATextFile(filePath) == false)
            {
                throw new ArgumentException(Resources.Exceptions_TextFileArgument_NotATextFile.Replace("{x}", filePath));
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }

        /// <summary>
        /// Gets the number of bytes in a file asynchronously.
        /// </summary>
        /// <param name="filePath">The file path of the file to be searched.</param>
        /// <param name="textEncodingType">The type of encoding to use to decode the bytes.</param>
        /// <returns>the number of bytes in a file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file could not be located.</exception>
        public async Task<ulong> CountBytesInFileAsync(string filePath, Encoding textEncodingType)
        {
            if (File.Exists(filePath) && IsATextFile(filePath) == true)
            {
                string[] fileContents = await File.ReadAllLinesAsync(filePath);

                return await _byteCounter.CountBytesAsync(fileContents, textEncodingType);
            }
            else if (IsATextFile(filePath) == false)
            {
                throw new ArgumentException(Resources.Exceptions_TextFileArgument_NotATextFile.Replace("{x}", filePath));
            }
            else
            {
                throw new FileNotFoundException(Resources.Exceptions_FileNotFound_Message, filePath);
            }
        }
    }
}