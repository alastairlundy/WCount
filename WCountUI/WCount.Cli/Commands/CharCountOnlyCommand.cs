using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BasisBox.Cli.Tools.WCount.Settings;

using Spectre.Console;
using Spectre.Console.Cli;

using WCountLib;
using WCountLib.Counters.Abstractions;

namespace BasisBox.Cli.Tools.WCount.Commands
{
    public class CharCountOnlyCommand : Command<CharCountOnlyCommand.Settings>
    {
        private readonly ICharCounter _charCounter;

        public CharCountOnlyCommand(ICharCounter charCounter)
        {
            _charCounter = charCounter;
        }

        public class Settings : SharedWCountSettings
        {

        }
        public override int Execute(CommandContext context, Settings settings)
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
                ulong totalChars = 0;

                foreach (string file in settings.Files!)
                {
                    ulong charCount = +_charCounter.CountCharactersInFile(file);
                    totalChars += charCount;

                    string label = "";

                    if (charCount == 1)
                    {
                        label = Resources.WCount_App_Labels_Characters_Singular;
                    }
                    else
                    {
                        label = Resources.WCount_App_Labels_Characters_Plural;
                    }

                    AnsiConsole.WriteLine($"{file} {charCount} {label}");
                }

                if (settings.Files.Length > 1)
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
                AnsiConsole.WriteException(ex, exceptionFormats);
                return -1;
            }
        }
    }
}
