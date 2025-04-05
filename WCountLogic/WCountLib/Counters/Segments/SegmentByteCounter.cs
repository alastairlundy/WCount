/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Counters.Segments;

public class SegmentByteCounter : ISegmentByteCounter
{
    public int CountBytesInt32(IEnumerable<StringSegment> segments)
    {
        
    }

    public ulong CountBytesUInt64(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<int> CountBytesInt32Async(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<ulong> CountBytesUInt64Async(IEnumerable<StringSegment> segments)
    {
        
    }
}