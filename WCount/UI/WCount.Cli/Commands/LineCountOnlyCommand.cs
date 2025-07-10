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
            int fileResult = FileArgumentHelpers.HandleFileArgument(settings.Files, settings.Verbose);

            if (fileResult == -1)
            {
                return -1;
            }

            string[] files = FileArgumentHelpers.ResolveFilePaths(settings.Files!, settings.Verbose);

            try
            {
                int totalLines = 0;

                foreach (string file in files)
                {
                    string fileContents = await File.ReadAllTextAsync(file);
                    
                    using StringReader reader = new StringReader(fileContents);
                    
                    int lineCount = await _lineCounter.CountLinesAsync(reader);
                    totalLines += lineCount;

                    string label = lineCount == 1 ? Resources.WCount_App_Labels_Lines_Singular : Resources.WCount_App_Labels_Lines_Plural;

                    AnsiConsole.WriteLine($"{file} {lineCount} {label}");
                }

                if (files.Length > 1)
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
                ExceptionHelper.PrintException(ex, settings.Verbose);
                return -1;
            }
        }
    }
}
