﻿/*
    WCountLib
    Copyright (C) 2024 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Linq;

using AlastairLundy.Extensions.System.Generics;
using AlastairLundy.Extensions.System.Strings.SpecialCharacters;

using WCountLib.Abstractions;
// ReSharper disable RedundantBoolCompare

namespace WCountLib
{
    /// <summary>
    /// A class to detect if strings that looks like words are words.
    /// </summary>
    public class WordDetector : IWordDetector
    {
        /// <summary>
        /// Checks to see if a string looks like a word.
        /// Results may not be 100% accurate.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <returns>true if the string is not a special character and doesn't contain a space; false otherwise.</returns>
        public bool IsStringAWord(string s)
        {
            return IsStringAWord(s, true);
        }

        /// <summary>
        /// Checks to see if a string looks like a word.
        /// Results may not be 100% accurate.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="excludeStringsWithSpaces">Whether to exclude strings that contain 1 or more spaces within them.</param>
        /// <returns>true if the string is not a special character and doesn't contain a space character if spaces are excluded; false otherwise.</returns>
        public bool IsStringAWord(string s, bool excludeStringsWithSpaces)
        {
            bool output = false;

            if (s.Split(' ').Length > 0 && excludeStringsWithSpaces == true)
            {
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(s) == false)
            {
                if (s.Length > 1 && s.ToCharArray().All(c => c.IsSpecialCharacter() == false) ||
                    (s.Length == 1 && s[0].IsSpecialCharacter() == false))
                {
                    output = true;
                }
            }

            return output;
        }

        /// <summary>
        /// Checks to see if a string looks like a word and doesn't contain the specified delimiters.
        /// Results may not be 100% accurate.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="delimitersToExclude">The delimiters to be used to exclude non-words.</param>
        /// <returns>true if the string does not contain any delimiters to exclude or is not a special character and doesn't contain a space character; false otherwise.</returns>
        public bool IsStringAWord(string s, IEnumerable<char> delimitersToExclude)
        {
            return IsStringAWord(s, delimitersToExclude, true);
        }

        /// <summary>
        /// Checks to see if a string looks like a word and doesn't contain the specified delimiters.
        /// Results may not be 100% accurate.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="delimitersToExclude"></param>
        /// <param name="excludeStringsWithSpaces"></param>
        /// <returns>true if the string does not contain any delimiters to exclude or is not a special character and doesn't contain a space character if space characters are excluded; false otherwise.</returns>
        public bool IsStringAWord(string s, IEnumerable<char> delimitersToExclude, bool excludeStringsWithSpaces)
        {
            bool output = false;
            
            if (string.IsNullOrWhiteSpace(s) == false)
            {
                output = false;    
            }
            
            if (s.Split(' ').Length > 0 && excludeStringsWithSpaces == true)
            {
                return false;
            }
            
            if (s.ToCharArray().All(c => c.ContainsAnyOf(delimitersToExclude) == false))
            {
                output = IsStringAWord(s, excludeStringsWithSpaces);
            }

            return output;
        }
    }
}