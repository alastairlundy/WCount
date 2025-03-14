﻿/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;


// ReSharper disable RedundantIfElseBlock

namespace AlastairLundy.WCountLib.Counters
{
    public class LineCounter : ILineCounter
    {

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
	}
}