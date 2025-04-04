/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Linq;

using AlastairLundy.DotExtensions.Strings;

using AlastairLundy.WCountLib.Abstractions.Detectors;
using Microsoft.Extensions.Primitives;

// ReSharper disable RedundantBoolCompare

namespace AlastairLundy.WCountLib.Detectors
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
        /// <param name="input">The string to be searched.</param>
        /// <param name="countStringsWithSpacesAsWords">Whether to count strings that contain 1 or more spaces within them as a word. Set to false by default.</param>
        /// <returns>true if the string is not a special character and doesn't contain a space character if spaces are excluded; false otherwise.</returns>
        public bool IsStringAWord(string input, bool countStringsWithSpacesAsWords = false)
        {
            bool output = input.ContainsSpaceSeparatedSubStrings() == false;

            if (string.IsNullOrWhiteSpace(input) == true || 
                input.ContainsSpaceSeparatedSubStrings() && countStringsWithSpacesAsWords == false)
            {
                output = false;
            }
            
            if(input.ToCharArray().All(c => c.IsSpecialCharacter() == true))
            {
                output = false;
            }

            if (input.Split(' ').Length == 1 && countStringsWithSpacesAsWords == false)
            {
                output = true;
            }

            return output;
        }

        /// <summary>
        /// Checks to see if a string looks like a word and doesn't contain the specified delimiters.
        /// Results may not be 100% accurate.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="delimitersToExclude">Deli</param>
        /// <param name="countStringsWithSpacesAsWords">Whether to count strings that contain 1 or more spaces within them as a word. Set to false by default.</param>
        /// <returns>true if the string does not contain any delimiters to exclude or is not a special character and doesn't contain a space character if space characters are excluded; false otherwise.</returns>
        public bool IsStringAWord(string s, IEnumerable<char> delimitersToExclude, bool countStringsWithSpacesAsWords = false)
        {
            bool output = false;
            
            if (string.IsNullOrWhiteSpace(s) == false ||
                s.ContainsSpaceSeparatedSubStrings() && countStringsWithSpacesAsWords == false)
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