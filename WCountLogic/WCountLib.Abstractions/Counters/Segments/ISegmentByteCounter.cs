/*
    WCountLib.Abstraction
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Segments
{
    
    public interface ISegmentByteCounter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        int CountBytesInt32(IEnumerable<StringSegment> segments, Encoding encoding);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        UInt64 CountBytesUInt64(IEnumerable<StringSegment> segments, Encoding encoding);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<int> CountBytesInt32Async(IEnumerable<StringSegment> segments, Encoding encoding);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<UInt64> CountBytesUInt64Async(IEnumerable<StringSegment> segments, Encoding encoding);
    }
}