using System.IO;
using System.Threading.Tasks;
using AlastairLundy.CliInvoke.Abstractions;
using AlastairLundy.WCountLib.Abstractions.Counters;
using WCountLib.Providers.wc.Helpers;

#if NETSTANDARD2_0 || NETSTANDARD2_1
using OperatingSystem = Polyfills.OperatingSystemPolyfill;
#endif

namespace WCountLib.Providers.wc.Counters;

public class WcLineCounter : ILineCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;
    
    public WcLineCounter(ICliCommandInvoker commandInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(commandInvoker);
    }
    
    public int CountLines(TextReader textReader)
    {
        return _wcCommandExecutionHelper.RunInt32("-l", textReader);
    }

    public async Task<int> CountLinesAsync(TextReader textReader)
    {
       return await _wcCommandExecutionHelper.RunInt32Async("-l", textReader);
    }
}