/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;

using System.Runtime.Versioning;
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
    /// <param name="processInvoker"></param>
    public WcLineCounter(IProcessInvoker processInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public int CountLines(string text)
    {
        return _wcCommandExecutionHelper.RunInt32("-l", text);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task<int> CountLinesAsync(string text)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-l", text);

    }
}