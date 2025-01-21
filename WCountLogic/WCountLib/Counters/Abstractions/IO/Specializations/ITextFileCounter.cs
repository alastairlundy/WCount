/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountLib.Counters.Abstractions.IO.Specializations
{
    public interface ITextFileCounter : IFileLineCounter, IFileByteCounter, IFileCharCounter, IFileWordCounter
    {
        public bool IsATextFile(string filePath);
    }
}