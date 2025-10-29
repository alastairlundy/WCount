/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Factories;
using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Providers.wc.Helpers;

namespace AlastairLundy.WCountLib.Providers.wc.Counters;

/// <summary>
/// 
/// </summary>
public class WcWordCounter : IWordCounter
{
	private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;
		
	public WcWordCounter(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
	{
		_wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker, processConfigurationFactory);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public int CountWords(string text)
	{
		return _wcCommandExecutionHelper.RunInt32("-w", text);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public async Task<int> CountWordsAsync(string text)
	{
		return await _wcCommandExecutionHelper.RunInt32Async("-w", text);
	}
}