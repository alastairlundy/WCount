﻿/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Linq;

using AlastairLundy.Extensions.System.Strings;

using WCountLib.Detectors.Abstractions;

// ReSharper disable RedundantBoolCompare

namespace WCountLib.Detectors
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
        /// <param name="excludeStringsWithSpaces">Whether to exclude strings that contain 1 or more spaces within them. Set to true by default.</param>
        /// <returns>true if the string is not a special character and doesn't contain a space character if spaces are excluded; false otherwise.</returns>
        public bool IsStringAWord(string s, bool excludeStringsWithSpaces = true)
        {
            bool output = s.ContainsSpaceSeparatedSubStrings() == false;

            if (string.IsNullOrWhiteSpace(s) == true || 
                s.ContainsSpaceSeparatedSubStrings() && excludeStringsWithSpaces == true)
            {
                output = false;
            }
            
            if(s.ToCharArray().All(c => c.IsSpecialCharacter() == true))
            {
                output = false;
            }

            return output;
        }

        /// <summary>
        /// Checks to see if a string looks like a word and doesn't contain the specified delimiters.
        /// Results may not be 100% accurate.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="delimitersToExclude">Deli</param>
        /// <param name="excludeStringsWithSpaces">Whether to exclude strings that contain 1 or more spaces within them. Set to true by default.</param>
        /// <returns>true if the string does not contain any delimiters to exclude or is not a special character and doesn't contain a space character if space characters are excluded; false otherwise.</returns>
        public bool IsStringAWord(string s, IEnumerable<char> delimitersToExclude, bool excludeStringsWithSpaces = true)
        {
            bool output = false;
            
            if (string.IsNullOrWhiteSpace(s) == false ||
                s.ContainsSpaceSeparatedSubStrings() && excludeStringsWithSpaces == true)
            {
                output = false;    
            }

            if (s.ContainsAnyOf(delimitersToExclude) == true)
            {
                output = false;
            }

            return output;
        }
    }
}