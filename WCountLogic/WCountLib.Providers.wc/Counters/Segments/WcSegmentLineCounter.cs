using System.Collections.Generic;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters.Segments;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments;

public class WcSegmentLineCounter : ISegmentLineCounter
{
    public int CountLinesInt32(IEnumerable<StringSegment> segments)
    {
        
    }

    public ulong CountLinesUInt64(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<int> CountLinesInt32Async(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<ulong> CountLinesUInt64Async(IEnumerable<StringSegment> segments)
    {
        
    }
}