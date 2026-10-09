/*
    WCountLib
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace WCountLib.Models;

/// <summary>
/// The single value naming which counts a counting operation was asked to produce.
/// </summary>
/// <remarks>
/// One request is built per run. The Counting Engine computes only the counts whose
/// flag is <c>true</c>; the flagless CLI run maps to words, lines, and bytes.
/// The maximum-line-length count (<c>-L</c>) is opt-in and off by default.
/// </remarks>
public readonly record struct CountRequest(bool Words, bool Lines, bool Bytes, bool Characters, bool MaximumLineLength = false);
