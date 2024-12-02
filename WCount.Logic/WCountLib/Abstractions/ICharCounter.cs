/*
    WCountLib
    Copyright (C) 2024 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WCountLib.Abstractions
{
    public interface ICharCounter
    {
        public ulong CountCharacters(string s);
        public int CountCharacters(string s, Encoding textEncodingType);
        public ulong CountCharactersInFile(string filePath);
        public Task<ulong> CountCharactersInFileAsync(string filePath);
        public ulong CountCharacters(IEnumerable<string> enumerable);
        public Task<ulong> CountCharactersAsync(IEnumerable<string> enumerable);

    }
}