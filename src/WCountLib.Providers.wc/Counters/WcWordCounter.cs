/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Runtime.Versioning;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Factories;
using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Providers.wc.Helpers;

namespace AlastairLundy.WCountLib.Providers.wc.Counters;

/// <summary>
/// A class that implements the <see cref="IWordCounter"/> interface and provides functionality
/// to count the number of words in a text string by using the `wc` command-line tool.
/// </summary>
public class WcWordCounter : IWordCounter
{
	private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

	/// <summary>
	/// Provides functionality to count the number of words in a text string
	/// by utilizing the underlying `wc` command through command-line invocation.
	/// </summary>
	public WcWordCounter(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
	{
		_wcCommandExecutionHelper = new WcCommandExecutionHelper(processInvoker, processConfigurationFactory);
	}

	/// <summary>
	/// Counts the number of words in the provided text string.
	/// </summary>
	/// <param name="text">The input text whose words are to be counted.</param>
	/// <returns>The word count as an integer.</returns>
	#if NET8_0_OR_GREATER
	[UnsupportedOSPlatform("windows")]
	[SupportedOSPlatform("macos")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("maccatalyst")]
	[SupportedOSPlatform("freebsd")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
	#endif
	public int CountWords(string text)
	{
		return _wcCommandExecutionHelper.RunInt32("-w", text);
	}

	/// <summary>
	/// Asynchronously counts the number of words in the provided text string.
	/// </summary>
	/// <param name="text">The input text whose words are to be counted.</param>
	/// <returns>A task representing the asynchronous operation, which contains the word count as an integer when completed.</returns>
#if NET8_0_OR_GREATER
	[UnsupportedOSPlatform("windows")]
	[SupportedOSPlatform("macos")]
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("maccatalyst")]
	[SupportedOSPlatform("freebsd")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
#endif
	public async Task<int> CountWordsAsync(string text)
	{
		return await _wcCommandExecutionHelper.RunInt32Async("-w", text);
	}
}