/*
	WCount CLI
	Copyright (c) Alastair Lundy 2024-2025
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

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
// ReSharper disable ClassNeverInstantiated.Global

namespace WCount.Cli.Commands;

public class WCountCommand : AsyncCommand<WCountCommand.Settings>
{
    private readonly IWordCounter _wordCounter;
    private readonly ICharacterCounter _charCounter;
    private readonly IByteCounter _byteCounter;
    private readonly ILineCounter _lineCounter;

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
        public bool OnlyLineCount { get; init; }
        
        [CommandOption("-w|--word-count")]
        [DefaultValue(false)]
        public bool OnlyWordCount { get; init; }
        
        [CommandOption("-m|--character-count")]
        [DefaultValue(false)]
        public bool OnlyCharacterCount { get; init; }
        
        [CommandOption("-c|--byte-count")]
        [DefaultValue(false)]
        public bool OnlyByteCount { get; init; }
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
            Grid grid = new();

                if (settings.OnlyLineCount)
                {                    
                    grid.AddColumn();
                    grid.AddColumn();

                    int totalLines = 0;
                    
                    foreach (string file in files)
                    {
                       if(Path.IsPathFullyQualified(file) && File.Exists(file))
                        {
						    string fileContents = await File.ReadAllTextAsync(file);

                            using StringReader reader = new StringReader(fileContents);
                            
						    int lineCount = await _lineCounter.CountLinesAsync(reader);

						    totalLines += lineCount;
						    
                            grid.AddRow(new[] { lineCount.ToString(), new TextPath(file).ToString()! });
					    }
                    }

                    grid.AddRow(new[] { totalLines.ToString() , Resources.WCount_App_Labels_Total});
                    
                    AnsiConsole.Write(grid);
                    return 0;
                }

                if (settings.OnlyWordCount)
                {                    
                    grid.AddColumn();
                    grid.AddColumn();

                    ulong totalWords = 0;
                    
                    foreach (string file in files)
                    {
                        string fileContents = await File.ReadAllTextAsync(file);
                        
                        using StringReader reader = new StringReader(fileContents);
                        
                        ulong wordCount = await _wordCounter.CountWordsAsync(reader);
                        totalWords += wordCount;
                        grid.AddRow(new[] { wordCount.ToString(), file});
                    }

                    grid.AddRow(new[] { totalWords.ToString(), Resources.WCount_App_Labels_Total});
                    
                    AnsiConsole.Write(grid);
                    return 0;
                }

                if (settings.OnlyCharacterCount)
                {                    
                    grid.AddColumn();
                    grid.AddColumn();

                    ulong totalChars = 0;
                    
                    foreach (string file in files)
                    {
                        string fileContents = await File.ReadAllTextAsync(file);
                        
                        using StringReader reader = new StringReader(fileContents);
                        
                        ulong charCount = await _charCounter.CountCharactersAsync(reader, Encoding.Default);
                        totalChars += charCount;
                        grid.AddRow(new[] { charCount.ToString(), file });
                    }

                    grid.AddRow(totalChars.ToString(), Resources.WCount_App_Labels_Total);
                    
                    AnsiConsole.Write(grid);
                    return 0;
                }
                
                if (settings.OnlyByteCount)
                {
                    grid.AddColumn();
                    grid.AddColumn();

                    ulong totalBytes = 0;
                    
                    foreach (string file in files)
                    {
                        string fileContents = await File.ReadAllTextAsync(file);
                        
                        using StringReader reader = new StringReader(fileContents);
                        
                        ulong byteCount = await _byteCounter.CountBytesAsync(reader, Encoding.Default);
                        totalBytes += byteCount;
                        grid.AddRow(new[] { byteCount.ToString(), file});
                    }

                    grid.AddRow(new[] { totalBytes.ToString(), Resources.WCount_App_Labels_Total});

                    AnsiConsole.Write(grid);
                    return 0;
                }

                if (!settings.OnlyWordCount &&
                    !settings.OnlyLineCount &&
                    !settings.OnlyByteCount &&
                    !settings.OnlyCharacterCount)
                {
                    int totalLineCount = 0;
                    ulong totalWordCount = 0;
                    ulong totalCharCount = 0;

                    foreach (string file in files)
                    {
                        string fileContents = await File.ReadAllTextAsync(file);
                        
                        using StringReader reader = new StringReader(fileContents);
                        
                        totalLineCount += await _lineCounter.CountLinesAsync(reader);
                        totalWordCount += await _wordCounter.CountWordsAsync(reader);
                        totalCharCount += await _charCounter.CountCharactersAsync(reader, Encoding.Default);
                    }
                    
                    grid.AddColumn();
                    grid.AddColumn();
                    grid.AddColumn();
                    
                    foreach (string file in files)
                    {
                        string fileContents = await File.ReadAllTextAsync(file);
                        
                        using StringReader reader = new StringReader(fileContents);
                        
                        int lineCount = await _lineCounter.CountLinesAsync(reader);
                        ulong wordCount = await _wordCounter.CountWordsAsync(reader);
                        ulong charCount = await _charCounter.CountCharactersAsync(reader, Encoding.Default);
                        
                        grid.AddRow(new[] { lineCount.ToString(), wordCount.ToString(), charCount.ToString(), file});
                    }

                    grid.AddRow(new[] { totalLineCount.ToString(), totalWordCount.ToString(), totalCharCount.ToString(), Resources.WCount_App_Labels_Total});
                    
                    AnsiConsole.Write(grid);
                    return 0;
                }
        }
        catch(Exception exception)
        {
            ExceptionHelper.PrintException(exception, settings.Verbose);
            return -1;
        }

        return -1;
    }
}