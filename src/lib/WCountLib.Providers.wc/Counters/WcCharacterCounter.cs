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
/// A character counting implementation that utilizes the Unix <c>wc</c> command for processing.
/// </summary>
public class WcCharacterCounter : ICharacterCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    /// <param name="processConfigurationFactory"></param>
    public WcCharacterCounter(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker, processConfigurationFactory);
    }

    /// <summary>
    /// Counts the number of characters in the given text.
    /// </summary>
    /// <param name="text">The input text for which the character count is to be computed.</param>
    /// <param name="textEncodingType">The encoding type of the input text.</param>
    /// <returns>The number of characters in the provided text.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public int CountCharacters(string text, Encoding textEncodingType)
    {
        return _wcCommandExecutionHelper.RunInt32("-m", text);
    }

    /// <summary>
    /// Asynchronously counts the number of characters in the given text.
    /// </summary>
    /// <param name="text">The input text for which the character count is to be computed.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the number of characters in the provided text.</returns>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    public async Task<int> CountCharactersAsync(string text)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-m", text);
    }
}