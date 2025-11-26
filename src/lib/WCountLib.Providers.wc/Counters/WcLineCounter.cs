/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace AlastairLundy.WCountLib.Providers.wc.Counters;

/// <summary>
/// A class that implements the <see cref="ILineCounter"/> interface to count the number of lines in a text using the wc command.
/// </summary>
public class WcLineCounter : ILineCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    /// <param name="processConfigurationFactory"></param>
    public WcLineCounter(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker, processConfigurationFactory);
    }

    /// <summary>
    /// Counts the number of lines in the provided text.
    /// </summary>
    /// <param name="text">The input text whose lines are to be counted.</param>
    /// <returns>The number of lines in the input text.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public int CountLines(string text)
    {
        return _wcCommandExecutionHelper.RunInt32("-l", text);
    }

    /// <summary>
    /// Asynchronously counts the number of lines in the provided text.
    /// </summary>
    /// <param name="text">The input text whose lines are to be counted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the number of lines in the input text.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public async Task<int> CountLinesAsync(string text)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-l", text);

    }
}