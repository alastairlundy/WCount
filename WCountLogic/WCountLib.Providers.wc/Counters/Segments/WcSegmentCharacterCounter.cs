/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments;

public class WcSegmentCharacterCounter : ISegmentCharacterCounter
{
    public int CountCharactersInt32(IEnumerable<StringSegment> segments)
    {
      
    }

    public ulong CountCharactersUInt64(IEnumerable<StringSegment> segments)
    {
       
    }

    public async Task<int> CountCharactersInt32Async(IEnumerable<StringSegment> segments)
    {
      
    }

    public async Task<ulong> CountCharactersUInt64Async(IEnumerable<StringSegment> segments)
    {
       
    }
}