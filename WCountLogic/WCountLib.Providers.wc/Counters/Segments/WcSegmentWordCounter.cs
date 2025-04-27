/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Abstractions;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using AlastairLundy.WCountLib.Providers.wc.Helpers;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments
{
    /// <summary>
    /// Uses Unix's ``wc`` program for implementing Word counting in sequences of string segments.
    /// </summary>
    public class WcSegmentWordCounter : ISegmentWordCounter
    {
        private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

        /// <summary>
        /// Initializes a new instance of the WcSegmentWordCounter class.
        /// </summary>
        /// <param name="commandInvoker">The ICliCommandInvoker to be used to execute the ``wc``program.</param>
        public WcSegmentWordCounter(ICliCommandInvoker commandInvoker)
        {
            _wcCommandExecutionHelper = new WcCommandExecutionHelper(commandInvoker);
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public int CountWordsInt32(IEnumerable<StringSegment> segments)
        {
            return _wcCommandExecutionHelper.RunInt32("-w", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public ulong CountWordsUInt64(IEnumerable<StringSegment> segments)
        {
            return _wcCommandExecutionHelper.RunUInt64("-w", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public async Task<int> CountWordsInt32Async(IEnumerable<StringSegment> segments)
        {
            return await _wcCommandExecutionHelper.RunInt32Async("-w", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public async Task<ulong> CountWordsUInt64Async(IEnumerable<StringSegment> segments)
        {
            return await _wcCommandExecutionHelper.RunUInt64Async("-w",
                _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }
    }
}