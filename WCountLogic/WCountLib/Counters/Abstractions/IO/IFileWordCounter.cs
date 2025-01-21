/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Threading.Tasks;

namespace WCountLib.Counters.Abstractions.IO
{
    public interface IFileWordCounter
    {
        public Task<ulong> CountWordsInFileAsync(string filePath);
        public ulong CountWordsInFile(string filePath);
    }
}