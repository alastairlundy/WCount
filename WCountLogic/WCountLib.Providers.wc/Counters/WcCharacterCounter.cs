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

public class WcCharacterCounter : ICharacterCounter
{
    private readonly WcCommandExecutionHelper _wcCommandExecutionHelper;

    public WcCharacterCounter(ICliCommandInvoker cliCommandInvoker)
    {
        _wcCommandExecutionHelper = new WcCommandExecutionHelper(cliCommandInvoker);
    }
    
    public int CountCharacters(TextReader textReader, Encoding textEncodingType)
    {
        return _wcCommandExecutionHelper.RunInt32("-m", textReader);
    }

    public async Task<ulong> CountCharactersAsync(TextReader textReader, Encoding textEncodingType)
    {
       return await _wcCommandExecutionHelper.RunUInt64Async("-m", textReader);
    }
}