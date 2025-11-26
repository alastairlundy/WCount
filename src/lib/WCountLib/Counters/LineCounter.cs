/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */




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
		ArgumentNullException.ThrowIfNull(source);
		
		int output = 0;
		IEnumerable<StringSegment> segments = new StringTokenizer(source, [' ']);

		StringSegment environmentNewLine = new StringSegment(Environment.NewLine);
		    
		foreach (StringSegment segment in segments)
		{
			if (segment.Contains(environmentNewLine) || segment.Equals(environmentNewLine))
			{
				output++;
			}
		}
		    
		return output;
	}
}