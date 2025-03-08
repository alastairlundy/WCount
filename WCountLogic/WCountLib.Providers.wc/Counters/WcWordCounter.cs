using System;
using System.IO;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Abstractions;
using AlastairLundy.CliInvoke.Exceptions;
using AlastairLundy.WCountLib.Abstractions.Counters;
using WCountLib.Providers.wc.Helpers;


#if NETSTANDARD2_0 || NETSTANDARD2_1
using OperatingSystem = Polyfills.OperatingSystemPolyfill;
#endif

namespace WCountLib.Providers.wc.Counters;

/// <summary>
/// 
/// </summary>
public class WcWordCounter : IWordCounter
{
	private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;
		
	public WcWordCounter(ICliCommandInvoker cliCommandInvoker)
	{
		_wcCommandExecutionHelper = new WcCommandExecutionHelper(cliCommandInvoker);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="textReader"></param>
	/// <returns></returns>
	/// <exception cref="PlatformNotSupportedException">Thrown if run on Windows</exception>
	public ulong CountWords(TextReader textReader)
	{
		return _wcCommandExecutionHelper.RunUInt64("-w", textReader);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="textReader"></param>
	/// <returns></returns>
	/// <exception cref="PlatformNotSupportedException"></exception>
	/// <exception cref="CliCommandNotSuccessfulException"></exception>
	public async Task<ulong> CountWordsAsync(TextReader textReader)
	{
		return await _wcCommandExecutionHelper.RunUInt64Async("-w", textReader);
	}
}