﻿/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WCountLib.Counters.Abstractions
{
    public interface ICharCounter
    {
        public int CountCharacters(string s, Encoding textEncodingType);
        
        public ulong CountCharacters(IEnumerable<string> enumerable, Encoding textEncodingType);
        public Task<ulong> CountCharactersAsync(IEnumerable<string> enumerable, Encoding textEncodingType);

    }
}