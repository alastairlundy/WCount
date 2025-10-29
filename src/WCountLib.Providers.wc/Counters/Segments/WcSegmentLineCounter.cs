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
using AlastairLundy.CliInvoke.Core.Factories;
using AlastairLundy.DotExtensions.MsExtensions.StringSegments.Collections;
using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using AlastairLundy.WCountLib.Providers.wc.Helpers;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments;

/// <summary>
/// Uses Unix's ``wc`` program for implementing Line counting in sequences of string segments.
/// </summary>
public class WcSegmentLineCounter : ISegmentLineCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    /// <summary>
    /// Initializes a new instance of the WcSegmentLineCounter class.
    /// </summary>
    /// <param name="processInvoker">The ICliCommandInvoker to be used to execute the ``wc``program.</param>
    /// <param name="processConfigurationFactory"></param>
    public WcSegmentLineCounter(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker, processConfigurationFactory);
    }
        
    /// <summary>
    /// Counts the total number of lines in a sequence of string segments.
    /// </summary>
    /// <param name="segments">A sequence of StringSegment objects.</param>
    /// <returns>The total number of lines as a signed 32-bit integer.</returns>
    public int CountLines(IEnumerable<StringSegment> segments)
    {
        return _wcCommandExecutionHelper.RunInt32("-l", segments.ToString(' '));
    }
        
    /// <summary>
    /// Counts the total number of lines in a sequence of string segments.
    /// </summary>
    /// <param name="segments">A sequence of StringSegment objects.</param>
    /// <returns>The total number of lines as a signed 32-bit integer.</returns>
    public async Task<int> CountLinesAsync(IEnumerable<StringSegment> segments)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-l", segments.ToString(' '));
    }
}