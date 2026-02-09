/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using WCountLib.Abstractions.Counters;
using WCountLib.Providers.wc.Helpers;

namespace WCountLib.Providers.wc.Counters;

/// <summary>
/// 
/// </summary>
public class WcByteCounter : IByteCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    /// <param name="processConfigurationFactory"></param>
    public WcByteCounter(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker, processConfigurationFactory);
    }

    /// <summary>
    /// Counts the number of bytes in the provided text using the specified encoding.
    /// </summary>
    /// <param name="text">The input string whose byte count is to be calculated.</param>
    /// <param name="encoding">The encoding to be used to determine byte representation.</param>
    /// <returns>The number of bytes in the input text based on the specified encoding.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public int CountBytes(string text, Encoding encoding)
    {
        return _wcCommandExecutionHelper.RunInt32("-c", text);
    }

    /// <summary>
    /// Asynchronously counts the number of bytes in the provided text using the specified encoding.
    /// </summary>
    /// <param name="text">The input string whose byte count is to be calculated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of bytes in the input text based on the specified encoding.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public async Task<int> CountBytesAsync(string text)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-c", text);
    }
}