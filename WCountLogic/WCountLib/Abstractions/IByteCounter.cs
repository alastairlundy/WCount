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
    public interface IByteCounter
    {
        public int CountBytes(string s, Encoding encoding);

        public ulong CountBytesInFile(string filePath, Encoding encoding);
        public Task<ulong> CountBytesInFileAsync(string filePath, Encoding encoding);

        public ulong CountBytes(IEnumerable<string> strings, Encoding encoding);
        public Task<ulong> CountBytesAsync(IEnumerable<string> strings, Encoding encoding);
    }
}