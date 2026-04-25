/*
    WCount
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Text;
using Microsoft.Extensions.DependencyInjection;
using WCountCli.Localizations;
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

IServiceProvider serviceProvider = services.BuildServiceProvider();

List<string> files = new();

bool showCharacterCount = false;
bool showWordCount = false;
bool showLineCount = false;
bool showByteCount = false;

CommandApp app = new("WCount")
{
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
    (ctx, _) => DefaultCommand(ctx)
};

return await app.RunAsync(args);

async ValueTask<int> DefaultCommand(CommandRunContext ctx, CancellationToken cancellationToken = default)
{
    bool configuredArgs = new[] { showCharacterCount, showWordCount, showLineCount, showByteCount }.Any(x => x);
    
    if (files.Count == 0)
    {
        
    }

    if (!configuredArgs)
    {
        try
        {
            int totalWords = 0;
            int totalLines = 0;
            int totalChars = 0;
            
            foreach (string file in files.Select(f => Path.GetFullPath(f)))
            {
                string fileContents = await File.ReadAllTextAsync(file, cancellationToken);

                string[] contentsLines =  fileContents.Split(Environment.NewLine);

                IWordCounter wordCounter =  serviceProvider.GetRequiredService<IWordCounter>();
                ICharacterCounter characterCounter = serviceProvider.GetRequiredService<ICharacterCounter>();

                int wordCount = wordCounter.CountWords(fileContents);
                int charCount = characterCounter.CountCharacters(fileContents, Encoding.Default);
              
                await PrintDefaultResult(file, contentsLines.Length, wordCount, charCount);

                totalChars += charCount;
                totalWords += wordCount;
                totalLines += contentsLines.Length;
            }

            await PrintDefaultResult(Resources.Output_Labels_Total, totalLines, totalWords, totalChars);

            return 0;
        }
        catch(Exception exception)
        {
            await Console.Error.WriteLineAsync("Ran into issues whilst  ");
        }
    }
    
}


async Task PrintDefaultResult(string file, int lineCount, int wordCount, int characterCount)
{
    StringBuilder stringBuilder = new();

    stringBuilder.Append(lineCount);

    int requiredSpacing = CalculateRequiredSpacing();
    for (int i = 0; i < requiredSpacing; i++)
    {
        stringBuilder.Append(' ');
    }

    stringBuilder.Append(wordCount);
    
    for (int i = 0; i < requiredSpacing; i++)
    {
        stringBuilder.Append(' ');
    }

    stringBuilder.Append(characterCount);
    
    for (int i = 0; i < requiredSpacing; i++)
    {
        stringBuilder.Append(' ');
    }
    
    stringBuilder.Append(file);

    await Console.Out.WriteLineAsync(stringBuilder.ToString());
}

int CalculateRequiredSpacing()
{
    
}