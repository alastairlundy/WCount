/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;

// ReSharper disable RedundantIfElseBlock

namespace AlastairLundy.WCountLib.Counters
{
    public class ByteCounter : IByteCounter
    {
        /// <summary>
        /// Gets the number of bytes in a string.
        /// </summary>
        /// <param name="s">The string to be searched.</param>
        /// <param name="textEncodingType">The type of encoding to use to decode the bytes.</param>
        /// <returns>the number of bytes in the string.</returns>
        protected int CountBytesWorker(string s, Encoding textEncodingType)
        {
            int byteCount;

            if (Equals(textEncodingType, Encoding.Unicode))
            {
                byteCount = Encoding.Unicode.GetByteCount(s);
            }
            else if (Equals(textEncodingType, Encoding.UTF32))
            {
                byteCount = Encoding.UTF32.GetByteCount(s);
            }
            else if (Equals(textEncodingType, Encoding.UTF8))
            {
                byteCount = Encoding.UTF8.GetByteCount(s);
            }
            else if (Equals(textEncodingType, Encoding.ASCII))
            {
                byteCount = Encoding.ASCII.GetByteCount(s);
            }
            else if (Equals(textEncodingType, Encoding.BigEndianUnicode))
            {
                byteCount = Encoding.BigEndianUnicode.GetByteCount(s);
            }
#if NET8_0_OR_GREATER
            else if (Equals(textEncodingType, Encoding.Latin1))
            {
                byteCount = Encoding.Latin1.GetByteCount(s);
            }
#endif
            else
            {
                byteCount = Encoding.Default.GetByteCount(s);
            }

            return byteCount;
        }

        public int CountBytes(TextReader textReader, Encoding textEncodingType)
        {
            int byteCount = 0;

            string? latestLine;

            do
            {
                latestLine = textReader.ReadLine();

                if (latestLine != null)
                {
                    byteCount += CountBytesWorker(latestLine, textEncodingType);
                }
            }
            while (latestLine != null);


            return byteCount;
        }

        public async Task<ulong> CountBytesAsync(TextReader textReader, Encoding textEncodingType)
        {
            ulong byteCount = 0;

            string? latestLine;

            do
            {
                latestLine = await textReader.ReadLineAsync();

                if (latestLine != null)
                {
                    byteCount += Convert.ToUInt64(CountBytesWorker(latestLine, textEncodingType));
                }
            }
            while (latestLine != null);


            return await new ValueTask<ulong>(byteCount);
        }
    }
}