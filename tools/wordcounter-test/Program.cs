using System;
using System.IO;
using System.Linq;
using System.Text;
using EnhancedLinq.Deferred;
using WCountLib.Counters;
using WCountLib.Detectors;

class Program
{
    static void Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : "test-files\\NATURE.txt";
        string content = File.ReadAllText(path);
        Console.WriteLine($"File length (chars): {content.Length}");
        char[] arr = content.ToCharArray();

        var segments = arr.SplitBy(c => c == ' ');
        int sc = 0;
        foreach (var seg in segments)
        {
            sc++;
            var sarr = seg.ToArray();
            Console.WriteLine($"Segment {sc}: len={sarr.Length} repr='{{0}}'", new string(sarr));
        }

        var detector = new WordDetector();
        var counter = new WordCounter(detector);

        int wordsFromChars = counter.CountWords(arr);

        Console.WriteLine($"Words (char[] overload): {wordsFromChars}");
    }
}
