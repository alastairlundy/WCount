using System;
using Spectre.Console;
using Spectre.Console.Cli;
using WCount.Cli.Models;
using WCountLib.Counters.Abstractions;

namespace WCount.Cli.Commands
{
    public class WordCountOnlyCommand : Command<WordCountOnlyCommand.Settings>
    {
        private readonly IWordCounter _wordCounter;

        public WordCountOnlyCommand(IWordCounter wordCounter)
        {
            _wordCounter = wordCounter;
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

            if(fileResult == -1)
            {
                return -1;
            }


            try
            {
                ulong totalWords = 0;

                foreach (string file in settings.Files!)
                {
                    ulong wordCount = _wordCounter.CountWordsInFile(file);
                    totalWords += wordCount;

                    string wordLabel = "";

                    if (wordCount == 1)
                    {
                        wordLabel = Resources.WCount_App_Labels_Words_Singular;
                    }
                    else
                    {
                        wordLabel = Resources.WCount_App_Labels_Words_Plural;
                    }

                    AnsiConsole.WriteLine($"{file} {wordCount} {wordLabel}");
                }

                if (settings.Files.Length > 1)
                {
                    if (totalWords == 0 || totalWords > 1)
                    {
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalWords} {Resources.WCount_App_Labels_Words_Plural}");
                    }
                    else
                    {
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalWords} {Resources.WCount_App_Labels_Words_Singular}");
                    }
                }

                return 0;
            }
            catch(Exception ex) 
            {
                AnsiConsole.WriteException(ex, exceptionFormats);
                return -1;
            }
        }
    }
}
