/*
    WCountLib.Abstraction
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace WCountLib.Abstractions.Models;

/// <summary>
/// Holds the optional results of a counting operation.
/// </summary>
public class WCountInfo
{
    /// <summary>
    /// The total number of words counted, or <c>null</c> if word counting was not requested.
    /// </summary>
    public long? WordCount { get; set; }

    /// <summary>
    /// The total number of characters counted, or <c>null</c> if character counting was not requested.
    /// </summary>
    public long? CharCount { get; set; }

    /// <summary>
    /// The total number of lines counted, or <c>null</c> if line counting was not requested.
    /// </summary>
    public long? LineCount { get; set; }

    /// <summary>
    /// The total number of bytes counted, or <c>null</c> if byte counting was not requested.
    /// </summary>
    public long? ByteCount { get; set; }
}
