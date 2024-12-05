using System.Collections.Generic;

using AlastairLundy.Extensions.System.Strings.Contains;

using WCountLib.Abstractions;
// ReSharper disable RedundantBoolCompare

namespace WCountLib.ML;

public class MLWordDetector : IWordDetector
{
    
    public bool IsStringAWord(string s, bool excludeStringsWithSpaces = true)
    {
        return
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="delimitersToExclude"></param>
    /// <param name="excludeStringsWithSpaces"></param>
    /// <returns></returns>
    public bool IsStringAWord(string s, IEnumerable<char> delimitersToExclude, bool excludeStringsWithSpaces = true)
    {
        if (string.IsNullOrWhiteSpace(s) == true)
        {
            return false;
        }

        if (s.ContainsAnyOf(delimitersToExclude) == true)
        {
            return false;
        }
        
        return IsStringAWord(s, excludeStringsWithSpaces);
    }
}