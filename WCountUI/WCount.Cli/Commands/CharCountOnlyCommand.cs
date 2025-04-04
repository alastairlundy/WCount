﻿/*
	WCount CLI
	Copyright (c) Alastair Lundy 2024-2025
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;

using Spectre.Console;
using Spectre.Console.Cli;

using WCount.Cli.Helpers;
using WCount.Cli.Localizations;
using WCount.Cli.Models;

namespace WCount.Cli.Commands
{
    public class CharCountOnlyCommand : AsyncCommand<CharCountOnlyCommand.Settings>
    {
        private readonly ICharacterCounter _charCounter;

        public CharCountOnlyCommand(ICharacterCounter charCounter)
        {
            _charCounter = charCounter;
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
                ulong totalChars = 0;

                foreach (string file in files)
                {
                    string fileContents = await File.ReadAllTextAsync(file);
                    
                    using StringReader reader = new StringReader(fileContents);
                    
                    ulong charCount =  await _charCounter.CountCharactersAsync(reader, Encoding.Default);
                    totalChars += charCount;

                    string label = charCount == 1 ? Resources.WCount_App_Labels_Characters_Singular : Resources.WCount_App_Labels_Characters_Plural;

                    AnsiConsole.WriteLine($"{file} {charCount} {label}");
                }

                if (files.Length > 1)
                {
                    if (totalChars == 0 || totalChars > 1)
                    {
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalChars} {Resources.WCount_App_Labels_Characters_Plural}");
                    }
                    else
                    {
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalChars} {Resources.WCount_App_Labels_Characters_Singular}");
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
