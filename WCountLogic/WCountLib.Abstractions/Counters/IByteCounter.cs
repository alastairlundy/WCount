/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AlastairLundy.WCountLib.Abstractions.Counters
{
    public interface IByteCounter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="textEncodingType"></param>
        /// <returns></returns>
        int CountBytes(TextReader textReader, Encoding textEncodingType);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="textEncodingType"></param>
        /// <returns></returns>
        Task<ulong> CountBytesAsync(TextReader textReader, Encoding textEncodingType);
    }
}