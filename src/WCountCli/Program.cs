/*
    WCount
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using Microsoft.Extensions.DependencyInjection;
using WCountLib.Abstractions.Counters;
using WCountLib.Abstractions.Detectors;
using WCountLib.Counters;
using WCountLib.Detectors;
using XenoAtom.CommandLine;
using XenoAtom.Terminal;

IServiceCollection services = new ServiceCollection();

services.AddSingleton<IWordDetector, WordDetector>();
services.AddSingleton<IWordCounter, WordCounter>();
services.AddSingleton<ICharacterCounter, CharacterCounter>();
services.AddSingleton<IByteCounter, ByteCounter>();

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

async ValueTask<int> CommandRouter(CommandRunContext ctx, CancellationToken cancellationToken = default)
{
    bool configuredArgs = new[] { showCharacterCount, showWordCount, showLineCount, showByteCount }.Any(x => x);
    
    if (files.Count == 0)
        return await InteractiveCommand(ctx, configuredArgs, cancellationToken);

    if (!configuredArgs)
        return await DefaultCommand(ctx, cancellationToken);
    
    return await ConfiguredCommand(ctx, cancellationToken);
}

async ValueTask<int> InteractiveCommand(CommandRunContext ctx, bool configuredArgs, CancellationToken cancellationToken = default)
{
    IWordCounter wordCounter =  serviceProvider.GetRequiredService<IWordCounter>();
    ICharacterCounter characterCounter = serviceProvider.GetRequiredService<ICharacterCounter>();

    using TextReader reader = Terminal.In;
    
    long? totalWords = showWordCount || !configuredArgs? 0 : null;
    long? totalLines = showLineCount || !configuredArgs ? 0 : null;
    long? totalChars = showCharacterCount || !configuredArgs ? 0 : null;
    long? totalBytes = showByteCount ? 0 : null;

    char[] buffer = new char[8192];

    try
    {
        while (await reader.ReadAsync(buffer, 0, buffer.Length) > 0)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].ToString() == Environment.NewLine)
                {
                    totalLines += 1;
                }

                totalBytes += 1;
            }

            if (totalWords is not null)
                totalWords += Convert.ToInt64(wordCounter.CountWords(new string(buffer)));

            if (totalChars is not null)
                totalChars += Convert.ToInt64(characterCounter.CountCharacters(new string(buffer), Encoding.Default));
        }

        if(configuredArgs)
            await ResultPrintingHelper.PrintCustomResultLine("", totalLines, totalWords,
                totalChars, totalBytes);
        else
        {
            if (totalChars is not null && totalLines is not null && totalWords is not null)
            {
                await ResultPrintingHelper.PrintDefaultResultLine("", totalLines.Value, totalWords.Value,
                    totalChars.Value);
            }
        }

        return 0;
    }
    catch (Exception exception)
    {
        await Terminal.Error.WriteLineAsync("Ran into issues whilst reading standard in.");

        if (verbose)
        {
            await Terminal.Error.WriteLineAsync($"Exception Details: {exception.Message}");
        }

        return 1;
    }
}

async ValueTask<int> ConfiguredCommand(CommandRunContext ctx, CancellationToken cancellationToken = default)
{
    try
    {
        long? totalWords = showWordCount ? 0 : null;
        long? totalLines = showLineCount ? 0 : null;
        long? totalChars = showCharacterCount ? 0 : null;
        long? totalBytes = showByteCount ? 0 : null;
            
        foreach (string file in files.Select(f => Path.GetFullPath(f)))
        {
            int? currentWords = showWordCount ? 0 : null;
            int? currentLines = showLineCount ? 0 : null;
            int? currentChars = showCharacterCount ? 0 : null;
            int? currentBytes = showByteCount ? 0 : null;
            
            string fileContents = await File.ReadAllTextAsync(file, cancellationToken);

            string[] contentsLines =  fileContents.Split(Environment.NewLine);

            IWordCounter wordCounter =  serviceProvider.GetRequiredService<IWordCounter>();
            ICharacterCounter characterCounter = serviceProvider.GetRequiredService<ICharacterCounter>();
            IByteCounter byteCounter =  serviceProvider.GetRequiredService<IByteCounter>();

            if (showByteCount && totalBytes is not null)
            {
                currentBytes = byteCounter.CountBytes(fileContents, Encoding.Default);
                totalBytes += currentBytes;
            }

            if (showWordCount && totalWords is not null)
            {
                currentWords =  wordCounter.CountWords(fileContents);
                totalWords += currentWords;
            }

            if (showCharacterCount && totalChars is not null)
            {
                currentChars = characterCounter.CountCharacters(fileContents, Encoding.Default);
                totalChars += currentChars;
            }

            if (showLineCount && totalLines is not null)
            {
                currentLines = contentsLines.Length;
                totalLines += currentLines;
            }
            
            await ResultPrintingHelper.PrintCustomResultLine(file, currentLines, currentWords, currentChars,
                currentBytes);
        }

        if(files.Count > 1)
            await ResultPrintingHelper.PrintCustomResultLine(Resources.Output_Labels_Total, totalLines, totalWords, 
                totalChars, totalBytes);

        return 0;
    }
    catch(Exception exception)
    {
        await Terminal.Error.WriteLineAsync("Ran into issues whilst reading standard in.");
        
        if (verbose)
        {
            await Terminal.Error.WriteLineAsync($"Exception Details: {exception.Message}");
        }
        
        return 1;
    }
}

async ValueTask<int> DefaultCommand(CommandRunContext ctx, CancellationToken cancellationToken = default)
{
    try
    {
        long totalWords = 0;
        long totalLines = 0;
        long totalChars = 0;
            
        foreach (string file in files.Select(f => Path.GetFullPath(f)))
        {
            string fileContents = await File.ReadAllTextAsync(file, cancellationToken);

            string[] contentsLines =  fileContents.Split(Environment.NewLine);

            IWordCounter wordCounter =  serviceProvider.GetRequiredService<IWordCounter>();
            ICharacterCounter characterCounter = serviceProvider.GetRequiredService<ICharacterCounter>();

            int wordCount = wordCounter.CountWords(fileContents);
            int charCount = characterCounter.CountCharacters(fileContents, Encoding.Default);
              
            await ResultPrintingHelper.PrintDefaultResultLine(file, contentsLines.Length, wordCount, charCount);

            totalChars += charCount;
            totalWords += wordCount;
            totalLines += contentsLines.Length;
        }

        if(files.Count > 1)
            await ResultPrintingHelper.PrintDefaultResultLine(Resources.Output_Labels_Total, totalLines, totalWords, totalChars);

        return 0;
    }
    catch(Exception exception)
    {
        await Terminal.Error.WriteLineAsync("Ran into issues whilst  ");
        return 1;
    }
}