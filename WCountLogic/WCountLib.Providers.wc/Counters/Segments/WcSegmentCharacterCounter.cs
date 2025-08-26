/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Core;
using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using AlastairLundy.WCountLib.Providers.wc.Helpers;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments;
{
    /// <summary>
    /// Uses Unix's ``wc`` program for implementing Character counting in sequences of string segments.
    /// </summary>
    public class WcSegmentCharacterCounter : ISegmentCharacterCounter
    {
        private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

        /// <summary>
        /// Initializes a new instance of the WcSegmentCharacterCounter class.
        /// </summary>
        /// <param name="processInvoker">The ICliCommandInvoker to be used to execute the ``wc``program.</param>
        public WcSegmentCharacterCounter(IProcessInvoker processInvoker)
        {
            _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker);
        }
        
        /// <summary>
        /// Counts the total number of characters in a sequence of string segments.
        /// </summary>
        /// <param name="segments">A sequence of StringSegment objects.</param>
        /// <returns>The total number of characters as a signed 32-bit integer.</returns>
        public int CountCharacters(IEnumerable<StringSegment> segments)
        {
            return _wcCommandExecutionHelper.RunInt32("-m", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }
        
        /// <summary>
        /// Counts the total number of characters in a sequence of string segments.
        /// </summary>
        /// <param name="segments">A sequence of StringSegment objects.</param>
        /// <returns>The total number of characters as a signed 32-bit integer.</returns>
        public async Task<int> CountCharactersAsync(IEnumerable<StringSegment> segments)
        {
            return await _wcCommandExecutionHelper.RunInt32Async("-m", _wcCommandExecutionHelper.GetSegmentsToTextReader(segments));
        }
        
    }
}