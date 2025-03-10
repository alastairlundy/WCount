using System;
using System.IO;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Abstractions;
using AlastairLundy.CliInvoke.Exceptions;
using AlastairLundy.WCountLib.Abstractions.Counters;
using WCountLib.Providers.wc.Helpers;


#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
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
	/// Synchronously reads from the provided TextReader  and uses the Unix ``wc`` program to count the total number of words.
	/// </summary>
	/// <param name="textReader">The TextReader from which to count words.</param>
	/// <returns>The total number of words counted.</returns>
	/// <exception cref="PlatformNotSupportedException">Thrown if run on a platform that doesn't support the Unix ``wc`` command (such as Windows).</exception>
#if NET5_0_OR_GREATER
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	[SupportedOSPlatform("maccatalyst")]
	[SupportedOSPlatform("freebsd")]
	[UnsupportedOSPlatform("windows")]
#endif
	public ulong CountWords(TextReader textReader)
	{
		return _wcCommandExecutionHelper.RunUInt64("-w", textReader);
	}

	/// <summary>
	/// Asynchronously reads from the provided TextReader and uses the Unix ``wc`` program to count the total number of words.
	/// </summary>
	/// <param name="textReader">The TextReader from which to count words.</param>
	/// <returns>The total number of words counted.</returns>
	/// <exception cref="PlatformNotSupportedException">Thrown if run on a platform that doesn't support the Unix ``wc`` command (such as Windows).</exception>
	/// <exception cref="CliCommandNotSuccessfulException">Thrown if the Command is not successfully executed.</exception>
#if NET5_0_OR_GREATER
	[SupportedOSPlatform("linux")]
	[SupportedOSPlatform("macos")]
	[SupportedOSPlatform("maccatalyst")]
	[SupportedOSPlatform("freebsd")]
	[UnsupportedOSPlatform("windows")]
#endif
	public async Task<ulong> CountWordsAsync(TextReader textReader)
	{
		return await _wcCommandExecutionHelper.RunUInt64Async("-w", textReader);
	}
}