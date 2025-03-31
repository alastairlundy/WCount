/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */


using System.Collections.Generic;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Words
{
    public interface IStringSegmentWordCounter
    {
        int CountWordsInt32(IEnumerable<StringSegment> segments);
    
        ulong CountWordsUInt64(IEnumerable<StringSegment> segments);
    }
}