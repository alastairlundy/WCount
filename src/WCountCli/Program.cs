/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using Microsoft.Extensions.DependencyInjection;
using WCountCli.Logic;
using WCountCli.Models;
using WCountLib.Abstractions.Counters;
using WCountLib.Abstractions.Detectors;
using WCountLib.Counters;
using WCountLib.Detectors;
using XenoAtom.CommandLine;

IServiceCollection services = new ServiceCollection();

services.AddSingleton<IWordDetector, WordDetector>();
services.AddSingleton<IWordCounter, WordCounter>();
services.AddSingleton<ICharacterCounter, CharacterCounter>();
services.AddSingleton<IByteCounter, ByteCounter>();
services.AddSingleton<ITextReaderLogic, TextReaderLogic>();

IServiceProvider serviceProvider = services.BuildServiceProvider();

List<string> files = [];

bool showCharacterCount = false;
bool showWordCount = false;
bool showLineCount = false;
bool showByteCount = false;

bool verbose = false;

CommandApp app = new("WCount")
{
    {"v\\|verbose", "Enable verbose output", v => verbose = v is not null},
    "Arguments:",
    {"<files>+", Resources.Arguments_FilePaths_Description, input =>
        {
            if (File.Exists(Path.GetFullPath(input)))
            {
                files.Add(input);
            }
        },
        Validate.FileExists(),
        false
    },
    {"m", Resources.Arguments_CharacterCount_Description, (bool v) => showCharacterCount = v },
    {"l", Resources.Arguments_LineCount_Description, (bool v) => showLineCount = v},
    {"c", Resources.Arguments_ByteCount_Description, (bool v) => showByteCount = v},
    {"w", Resources.Arguments_WordCount_Description, (bool v) => showWordCount = v},
    new HelpOption(),
    (ctx, _) => CommandRouter(ctx)
};

return await app.RunAsync(args);

async ValueTask<int> CommandRouter(CommandRunContext ctx)
{
    bool configuredArgs = new[] { showCharacterCount, showWordCount, showLineCount, showByteCount }.Any(x => x);
    
    if (files.Count == 0)
        return await InteractiveCommand(ctx, configuredArgs);

    if (!configuredArgs)
        return await DefaultCommand(ctx);
    
    return await ConfiguredCommand(ctx);
}

async ValueTask<int> InteractiveCommand(CommandRunContext ctx, bool configuredArgs)
{
    ITextReaderLogic textReaderLogic = serviceProvider.GetRequiredService<ITextReaderLogic>();
    
    try
    {
        WCountInfo info = await textReaderLogic.ReadStandardInputAsync(showWordCount, showLineCount, showCharacterCount,
            showByteCount, configuredArgs);

        if (configuredArgs && info.WordCount is not null && info.LineCount is not null && info.CharCount is not null)
        {
            await ResultPrintingHelper.PrintDefaultResultLine("", ctx, info.LineCount.Value, info.WordCount.Value,
                info.CharCount.Value);
        }
        else
        {
            await ResultPrintingHelper.PrintCustomResultLine("", ctx, info.LineCount, info.WordCount, info.CharCount,
                info.ByteCount);
        }

        return 0;
    }
    catch (Exception exception)
    {
        await ctx.Error.WriteLineAsync("Ran into issues whilst reading standard input.");

        if (verbose)
        {
            await ctx.Error.WriteLineAsync($"Exception Details: {exception.Message}");
        }

        return 1;
    }
}

async ValueTask<int> ConfiguredCommand(CommandRunContext ctx)
{
    ITextReaderLogic textReaderLogic = serviceProvider.GetRequiredService<ITextReaderLogic>();
    
    try
    {
        long? totalWords = showWordCount ? 0 : null;
        long? totalLines = showLineCount ? 0 : null;
        long? totalChars = showCharacterCount ? 0 : null;
        long? totalBytes = showByteCount ? 0 : null;
            
        foreach (string file in files.Select(f => Path.GetFullPath(f)))
        {
            long? currentWords = showWordCount ? 0 : null;
            long? currentLines = showLineCount ? 0 : null;
            long? currentChars = showCharacterCount ? 0 : null;
            long? currentBytes = showByteCount ? 0 : null;
            
            WCountInfo info = await textReaderLogic.ReadFileAsync(file, showWordCount, showLineCount,
                showCharacterCount, showByteCount, true);
            
            if (showByteCount && totalBytes is not null && info.ByteCount is not null)
            {
                currentBytes = info.ByteCount;
                totalBytes += currentBytes;
            }

            if (showWordCount && totalWords is not null && info.WordCount is not null)
            {
                currentWords = info.WordCount;
                totalWords += currentWords;
            }

            if (showCharacterCount && totalChars is not null && info.CharCount is not null)
            {
                currentChars = info.CharCount;
                totalChars += currentChars;
            }

            if (showLineCount && totalLines is not null && info.LineCount is not null)
            {
                currentLines = info.LineCount;
                totalLines += currentLines;
            }
            
            await ResultPrintingHelper.PrintCustomResultLine(file, ctx, currentLines, currentWords,
                currentChars, currentBytes);
        }

        if(files.Count > 1)
            await ResultPrintingHelper.PrintCustomResultLine(Resources.Output_Labels_Total, ctx, totalLines,
                totalWords, totalChars, totalBytes);

        return 0;
    }
    catch(Exception exception)
    {
        await ctx.Error.WriteLineAsync("Ran into issues whilst reading standard in.");
        
        if (verbose)
        {
            await ctx.Error.WriteLineAsync($"Exception Details: {exception.Message}");
        }
        
        return 1;
    }
}

async ValueTask<int> DefaultCommand(CommandRunContext ctx)
{
    ITextReaderLogic textReaderLogic = serviceProvider.GetRequiredService<ITextReaderLogic>();
    
    try
    {
        long totalWords = 0;
        long totalLines = 0;
        long totalChars = 0;
            
        foreach (string file in files.Select(f => Path.GetFullPath(f)))
        {
            WCountInfo info = await textReaderLogic.ReadFileAsync(file, true, true,
                true, false, false);
              
            if(info.LineCount is not null && info.WordCount is not null && info.CharCount is not null)
                await ResultPrintingHelper.PrintDefaultResultLine(file, ctx, info.LineCount.Value, 
                    info.WordCount.Value, info.CharCount.Value);

            if(info.CharCount is not null)
                totalChars += info.CharCount.Value;
            
            if(info.WordCount is not null) 
                totalWords += info.WordCount.Value;
            
            if(info.LineCount is not null)
                totalLines += info.LineCount.Value;
        }

        if(files.Count > 1)
            await ResultPrintingHelper.PrintDefaultResultLine(Resources.Output_Labels_Total, ctx, totalLines, 
                totalWords, totalChars);

        return 0;
    }
    catch(Exception exception)
    {
        await ctx.Error.WriteLineAsync("Ran into issues whilst reading a file.");
        
        if (verbose)
        {
            await ctx.Error.WriteLineAsync($"Exception Details: {exception.Message}");
        }
        
        return 1;
    }
}