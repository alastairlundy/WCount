/*
    WCountLib
    Copyright (C) 2024 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountLib.Abstractions.Models
{
    public class WCountResult
    {
        public ulong CharCount { get; protected set; }
        public ulong WordCount { get; protected set; }
        public ulong ByteCount { get; protected set; }
        public int LineCount { get; protected set; }
    }
}