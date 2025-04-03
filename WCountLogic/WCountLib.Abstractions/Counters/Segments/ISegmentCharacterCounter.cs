/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Segments
{
    public interface ISegmentCharacterCounter
    {
        int CountCharactersInt32(IEnumerable<StringSegment> segments);
    
        UInt64 CountCharactersUInt64(IEnumerable<StringSegment> segments);
    }
}