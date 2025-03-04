/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.IO;
using System.Threading.Tasks;

namespace AlastairLundy.WCountLib.Abstractions.Counters
{
    public interface ILineCounter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        int CountLines(TextReader textReader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        Task<int> CountLinesAsync(TextReader textReader);
    }
}