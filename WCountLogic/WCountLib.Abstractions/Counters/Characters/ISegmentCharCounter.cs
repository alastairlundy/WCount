using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Abstractions.Counters.Characters
{
    public interface ISegmentCharacterCounter
    {
        int CountCharactersInt32(IEnumerable<StringSegment> segments);
    
        ulong CountCharactersUInt64(IEnumerable<StringSegment> segments);
    }
}