/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif

using AlastairLundy.CliInvoke.Core.Abstractions;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Providers.wc.Helpers;

namespace AlastairLundy.WCountLib.Providers.wc.Counters;

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
    public WcByteCounter(IProcessInvoker processInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker);
    }
    
    /// <summary>
    /// Synchronously reads from the provided TextReader and counts total the number of bytes in the specified Encoding.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count bytes.</param>
    /// <param name="textEncodingType">The Encoding type of the bytes to count.</param>
    /// <returns>The total number of bytes counted.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on a platform that doesn't support the Unix ``wc`` command (such as Windows).</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("windows")]
#endif
    public int CountBytes(TextReader textReader, Encoding textEncodingType)
    {
        return _wcCommandExecutionHelper.RunInt32("-c", textReader);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public int CountBytes(string text, Encoding encoding)
    {
        return _wcCommandExecutionHelper.RunInt32("-c", text);
    }

    /// <summary>
    /// Asynchronously reads from the provided TextReader and uses the Unix ``wc`` program to count the total number of bytes in the specified Encoding.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count bytes.</param>
    /// <param name="textEncodingType">The Encoding type of the bytes to count.</param>
    /// <returns>The total number of bytes counted.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on a platform that doesn't support the Unix ``wc`` command (such as Windows).</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("windows")]
#endif
    public async Task<int> CountBytesAsync(TextReader textReader, Encoding textEncodingType)
    {
       return await _wcCommandExecutionHelper.RunInt32Async("-c", textReader);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public async Task<int> CountBytesAsync(string text, Encoding encoding)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-c", text);
    }
}