using System.Collections.Generic;
using System.Threading.Tasks;
using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Counters.Segments;

public class WcSegmentByteCounter : ISegmentByteCounter
{
    public int CountBytesInt32(IEnumerable<StringSegment> segments)
    {
       
    }

    public ulong CountBytesUInt64(IEnumerable<StringSegment> segments)
    {
        
    }

    public async Task<int> CountBytesInt32Async(IEnumerable<StringSegment> segments)
    {
       
    }

    public async Task<ulong> CountBytesUInt64Async(IEnumerable<StringSegment> segments)
    {
        
    }
}