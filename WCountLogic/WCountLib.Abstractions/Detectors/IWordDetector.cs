/*
    WCountLib
    Copyright (C) 2024-2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace AlastairLundy.WCountLib.Abstractions.Detectors
{
    public interface IWordDetector
    {
        bool IsStringAWord(string s, bool excludeStringsWithSpaces = true);
        bool IsStringAWord(string s, IEnumerable<char> delimitersToExclude, bool excludeStringsWithSpaces = true);

    }
}