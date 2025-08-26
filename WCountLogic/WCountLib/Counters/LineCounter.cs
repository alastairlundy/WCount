/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;

using Microsoft.Extensions.Primitives;


// ReSharper disable RedundantIfElseBlock

namespace AlastairLundy.WCountLib.Counters;

public class LineCounter : ILineCounter
{
	public int CountLines(string source)
	{
		int output = 0;
		StringTokenizer tokenizer = new StringTokenizer(source, [' ']);

		StringSegment environmentNewLine = new StringSegment(Environment.NewLine);
		    
		foreach (StringSegment segment in tokenizer)
		{
			if (segment.Equals(environmentNewLine))
			{
				output++;
			}
		}
		    
		return output;
	}

	/// <summary>
	/// Synchronously reads from the provided TextReader and counts total the number of lines.
	/// </summary>
	/// <param name="textReader">The TextReader from which to count lines.</param>
	/// <returns>The total number of lines counted.</returns>
	public int CountLines(TextReader textReader)
	{
		int lineCount = 0;
			
		string? latestLine;

		do
		{
			latestLine = textReader.ReadLine();

			if(latestLine != null)
			{
				Interlocked.Increment(ref lineCount);
			}
		}
		while (latestLine != null);


		return lineCount;
	}

	/// <summary>
	/// Asynchronously reads from the provided TextReader and counts the total number of lines.
	/// </summary>
	/// <param name="textReader">The TextReader from which to count lines.</param>
	/// <returns>The total number of lines counted.</returns>
	public async Task<int> CountLinesAsync(TextReader textReader)
	{
		int lineCount = 0;

		string? latestLine;

		do
		{
			latestLine = await textReader.ReadLineAsync();

			if (latestLine != null)
			{
				Interlocked.Increment(ref lineCount);
			}
		}
		while (latestLine != null);


		return await new ValueTask<int>(lineCount);
	}

	public async Task<int> CountLinesAsync(string text)
	{
		Task<int> task = new Task<int>(() => CountLines(text));
		task.Start();

		int result = await task.WaitAsync(CancellationToken.None);
		    
		return await new ValueTask<int>(result);
	}
}