/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Concurrent;
using WCountLib.Abstractions.Detectors;

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
}