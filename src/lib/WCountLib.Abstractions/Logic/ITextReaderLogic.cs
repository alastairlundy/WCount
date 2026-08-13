/*
    WCountLib.Abstraction
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WCountLib.Abstractions.Models;

namespace WCountLib.Abstractions.Logic;

/// <summary>
/// Provides chunked counting over <see cref="TextReader"/> and file streams.
/// </summary>
public interface ITextReaderLogic
{
    /// <summary>
    /// Reads from the provided <see cref="TextReader"/> and counts lines, words, characters, and/or bytes.
    /// </summary>
    /// <param name="reader">The text reader to read from.</param>
    /// <param name="showWordCount">Whether to count words.</param>
    /// <param name="showLineCount">Whether to count lines.</param>
    /// <param name="showCharacterCount">Whether to count characters.</param>
    /// <param name="showByteCount">Whether to count bytes.</param>
    /// <param name="encoding">The encoding to use for byte/character counting. When <c>null</c>, the encoding is resolved from the reader or defaults to UTF-8.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A <see cref="WCountInfo"/> containing the requested counts.</returns>
    Task<WCountInfo> ReadTextReaderAsync(TextReader reader, bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, Encoding? encoding = null,
        CancellationToken ct = default);

    /// <summary>
    /// Reads from the specified file and counts lines, words, characters, and/or bytes.
    /// </summary>
    /// <param name="file">The path to the file to read.</param>
    /// <param name="showWordCount">Whether to count words.</param>
    /// <param name="showLineCount">Whether to count lines.</param>
    /// <param name="showCharacterCount">Whether to count characters.</param>
    /// <param name="showByteCount">Whether to count bytes.</param>
    /// <param name="encoding">The encoding to use for byte/character counting. When <c>null</c>, the encoding is resolved from the reader or defaults to UTF-8.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A <see cref="WCountInfo"/> containing the requested counts.</returns>
    Task<WCountInfo> ReadFileAsync(string file, bool showWordCount, bool showLineCount,
        bool showCharacterCount, bool showByteCount, Encoding? encoding = null,
        CancellationToken ct = default);
}
