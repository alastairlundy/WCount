/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Counters.Segments;

public class SegmentLineCounter : ISegmentLineCounter
{
    private int CountLineInt32Worker(StringSegment segment)
    {
        
    }

    private UInt64 CountLinesUInt64Worker(StringSegment segment)
    {
        
    }
    
    public int CountLinesInt32(IEnumerable<StringSegment> segments)
    {
        
    }

    public ulong CountLinesUInt64(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<int> CountLinesInt32Async(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<ulong> CountLinesUInt64Async(IEnumerable<StringSegment> segments)
    {
        
    }
}