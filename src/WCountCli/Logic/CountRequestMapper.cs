/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountCli.Logic;

/// <summary>
/// Translates the parsed count flags into the single <see cref="CountRequest"/>
/// the Counting Engine receives. Only plain BCL booleans cross this boundary:
/// no CLI-framework types leak past the Composition Root.
/// </summary>
public static class CountRequestMapper
{
    /// <summary>
    /// Maps one run's flag set to one request. A flagless run — no count flag
    /// at all — asks for words, lines, and bytes, the GNU default set. Any
    /// count flag present makes the run selective: only the named counts are
    /// computed, and <c>-L</c> stays opt-in.
    /// </summary>
    public static CountRequest ToRequest(
        bool words, bool lines, bool characters, bool bytes, bool maximumLineLength) =>
        words || lines || characters || bytes || maximumLineLength
            ? new CountRequest(Words: words, Lines: lines, Bytes: bytes, Characters: characters,
                MaximumLineLength: maximumLineLength)
            : new CountRequest(Words: true, Lines: true, Bytes: true, Characters: false);
}
