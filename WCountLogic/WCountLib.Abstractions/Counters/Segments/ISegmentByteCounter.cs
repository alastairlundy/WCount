/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Segments
{
    
    public interface ISegmentByteCounter
    {
        int CountBytesInt32(IEnumerable<StringSegment> segments);
    
        UInt64 CountBytesUInt64(IEnumerable<StringSegment> segments);
        
        Task<int> CountBytesInt32Async(IEnumerable<StringSegment> segments);
        Task<UInt64> CountBytesUInt64Async(IEnumerable<StringSegment> segments);
    }
}