/*
    WCountLib
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using EnhancedLinq.Deferred;

namespace WCountLib.Counters;

/// <summary>
/// 
/// </summary>
public class WordCounter : IWordCounter
{
    private readonly IWordDetector _wordDetector;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wordDetector"></param>
    public WordCounter(IWordDetector wordDetector)
    {
        _wordDetector = wordDetector;
    }

    private int CountWordsWorkerSegment(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        int totalWords = 0;

        string[] strings = input.Split(' ');

        OrderablePartitioner<string> partitioner =
            Partitioner.Create(strings, EnumerablePartitionerOptions.NoBuffering);

        Parallel.ForEach(partitioner, str =>
        {
            if (_wordDetector.IsStringAWord(str))
            {
                Interlocked.Increment(ref totalWords);
            }
        });

        return totalWords;
    }
    
    private int CountWordsWorkerSegment(char[] input)
    {
        ArgumentNullException.ThrowIfNull(input);

        int totalWords = 0;

        IEnumerable<IEnumerable<char>> strings = input.SplitBy(c => c == ' ');

        OrderablePartitioner<IEnumerable<char>> partitioner =
            Partitioner.Create(strings, EnumerablePartitionerOptions.NoBuffering);

        Parallel.ForEach(partitioner, str =>
        {
            if (_wordDetector.IsStringAWord(str))
            {
                Interlocked.Increment(ref totalWords);
            }
        });

        return totalWords;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public int CountWords(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return CountWordsWorkerSegment(text);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public int CountWords(char[] source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return CountWordsWorkerSegment(source);
    }
}