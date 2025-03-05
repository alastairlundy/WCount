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

using Microsoft.Extensions.Primitives;


// ReSharper disable RedundantIfElseBlock

namespace AlastairLundy.WCountLib.Counters
{
    public class LineCounter : ILineCounter
    {

		/// <summary>
		/// Gets the number of lines in a string.
		/// </summary>
		/// <param name="s">The string to be searched.</param>
		/// <returns>the number of lines in a string.</returns>
		public int CountLines(string s)
        {
            int totalCount = 0;
            foreach (char c in s)
            {
                if (c.ToString().Equals(Environment.NewLine))
                {
                    totalCount++;
                }
            }

            return totalCount;
        }


		public int CountLines(TextReader textReader)
		{
			int newLines = 0;
			
			string? latestLine = string.Empty;

			do
			{
				latestLine = textReader.ReadLine();

				if(latestLine != null)
				{
					Interlocked.Increment(ref newLines);
				}
			}
			while (latestLine != null);


			return newLines;
		}

		public async Task<int> CountLinesAsync(TextReader textReader)
		{
			int newLines = 0;

			string? latestLine = string.Empty;

			do
			{
				latestLine = await textReader.ReadLineAsync();

				if (latestLine != null)
				{
					Interlocked.Increment(ref newLines);
				}
			}
			while (latestLine != null);


			return await new ValueTask<int>(newLines);
		}
	}
}