/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

namespace WCountCli.Helpers;

[Flags]
public enum CountSelection
{
    None = 0,
    Lines = 1,
    Words = 2,
    Characters = 4,
    Bytes = 8,

    Default = Lines | Words | Characters
}
