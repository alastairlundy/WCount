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
using WCountLib.Abstractions.Logic;
using WCountLib.Logic;
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

Option<bool> wordOption = new("-w")
{
    Description = Resources.Arguments_WordCount_Description
};

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

    CountSelection selection = ResultPrintingHelper.ToSelection(
        showLineCount, showWordCount, showCharacterCount, showByteCount);

    async Task<int> InteractiveCommand()
    {
        ITextReaderLogic textReaderLogic = serviceProvider.GetRequiredService<ITextReaderLogic>();

        try
        {
            TextReader reader = Console.In;
            WCountInfo info = await textReaderLogic.ReadTextReaderAsync(reader, showWordCount, showLineCount,
                showCharacterCount, showByteCount, Console.InputEncoding, ct);

            await ResultPrintingHelper.PrintRow("", Console.Out, selection,
                info.LineCount, info.WordCount, info.CharCount, info.ByteCount);

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
                WCountInfo info = await textReaderLogic.ReadFileAsync(file, showWordCount, showLineCount,
                    showCharacterCount, showByteCount, null, ct);

                if (showByteCount && totalBytes is not null && info.ByteCount is not null)
                    totalBytes += info.ByteCount;
                if (showWordCount && totalWords is not null && info.WordCount is not null)
                    totalWords += info.WordCount;
                if (showCharacterCount && totalChars is not null && info.CharCount is not null)
                    totalChars += info.CharCount;
                if (showLineCount && totalLines is not null && info.LineCount is not null)
                    totalLines += info.LineCount;

                await ResultPrintingHelper.PrintRow(file, Console.Out, selection,
                    info.LineCount, info.WordCount, info.CharCount, info.ByteCount);
            }

            if (files.Length > 1)
                await ResultPrintingHelper.PrintRow(Resources.Output_Labels_Total, Console.Out, selection,
                    totalLines, totalWords, totalChars, totalBytes);

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
            CountSelection defaultSelection = ResultPrintingHelper.ToSelection(true, true, true, false);
            long totalWords = 0;
            long totalLines = 0;
            long totalChars = 0;

            foreach (string file in files.Select(f => Path.GetFullPath(f)))
            {
                WCountInfo info = await textReaderLogic.ReadFileAsync(file, true, true,
                    true, false, null, ct);

                if (info.CharCount is not null)
                    totalChars += info.CharCount.Value;
                if (info.WordCount is not null)
                    totalWords += info.WordCount.Value;
                if (info.LineCount is not null)
                    totalLines += info.LineCount.Value;

                await ResultPrintingHelper.PrintRow(file, Console.Out, defaultSelection,
                    info.LineCount, info.WordCount, info.CharCount, info.ByteCount);
            }

            if (files.Length >= 1)
                await ResultPrintingHelper.PrintRow(Resources.Output_Labels_Total, Console.Out, defaultSelection,
                    totalLines, totalWords, totalChars, null);

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
