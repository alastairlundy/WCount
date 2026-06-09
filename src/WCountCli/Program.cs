/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using WCountCli.Logic;
using WCountLib.Abstractions.Detectors;
using WCountLib.Counters;
using WCountLib.Detectors;

IServiceCollection services = new ServiceCollection();

services.AddSingleton<IWordDetector, WordDetector>();
services.AddSingleton<IWordCounter, WordCounter>();
services.AddSingleton<ICharacterCounter, CharacterCounter>();
services.AddSingleton<IByteCounter, ByteCounter>();
services.AddSingleton<ITextReaderLogic, TextReaderLogic>();

IServiceProvider serviceProvider = services.BuildServiceProvider();

Option<bool> wordOption = new("-w");
wordOption.Description = Resources.Arguments_WordCount_Description;

Option<bool> lineOption = new("-l");
lineOption.Description = Resources.Arguments_LineCount_Description;

Option<bool> charOption = new("-m");
charOption.Description = Resources.Arguments_CharacterCount_Description;

Option<bool> byteOption = new("-c");
byteOption.Description = Resources.Arguments_ByteCount_Description;

Option<bool> verboseOption = new("-v");
verboseOption.Description = "Enable verbose output";

Argument<string[]> filesArgument = new("files");
filesArgument.Description = Resources.Arguments_FilePaths_Description;
filesArgument.Arity = ArgumentArity.ZeroOrMore;
filesArgument.Validators.Add(result =>
{
    if (result.Tokens.Count > 0 && result.Tokens.Select(t => t.Value).Any(f => !File.Exists(Path.GetFullPath(f))))
    {
        result.AddError("One or more files do not exist.");
    }
});

RootCommand rootCommand = new(Resources.App_Description);
rootCommand.Add(wordOption);
rootCommand.Add(lineOption);
rootCommand.Add(charOption);
rootCommand.Add(byteOption);
rootCommand.Add(verboseOption);
rootCommand.Add(filesArgument);

rootCommand.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
{
    bool showWordCount = parseResult.GetValue(wordOption);
    bool showLineCount = parseResult.GetValue(lineOption);
    bool showCharacterCount = parseResult.GetValue(charOption);
    bool showByteCount = parseResult.GetValue(byteOption);
    bool verbose = parseResult.GetValue(verboseOption);

    string[] files = parseResult.GetValue(filesArgument) ?? [];

    bool configuredArgs = new[] { showCharacterCount, showWordCount, showLineCount, showByteCount }.Any(x => x);

    async Task<int> InteractiveCommand()
    {
        ITextReaderLogic textReaderLogic = serviceProvider.GetRequiredService<ITextReaderLogic>();

        try
        {
            TextReader reader = Console.In;
            WCountInfo info = await textReaderLogic.ReadStandardInputAsync(reader, showWordCount, showLineCount,
                showCharacterCount, showByteCount, ct);

            if (configuredArgs && info.WordCount is not null && info.LineCount is not null && info.CharCount is not null)
            {
                await ResultPrintingHelper.PrintDefaultResultLine("", Console.Out, info.LineCount.Value, info.WordCount.Value,
                    info.CharCount.Value);
            }
            else
            {
                await ResultPrintingHelper.PrintCustomResultLine("", Console.Out, info.LineCount, info.WordCount, info.CharCount,
                    info.ByteCount);
            }

            return 0;
        }
        catch (Exception exception)
        {
            await Console.Error.WriteLineAsync("Ran into issues whilst reading standard input.");

            if (verbose)
            {
                await Console.Error.WriteLineAsync($"Exception Details: {exception.Message}");
            }

            return 1;
        }
    }

    async Task<int> ConfiguredCommand()
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
                    showCharacterCount, showByteCount, ct);

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

                await ResultPrintingHelper.PrintCustomResultLine(file, Console.Out, currentLines, currentWords,
                    currentChars, currentBytes);
            }

            if (files.Length > 1)
                await ResultPrintingHelper.PrintCustomResultLine(Resources.Output_Labels_Total, Console.Out, totalLines,
                    totalWords, totalChars, totalBytes);

            return 0;
        }
        catch (Exception exception)
        {
            await Console.Error.WriteLineAsync("Ran into issues whilst reading standard in.");

            if (verbose)
            {
                await Console.Error.WriteLineAsync($"Exception Details: {exception.Message}");
            }

            return 1;
        }
    }

    async Task<int> DefaultCommand()
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
                    true, false, ct);

                if (info.LineCount is not null && info.WordCount is not null && info.CharCount is not null)
                    await ResultPrintingHelper.PrintDefaultResultLine(file, Console.Out, info.LineCount.Value,
                        info.WordCount.Value, info.CharCount.Value);

                if (info.CharCount is not null)
                    totalChars += info.CharCount.Value;

                if (info.WordCount is not null)
                    totalWords += info.WordCount.Value;

                if (info.LineCount is not null)
                    totalLines += info.LineCount.Value;
            }

            if (files.Length >= 1)
                await ResultPrintingHelper.PrintDefaultResultLine(Resources.Output_Labels_Total, Console.Out, totalLines,
                    totalWords, totalChars);

            return 0;
        }
        catch (Exception exception)
        {
            await Console.Error.WriteLineAsync("Ran into issues whilst reading a file.");

            if (verbose)
            {
                await Console.Error.WriteLineAsync($"Exception Details: {exception.Message}");
            }

            return 1;
        }
    }

    if (files.Length == 0)
        return await InteractiveCommand();

    if (!configuredArgs)
        return await DefaultCommand();

    return await ConfiguredCommand();
});

ParseResult parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync(new InvocationConfiguration(), CancellationToken.None);
