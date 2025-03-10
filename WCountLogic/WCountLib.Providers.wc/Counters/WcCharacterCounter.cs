using System;
using System.IO;
using System.Text;
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
public class WcCharacterCounter : ICharacterCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cliCommandInvoker"></param>
    public WcCharacterCounter(ICliCommandInvoker cliCommandInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(cliCommandInvoker);
    }
    
    /// <summary>
    /// Synchronously reads from the provided TextReader and uses the Unix ``wc`` program to count the total number of characters in the specified Encoding.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count characters.</param>
    /// <param name="textEncodingType">The Encoding type of the characters to count.</param>
    /// <returns>The total number of characters counted.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on a platform that doesn't support the Unix ``wc`` command (such as Windows).</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("windows")]
#endif
    public int CountCharacters(TextReader textReader, Encoding textEncodingType)
    {
        return _wcCommandExecutionHelper.RunInt32("-m", textReader);
    }

    /// <summary>
    /// Asynchronously reads from the provided TextReader and uses the Unix ``wc`` program to count the total number of characters in the specified Encoding.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count characters.</param>
    /// <param name="textEncodingType">The Encoding type of the characters to count.</param>
    /// <returns>The total number of characters counted.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on a platform that doesn't support the Unix ``wc`` command (such as Windows).</exception>
    /// <exception cref="CliCommandNotSuccessfulException">Thrown if the Command is not successfully executed.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("windows")]
#endif
    public async Task<ulong> CountCharactersAsync(TextReader textReader, Encoding textEncodingType)
    {
       return await _wcCommandExecutionHelper.RunUInt64Async("-m", textReader);
    }
}