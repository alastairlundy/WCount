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
    
    public interface ISegmentWordCounter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        int CountWordsInt32(IEnumerable<StringSegment> segments);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        UInt64 CountWordsUInt64(IEnumerable<StringSegment> segments);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        Task<int> CountWordsInt32Async(IEnumerable<StringSegment> segments);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        Task<UInt64> CountWordsUInt64Async(IEnumerable<StringSegment> segments);
    }
}