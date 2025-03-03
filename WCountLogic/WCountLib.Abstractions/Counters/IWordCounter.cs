/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCountLib.Abstractions.Counters
{
    public interface IWordCounter
    {
         Task<ulong> CountWordsAsync(string s);
         ulong CountWords(string s);
        
         Task<ulong> CountWordsAsync(IEnumerable<string> enumerable);
         ulong CountWords(IEnumerable<string> enumerable);
    }
}