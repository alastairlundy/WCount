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

using System.Runtime.Versioning;

using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Factories;
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
    public WcByteCounter(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker, processConfigurationFactory);
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