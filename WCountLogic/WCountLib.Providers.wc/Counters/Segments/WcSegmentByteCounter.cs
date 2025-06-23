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
using AlastairLundy.DotExtensions.MsExtensions.System.Collections;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using AlastairLundy.WCountLib.Providers.wc.Helpers;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments
{
    /// <summary>
    /// Uses Unix's ``wc`` program for implementing Byte counting in sequences of string segments.
    /// </summary>
    public class WcSegmentByteCounter : ISegmentByteCounter
    {
        private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

        /// <summary>
        /// Initializes a new instance of the WcSegmentByteCounter class.
        /// </summary>
        /// <param name="commandInvoker">The ICliCommandInvoker to be used to execute the ``wc``program.</param>
        public WcSegmentByteCounter(ICliCommandInvoker commandInvoker)
        {
            _wcCommandExecutionHelper = new WcCommandExecutionHelper(commandInvoker);
        }
    
        /// <summary>
        /// Counts the total number of bytes in a sequence of string segments.
        /// </summary>
        /// <param name="segments">A sequence of StringSegment objects.</param>
        /// <returns>The total number of bytes as a signed 32-bit integer.</returns>
        public int CountBytes(IEnumerable<StringSegment> segments)
        {
            return _wcCommandExecutionHelper.RunInt32("-c", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));

        }

        /// <summary>
        /// Counts the total number of bytes in a sequence of string segments.
        /// </summary>
        /// <param name="segments">A sequence of StringSegment objects.</param>
        /// <returns>The total number of bytes as an unsigned 64-bit integer.</returns>
        public ulong CountBytesUInt64(IEnumerable<StringSegment> segments)
        {
            return _wcCommandExecutionHelper.RunUInt64("-c", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }

        /// <summary>
        /// Counts the total number of bytes in a sequence of string segments.
        /// </summary>
        /// <param name="segments">A sequence of StringSegment objects.</param>
        /// <returns>The total number of bytes.</returns>
        public async Task<int> CountBytesInt32Async(IEnumerable<StringSegment> segments)
        {
            return await _wcCommandExecutionHelper.RunInt32Async("-c", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }

        /// <summary>
        /// Counts the total number of bytes in a sequence of string segments.
        /// </summary>
        /// <param name="segments">A sequence of StringSegment objects.</param>
        /// <returns>The total number of bytes.</returns>
        public async Task<ulong> CountBytesUInt64Async(IEnumerable<StringSegment> segments)
        {
            return await _wcCommandExecutionHelper.RunUInt64Async("-c", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }
    }
}