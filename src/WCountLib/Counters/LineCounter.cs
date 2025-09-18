/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;

using AlastairLundy.DotExtensions.MsExtensions.StringSegments;

using AlastairLundy.WCountLib.Abstractions.Counters;

using Microsoft.Extensions.Primitives;


// ReSharper disable RedundantIfElseBlock

namespace AlastairLundy.WCountLib.Counters;

/// <summary>
/// 
/// </summary>
public class LineCounter : ILineCounter
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	public int CountLines(string source)
	{
		int output = 0;
		StringTokenizer tokenizer = new StringTokenizer(source, [' ']);

		StringSegment environmentNewLine = new StringSegment(Environment.NewLine);
		    
		foreach (StringSegment segment in tokenizer)
		{
			if (segment.Contains(environmentNewLine) || segment.Equals(environmentNewLine))
			{
				output++;
			}
		}
		    
		return output;
	}
}