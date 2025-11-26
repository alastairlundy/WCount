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
/// Uses Unix's ``wc`` program for implementing Word counting in sequences of string segments.
/// </summary>
public class WcSegmentWordCounter : ISegmentWordCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    /// <summary>
    /// Initializes a new instance of the WcSegmentWordCounter class.
    /// </summary>
    /// <param name="processInvoker">The ICliCommandInvoker to be used to execute the ``wc``program.</param>
    /// <param name="processConfigurationFactory"></param>
    public WcSegmentWordCounter(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker, processConfigurationFactory);
    }

    /// <summary>
    /// Counts the number of words in a sequence of string segments.
    /// </summary>
    /// <param name="segments">The collection of string segments to be processed for word count.</param>
    /// <returns>The total number of words in the provided sequence of string segments.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public int CountWords(IEnumerable<StringSegment> segments)
    {
        return _wcCommandExecutionHelper.RunInt32("-w", segments.ToString(' '));
    }

    /// <summary>
    /// Asynchronously counts the number of words in a sequence of string segments.
    /// </summary>
    /// <param name="segments">The collection of <see cref="StringSegment"/> to process.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total count of words.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public async Task<int> CountWordsAsync(IEnumerable<StringSegment> segments)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-w", segments.ToString(' '));
    }
}