/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Detectors;

using Microsoft.Extensions.Primitives;
// ReSharper disable UseCollectionExpression
// ReSharper disable RedundantArgumentDefaultValue


namespace AlastairLundy.WCountLib.Counters;

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
        int totalWords = 0;
            
        StringTokenizer stringTokenizer = new StringTokenizer(input, new[] { ' ' });
            
        IEnumerable<StringSegment> segments = stringTokenizer;
        int segmentCount = segments.Count();
            
        if (segmentCount < 100)
        {
            Parallel.ForEach(segments, segment =>
            {
                if (_wordDetector.IsStringAWord(segment.Value, false))
                {
                    Interlocked.Increment(ref totalWords);
                }
            });
        }
        else
        {
            OrderablePartitioner<StringSegment> partitioner = Partitioner.Create(segments, EnumerablePartitionerOptions.NoBuffering);
                
            Parallel.ForEach(partitioner, segment =>
            {
                if (_wordDetector.IsStringAWord(segment.Value, false))
                {
                    Interlocked.Increment(ref totalWords);
                }
            });
        }
            
        return totalWords;
    }
        


    /// <summary>
    /// Synchronously reads from the provided TextReader and counts total the number of words.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count words.</param>
    /// <returns>The total number of words counted.</returns>
    public int CountWords(TextReader textReader)
    { 
        string input = textReader.ReadToEnd();
           
        return CountWordsWorkerSegment(input);
            
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public int CountWords(string text)
    {
        return CountWordsWorkerSegment(text);
    }

    /// <summary>
    /// Asynchronously reads from the provided TextReader and counts the total number of words.
    /// </summary>
    /// <param name="textReader">The TextReader from which to count words.</param>
    /// <returns>The total number of words counted.</returns>
    public async Task<int> CountWordsAsync(TextReader textReader)
    {
        string input = await textReader.ReadToEndAsync();

        int totalWords = 0;
            
        Task wordCountingTask = Task.Run(() =>
        {
            totalWords = CountWordsWorkerSegment(input);
        });
            
        await wordCountingTask;
            
        return totalWords;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task<int> CountWordsAsync(string text)
    {
        int totalWords = 0;
            
        Task wordCountingTask = Task.Run(() =>
        {
            totalWords = CountWordsWorkerSegment(text);
        });
            
        await wordCountingTask;
            
        return totalWords;
    }
}