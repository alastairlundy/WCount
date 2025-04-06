using System.Collections.Generic;
using System.Threading.Tasks;
using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments;

public class WcSegmentWordCounter : ISegmentWordCounter
{
    public int CountWordsInt32(IEnumerable<StringSegment> segments)
    {
       
    }

    public ulong CountWordsUInt64(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<int> CountWordsInt32Async(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<ulong> CountWordsUInt64Async(IEnumerable<StringSegment> segments)
    {
        
    }
}