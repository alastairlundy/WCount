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
    public class BytesCountOnlyCommand : AsyncCommand<BytesCountOnlyCommand.Settings>
    {
        private readonly IByteCounter _byteCounter;

        public BytesCountOnlyCommand(IByteCounter byteCounter)
        {
            _byteCounter = byteCounter;
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

                ulong totalBytes = 0;

                foreach (string file in settings.Files!)
                {
                    string fileContents = await File.ReadAllTextAsync(file);
                    
                    using StringReader reader = new StringReader(fileContents);
                    
                    ulong byteCount =
                        await _byteCounter.CountBytesAsync(reader, Encoding.Default);
                    
                    totalBytes += byteCount;

                    string label = "";

                    if (byteCount == 1)
                    {
                        label = Resources.WCount_App_Labels_Bytes_Singular;
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
                        AnsiConsole.WriteLine($"{Resources.WCount_App_Labels_Total} {totalBytes} {Resources.WCount_App_Labels_Bytes_Singular}");
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
