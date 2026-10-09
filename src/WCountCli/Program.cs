/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.CommandLine;
using System.CommandLine.Parsing;
using WCountCli.Logic;

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
    bool words = parseResult.GetValue(wordOption);
    bool lines = parseResult.GetValue(lineOption);
    bool characters = parseResult.GetValue(charOption);
    bool bytes = parseResult.GetValue(byteOption);
    bool verbose = parseResult.GetValue(verboseOption);
    string[] files = parseResult.GetValue(filesArgument) ?? [];

    // One Count Request per run. A flagless run asks for words, lines, and bytes.
    CountRequest request = words || lines || characters || bytes
        ? new CountRequest(Words: words, Lines: lines, Bytes: bytes, Characters: characters)
        : new CountRequest(Words: true, Lines: true, Bytes: true, Characters: false);

    CountingEngine engine = new();

    return await CountRunner.RunAsync(engine, request, files, Console.OpenStandardInput(),
        Console.Out, Console.Error, verbose, ct);
});

ParseResult parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync(new InvocationConfiguration(), CancellationToken.None);
