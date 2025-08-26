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
using AlastairLundy.CliInvoke.Core;

using System.Runtime.Versioning;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Providers.wc.Helpers;

namespace AlastairLundy.WCountLib.Providers.wc.Counters;

/// <summary>
/// 
/// </summary>
public class WcCharacterCounter : ICharacterCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processInvoker"></param>
    public WcCharacterCounter(IProcessInvoker processInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="textEncodingType"></param>
    /// <returns></returns>
    public int CountCharacters(string text, Encoding textEncodingType)
    {
        return _wcCommandExecutionHelper.RunInt32("-m", text);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="textEncodingType"></param>
    /// <returns></returns>
    public async Task<int> CountCharactersAsync(string text, Encoding textEncodingType)
    {
        return await _wcCommandExecutionHelper.RunInt32Async("-m", text);
    }
}