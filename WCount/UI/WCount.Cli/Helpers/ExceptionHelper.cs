/*
	WCount CLI
	Copyright (c) Alastair Lundy 2024-2025
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;

using Spectre.Console;

using WCount.Cli.Localizations;

namespace WCount.Cli.Helpers;

internal static class ExceptionHelper
{
    internal static void PrintException(Exception exception, bool isVerbose)
    {
        if (isVerbose == true)
        {
            AnsiConsole.WriteException(exception, ExceptionFormats.Default);
        }
        else
        {
            AnsiConsole.WriteException(exception, ExceptionFormats.NoStackTrace);
        }
        
        AnsiConsole.WriteLine();
        
        AnsiConsole.WriteLine(Resources.Exceptions_BugReport_Request);
        AnsiConsole.WriteLine(Resources.Exceptions_BugReport_FileIssue);
    }
}