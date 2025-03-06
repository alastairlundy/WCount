using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Words;

public interface IStringSegmentWordCounter
{
    int CountWordsInt32(IEnumerable<StringSegment> segments);
    
    ulong CountWordsUInt64(IEnumerable<StringSegment> segments);
}