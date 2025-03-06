using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AlastairLundy.WCountLib.Abstractions.Counters;
using Spectre.Console;
using Spectre.Console.Cli;
using WCount.Cli.Helpers;
using WCount.Cli.Localizations;
using WCount.Cli.Models;
using WCountLib.Counters.Abstractions;

namespace WCount.Cli.Commands;

public class WCountCommand : AsyncCommand<WCountCommand.Settings>
{
    private IWordCounter _wordCounter;
    private ICharacterCounter _charCounter;
    private IByteCounter _byteCounter;
    private ILineCounter _lineCounter;

    public WCountCommand(IWordCounter wordCounter, IByteCounter byteCounter, ICharacterCounter charCounter, ILineCounter lineCounter)
    {
        _byteCounter = byteCounter;
        _wordCounter = wordCounter;
        _charCounter = charCounter;
        _lineCounter = lineCounter;
    }

    public class Settings : SharedWCountSettings
    {
        [CommandOption("-l|--line-count")]
        [DefaultValue(false)]
        public bool LineCount { get; init; }
        
        [CommandOption("-w|--word-count")]
        [DefaultValue(false)]
        public bool WordCount { get; init; }
        
        [CommandOption("-m|--character-count")]
        [DefaultValue(false)]
        public bool CharacterCount { get; init; }
        
        [CommandOption("-c|--byte-count")]
        [DefaultValue(false)]
        public bool ByteCount { get; init; }
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
            Grid grid = new();

                if (settings.LineCount)
                {                    
                    grid.AddColumn();
                    grid.AddColumn();

                    int totalLines = 0;
                    
                    foreach (string file in settings.Files!)
                    {
                       if(Path.IsPathFullyQualified(file) && File.Exists(file))
                        {
						    string fileContents = await File.ReadAllTextAsync(file);

						    int lineCount = _lineCounter.CountLines(fileContents);

						    totalLines += lineCount;
						    
                            grid.AddRow(new string[] { lineCount.ToString(), new TextPath(file).ToString()! });
					    }
                    }

                    grid.AddRow(new string[] { totalLines.ToString() , Resources.WCount_App_Labels_Total});
                    
                    AnsiConsole.Write(grid);
                    return 0;
                }

                if (settings.WordCount)
                {                    
                    grid.AddColumn();
                    grid.AddColumn();

                    ulong totalWords = 0;
                    
                    foreach (string file in settings.Files!)
                    {
                        ulong wordCount = _wordCounter.CountWordsInFile(file);
                        totalWords += wordCount;
                        grid.AddRow(new string[] { wordCount.ToString(), file});
                    }

                    grid.AddRow(new string[] { totalWords.ToString(), Resources.WCount_App_Labels_Total});
                    
                    AnsiConsole.Write(grid);
                    return 0;
                }

                if (settings.CharacterCount)
                {                    
                    grid.AddColumn();
                    grid.AddColumn();

                    ulong totalChars = 0;
                    foreach (string file in settings.Files!)
                    {
                        ulong charCount = _charCounter.CountCharactersInFile(file);
                        totalChars += charCount;
                        grid.AddRow(new string[] { charCount.ToString(), file });
                    }

                    grid.AddRow(new string[] {totalChars.ToString(), Resources.WCount_App_Labels_Total});
                    
                    AnsiConsole.Write(grid);
                    return 0;
                }
                
                if (settings.ByteCount)
                {
                    grid.AddColumn();
                    grid.AddColumn();

                    ulong totalBytes = 0;
                    
                    foreach (string file in settings.Files!)
                    {
                        ulong byteCount = _byteCounter.CountBytesInFile(file, Encoding.UTF8);
                        totalBytes += byteCount;
                        grid.AddRow(new string[] { byteCount.ToString(), file});
                    }

                    grid.AddRow(new string[] { totalBytes.ToString(), Resources.WCount_App_Labels_Total});

                    AnsiConsole.Write(grid);
                    return 0;
                }

                if (!settings.WordCount && !settings.LineCount && !settings.ByteCount && !settings.CharacterCount)
                {
                    int totalLineCount = 0;
                    ulong totalWordCount = 0;
                    ulong totalCharCount = 0;

                    foreach (string file in settings.Files!)
                    {
                        totalLineCount += _lineCounter.CountLinesInFile(file);
                        totalWordCount += _wordCounter.CountWordsInFile(file);
                        totalCharCount += _charCounter.CountCharactersInFile(file);
                    }
                    
                    grid.AddColumn();
                    grid.AddColumn();
                    grid.AddColumn();
                    
                    foreach (string file in settings.Files!)
                    {
                        grid.AddRow(new string[] { _lineCounter.CountLinesInFile(file).ToString(), _wordCounter.CountWordsInFile(file).ToString(), charCounter.CountCharactersInFile(file).ToString(), file});
                    }

                    grid.AddRow(new string[] { totalLineCount.ToString(), totalWordCount.ToString(), totalCharCount.ToString(), Resources.WCount_App_Labels_Total});
                    
                    AnsiConsole.Write(grid);
                    return 0;
                }
        }
        catch(Exception exception)
        {
            AnsiConsole.WriteException(exception);
            return -1;
        }

        return -1;
    }
}