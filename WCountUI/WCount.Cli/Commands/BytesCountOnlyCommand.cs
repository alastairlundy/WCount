using System;
using System.Text;

using BasisBox.Cli.Tools.WCount.Settings;

using Spectre.Console;
using Spectre.Console.Cli;

using WCountLib;
using WCountLib.Counters.Abstractions;

namespace BasisBox.Cli.Tools.WCount.Commands
{
    public class BytesCountOnlyCommand : Command<BytesCountOnlyCommand.Settings>
    {
        private readonly IByteCounter _byteCounter;

        public BytesCountOnlyCommand(IByteCounter byteCounter)
        {
            _byteCounter = byteCounter;
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

                ulong totalBytes = 0;

                foreach (string file in settings.Files!)
                {
                    ulong byteCount = _byteCounter.CountBytesInFile(file, Encoding.Default);
                    totalBytes += byteCount;

                    string label = "";

                    if (byteCount == 1)
                    {
                        label = Resources.Wcount_App_Labels_Bytes_Singular;
                    }
                    else
                    {
                        label = Resources.WCount_App_Labels_Bytes_Plural;
                    }

                    AnsiConsole.WriteLine($"{file} {byteCount} {label}");
                }

                if (settings.Files.Length > 1)
                {
                    if (totalBytes == 0 || totalBytes > 1)
                    {
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalBytes} {Resources.WCount_App_Labels_Bytes_Plural}");
                    }
                    else
                    {
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalBytes} {Resources.Wcount_App_Labels_Bytes_Singular}");
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
