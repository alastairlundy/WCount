/*
	WCount CLI
	Copyright (c) Alastair Lundy 2024-2025
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;

using Spectre.Console;
using Spectre.Console.Cli;

using WCount.Cli.Helpers;
using WCount.Cli.Localizations;
using WCount.Cli.Models;

namespace WCount.Cli.Commands
{
    public class LineCountOnlyCommand : AsyncCommand<LineCountOnlyCommand.Settings>
    {
        private readonly ILineCounter _lineCounter;

        public LineCountOnlyCommand(ILineCounter lineCounter)
        {
            _lineCounter = lineCounter;
        }

        public class Settings : SharedWCountSettings
        {

        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            ExceptionFormats exceptionFormats;

            if (settings.Verbose)
            {
                exceptionFormats = ExceptionFormats.Default;
            }
            else
            {
                exceptionFormats = ExceptionFormats.NoStackTrace;
            }

            int fileResult = FileArgumentHelpers.HandleFileArgument(settings.Files, exceptionFormats);

            if (fileResult == -1)
            {
                return -1;
            }


            try
            {
                int totalLines = 0;

                foreach (string file in settings.Files!)
                {
                    string fileContents = await File.ReadAllTextAsync(file);
                    
                    using StringReader reader = new StringReader(fileContents);
                    
                    int lineCount = await _lineCounter.CountLinesAsync(reader);
                    totalLines += lineCount;

                    string label;

                    if (lineCount == 1)
                    {
                        label = Resources.WCount_App_Labels_Lines_Singular;
                    }
                    else
                    {
                        label = Resources.WCount_App_Labels_Lines_Plural;
                    }

                    AnsiConsole.WriteLine($"{file} {lineCount} {label}");
                }

                if (settings.Files.Length > 1)
                {
                    if (totalLines == 0 || totalLines > 1)
                    {
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalLines} {Resources.WCount_App_Labels_Lines_Plural}");
                    }
                    else
                    {
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalLines} {Resources.WCount_App_Labels_Lines_Singular}");
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, exceptionFormats);
                return -1;
            }
        }
    }
}
