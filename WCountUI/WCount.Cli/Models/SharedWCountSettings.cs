/*
	WCount CLI
	Copyright (c) Alastair Lundy 2024-2025
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.ComponentModel;
using Spectre.Console.Cli;

namespace WCount.Cli.Models
{
    public class SharedWCountSettings : CommandSettings
    {
        [CommandOption("--locale <locale_code>")]
        [DefaultValue("")]
        public string? Locale { get; init; }

        [CommandArgument(1, "<files>")]
        public string[]? Files { get; init; }

        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }
    }
}
