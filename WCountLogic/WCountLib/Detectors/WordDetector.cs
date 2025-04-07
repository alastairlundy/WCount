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
            bool output = false;

            switch (input.Length)
            {
                case 0:
                    return false;
                case 1:
                    return input[0].IsSpecialCharacter();
                case > 1:
                    if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input) 
                        || input.ToCharArray().All(X => X.IsSpecialCharacter())
                        || input.Contains(' ') && countStringsWithSpacesAsWords == false)
                    {
                        return false;
                    }
                    else
                    {
                        output = true;
                    }
                    
                    break;
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
            if (string.IsNullOrWhiteSpace(s) == false ||
                s.ContainsSpaceSeparatedSubStrings() && countStringsWithSpacesAsWords == false)
            {
                return false;    
            }

            if (s.ContainsAnyOf(delimitersToExclude) == true)
            {
                return false;
            }

            return IsStringAWord(s, countStringsWithSpacesAsWords);
        }
    }
}