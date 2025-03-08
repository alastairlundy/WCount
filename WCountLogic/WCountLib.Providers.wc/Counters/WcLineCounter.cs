using System.IO;
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
public class WcLineCounter : ILineCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="commandInvoker"></param>
    public WcLineCounter(ICliCommandInvoker commandInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(commandInvoker);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="textReader"></param>
    /// <returns></returns>
    public int CountLines(TextReader textReader)
    {
        return _wcCommandExecutionHelper.RunInt32("-l", textReader);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textReader"></param>
    /// <returns></returns>
    public async Task<int> CountLinesAsync(TextReader textReader)
    {
       return await _wcCommandExecutionHelper.RunInt32Async("-l", textReader);
    }
}