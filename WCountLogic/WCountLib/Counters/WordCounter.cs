/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Detectors;
using Microsoft.Extensions.Primitives;


namespace AlastairLundy.WCountLib.Counters
{
    public class WordCounter : IWordCounter
    {
        private readonly IWordDetector _wordDetector;

        public WordCounter(IWordDetector wordDetector)
        {
            _wordDetector = wordDetector;
        }

        protected ulong CountWordsWorker(string input)
        {
#if NET5_0_OR_GREATER
            ulong totalWords = 0;
#else
            long totalWords = 0;
#endif
            
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
            
#if NET5_0_OR_GREATER
            return totalWords;
#else
            return Convert.ToUInt64(totalWords);            
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        public ulong CountWords(TextReader textReader)
        { 
           string input = textReader.ReadToEnd();
           
            ulong totalWords = CountWordsWorker(input);
          
          textReader.Dispose();
          
          return totalWords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        public async Task<ulong> CountWordsAsync(TextReader textReader)
        {
            string input = await textReader.ReadToEndAsync();

            ulong totalWords = 0;
            
            Task wordCountingTask = Task.Run(() =>
            {
                totalWords = CountWordsWorker(input);
            });
            
            await wordCountingTask;
          
            textReader.Dispose();
          
            return totalWords;
        }
    }
}