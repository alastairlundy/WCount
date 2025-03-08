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

public class WcByteCounter : IByteCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    public WcByteCounter(ICliCommandInvoker cliCommandInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(cliCommandInvoker);
    }
    
    public int CountBytes(TextReader textReader, Encoding textEncodingType)
    {
        return _wcCommandExecutionHelper.RunInt32("-c", textReader);
    }

    public async Task<ulong> CountBytesAsync(TextReader textReader, Encoding textEncodingType)
    {
       return await _wcCommandExecutionHelper.RunUInt64Async("-c", textReader);
    }
}