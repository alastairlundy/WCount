/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using AlastairLundy.DotExtensions.MsExtensions.StringSegments.Collections;
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
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public int CountLines(IEnumerable<StringSegment> segments)
    {
        return _wcCommandExecutionHelper.RunInt32("-l", segments.ToString(' '));
    }
        
    /// <summary>
    /// Counts the total number of lines in a sequence of string segments.
    /// </summary>
    /// <param name="segments">A sequence of StringSegment objects.</param>
    /// <returns>The total number of lines as a signed 32-bit integer.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public async Task<int> CountLinesAsync(IEnumerable<StringSegment> segments)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-l", segments.ToString(' '));
    }
}