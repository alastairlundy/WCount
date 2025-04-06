using System.Collections.Generic;
using System.Threading.Tasks;
using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments;

public class WcSegmentCharacterCounter : ISegmentCharacterCounter
{
    public int CountCharactersInt32(IEnumerable<StringSegment> segments)
    {
      
    }

    public ulong CountCharactersUInt64(IEnumerable<StringSegment> segments)
    {
       
    }

    public async Task<int> CountCharactersInt32Async(IEnumerable<StringSegment> segments)
    {
      
    }

    public async Task<ulong> CountCharactersUInt64Async(IEnumerable<StringSegment> segments)
    {
       
    }
}