/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCountLib.Counters.Abstractions
{
    public interface IWordCounter
    {
        public Task<ulong> CountWordsAsync(string s);
        public ulong CountWords(string s);
        
        public Task<ulong> CountWordsAsync(IEnumerable<string> enumerable);
        public ulong CountWords(IEnumerable<string> enumerable);
    }
}