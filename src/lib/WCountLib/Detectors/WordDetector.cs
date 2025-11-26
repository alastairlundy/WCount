/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Linq;

using AlastairLundy.DotExtensions.Strings;

using AlastairLundy.WCountLib.Abstractions.Detectors;
// ReSharper disable ConvertClosureToMethodGroup
// ReSharper disable SimplifyConditionalTernaryExpression

namespace AlastairLundy.WCountLib.Detectors;

/// <summary>
/// A class to detect if strings that look like words are words.
/// </summary>
public class WordDetector : IWordDetector
{
    /// <summary>
    /// Checks to see if a string looks like a word.
    /// The results may not be 100% accurate.
    /// </summary>
    /// <param name="input">The string to be searched.</param>
    /// <param name="countStringsWithSpacesAsWords">Whether to count strings that contain 1 or more spaces within them as a word. Set to false by default.</param>
    /// <returns>true if the string is not a special character and doesn't contain a space character if spaces are excluded; false otherwise.</returns>
    public bool IsStringAWord(string input, bool countStringsWithSpacesAsWords = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);
        
        if (input.Length == 1)
            return !char.IsSpecialCharacter(input[0]);

        int separatorCount = 0;
        int specialCharCount = 0;

        bool charValidity = false;
        
        foreach (char c in input)
        {
            if(char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsAsciiLetter(c) ||
               char.IsSymbol(c))
                charValidity = true;

            if (char.IsSeparator(c))
                separatorCount++;
            
            if(char.IsPunctuation(c))
                specialCharCount++;
        }
        
        if (separatorCount == input.Length || specialCharCount == input.Length)
            return false;

        if (countStringsWithSpacesAsWords && input.ContainsSpaceSeparatedSubStrings() && charValidity)
            return true;

        return charValidity;
    }

    /// <summary>
    /// Determines whether a string contains one or more words.
    /// </summary>
    /// <param name="s">The string to be searched for a word.</param>
    /// <param name="wordSeparator">The separator char to look for between words.</param>
    /// <param name="countStringsWithSpacesAsWords">Whether to count strings with spaces in them as words.</param>
    /// <returns>True if one or more words were found, false otherwise.</returns>
    public bool DoesStringContainWords(string s, char wordSeparator, bool countStringsWithSpacesAsWords = false)
    {
        if (string.IsNullOrEmpty(s) || s.All(c => char.IsWhiteSpace(c)))
            return false;

        string[] possibleWords = s.Split(wordSeparator);

        return possibleWords.Any(x => IsStringAWord(x, countStringsWithSpacesAsWords));
    }
}