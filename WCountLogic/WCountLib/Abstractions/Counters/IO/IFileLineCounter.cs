﻿/*
    WCountLib
    Copyright (C) 2024 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Threading.Tasks;

namespace WCountLib.Abstractions.Counters.IO
{
    public interface IFileLineCounter
    {
        public int CountLinesInFile(string filePath);
        public Task<int> CountLinesInFileAsync(string filePath);
    }
}