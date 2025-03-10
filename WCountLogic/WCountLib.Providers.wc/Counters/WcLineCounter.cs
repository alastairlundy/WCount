/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Abstractions;
using AlastairLundy.CliInvoke.Exceptions;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Providers.wc.Helpers;

namespace AlastairLundy.WCountLib.Providers.wc.Counters;

/// <summary>
/// 
/// </summary>
public class WcLineCounter : ILineCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="commandInvoker"></param>
    public WcLineCounter(ICliCommandInvoker commandInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(commandInvoker);
    }
    
    /// <summary>
    /// Synchronously reads from the provided TextReader and uses the Unix ``wc`` program to count the total number of lines.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count lines.</param>
    /// <returns>The total number of lines counted.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on a platform that doesn't support the Unix ``wc`` command (such as Windows).</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("windows")]
#endif
    public int CountLines(TextReader textReader)
    {
        return _wcCommandExecutionHelper.RunInt32("-l", textReader);
    }

    /// <summary>
    /// Asynchronously reads from the provided TextReader and uses the Unix ``wc`` program to count the total number of lines.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count lines.</param>
    /// <returns>The total number of lines counted.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on a platform that doesn't support the Unix ``wc`` command (such as Windows).</exception>
    /// <exception cref="CliCommandNotSuccessfulException">Thrown if the Command is not successfully executed.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("windows")]
#endif
    public async Task<int> CountLinesAsync(TextReader textReader)
    {
       return await _wcCommandExecutionHelper.RunInt32Async("-l", textReader);
    }
}