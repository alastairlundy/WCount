/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */


using System.Collections.Generic;

namespace WCountLib.Privacy.Abstractions
{
    public interface IWordSubstitutionProvider
    {
        /// <summary>
        /// Substitutes every word in the string with another word of an identical length.
        /// </summary>
        /// <param name="words">The words to be substituted.</param>
        /// <returns>The updated string with substituted words.</returns>
        public string SubstituteWords(string words);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public IEnumerable<string> SubstituteWords(IEnumerable<string> words); 
    }
}