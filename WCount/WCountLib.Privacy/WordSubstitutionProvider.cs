/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Bogus.DataSets;

using WCountLib.Privacy.Abstractions;
// ReSharper disable RedundantBoolCompare

namespace WCountLib.Privacy
{
    public class WordSubstitutionProvider : IWordSubstitutionProvider
    {
        private readonly Dictionary<int, string> _replacementWords;

        public WordSubstitutionProvider()
        {
            _replacementWords = new Dictionary<int, string>();
            
            InitializeReplacementWords();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacementWords"></param>
        public WordSubstitutionProvider(IDictionary<int, string> replacementWords)
        {
            _replacementWords = new Dictionary<int, string>(replacementWords);
        }
        
        private void InitializeReplacementWords()
        {
            Lorem lorem = new Lorem();
            string[] replacementWordCandidates = lorem.Words(1000).Distinct().ToArray();

            (string[] words, int[] lengths) replacementWords = (replacementWordCandidates,
                replacementWordCandidates.Select(x => x.Length).ToArray());

            for (int i = 0; i < replacementWords.words.Length; i++)
            {
                (string text, int length) word = (replacementWords.words[i], replacementWords.lengths[i]);
                
#if NET8_0_OR_GREATER
                _replacementWords.TryAdd(word.length, word.text);
#else
                if( _replacementWords.ContainsKey(word.length) == false)
                {
                    _replacementWords.Add(word.length, word.text);
                }
#endif
            }
        }
        
        public string SubstituteWords(string words)
        {
            string[] splitWords = words.Split(' ');

            for (int index = 0; index < splitWords.Length; index++)
            {
                int splitWordLength = splitWords[index].Length;

                if (_replacementWords.TryGetValue(splitWordLength, out var word) == true)
                {
                    splitWords[index] = word;
                }
                else
                {
                    int nextIndex = splitWords.Length + 1;
                    while (true)
                    {
                        if (_replacementWords.TryGetValue(nextIndex, out var replacementWord) == true)
                        {
                            splitWords[index] = replacementWord.Substring(0, splitWordLength);
                            break;
                        }

                        if (nextIndex > 10_000_000)
                        {
                            break;
                        }
                        
                        nextIndex++;
                    }
                }
            }
            
            return String.Join(" ", splitWords);
        }

        public IEnumerable<string> SubstituteWords(IEnumerable<string> words)
        {
           List<string> output = new List<string>();

           string[] enumerable = words as string[] ?? words.ToArray();
           
           for (int index = 0; index < enumerable.Length; index++)
           {
               output.Add(SubstituteWords(enumerable[index]));
           }

           return output;
        }
    }
}