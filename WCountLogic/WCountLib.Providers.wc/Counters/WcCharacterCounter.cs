using System.IO;
using System.Text;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Abstractions;
using AlastairLundy.WCountLib.Abstractions.Counters;
using WCountLib.Providers.wc.Helpers;

#if NETSTANDARD2_0 || NETSTANDARD2_1
using OperatingSystem = Polyfills.OperatingSystemPolyfill;
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
    /// 
    /// </summary>
    /// <param name="textReader"></param>
    /// <param name="textEncodingType"></param>
    /// <returns></returns>
    public int CountCharacters(TextReader textReader, Encoding textEncodingType)
    {
        return _wcCommandExecutionHelper.RunInt32("-m", textReader);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textReader"></param>
    /// <param name="textEncodingType"></param>
    /// <returns></returns>
    public async Task<ulong> CountCharactersAsync(TextReader textReader, Encoding textEncodingType)
    {
       return await _wcCommandExecutionHelper.RunUInt64Async("-m", textReader);
    }
}