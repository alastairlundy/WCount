using System;
using Spectre.Console;
using WCount.Cli.Localizations;

namespace WCount.Cli.Helpers
{
    class FileArgumentHelpers
    {
        public static int HandleFileArgument(string? file, ExceptionFormats exceptionFormats)
        {
            if(file == null)
            {
                AnsiConsole.WriteException(new ArgumentException(Resources.Exceptions_NoFileProvided), exceptionFormats);
                return -1;
            }

            return 0;
        }
        
        internal static int HandleFileArgument(string[]? files, ExceptionFormats formats)
        {
            if(files == null)
            {
                AnsiConsole.WriteException(new NullReferenceException(Resources.Exceptions_NoFileProvided), formats);
                return -1;
            }
            else if(files.Length == 0)
            {
                AnsiConsole.WriteException(new ArgumentException(Resources.Exceptions_NoFileProvided));
                return -1;
            }

            return 0;
        }
    }
}
