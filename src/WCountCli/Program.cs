/*
    WCount Cli
    Copyright (C) 2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.CommandLine;
using System.CommandLine.Parsing;

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

Option<bool> maxLengthOption = new("-L");
maxLengthOption.Description = Resources.Arguments_MaxLineLength_Description;

Option<bool> verboseOption = new("-v");
verboseOption.Description = "Enable verbose output";

Option<string?> filesFromOption = new("--files0-from");
filesFromOption.Description = Resources.Arguments_Files0From_Description;
filesFromOption.HelpName = "F";

Option<string?> totalOption = new("--total");
totalOption.Description = Resources.Arguments_Total_Description;
totalOption.HelpName = "WHEN";

Argument<string[]> filesArgument = new("files");
filesArgument.Description = Resources.Arguments_FilePaths_Description;
filesArgument.Arity = ArgumentArity.ZeroOrMore;

RootCommand rootCommand = new(Resources.App_Description);
rootCommand.Add(wordOption);
rootCommand.Add(lineOption);
rootCommand.Add(charOption);
rootCommand.Add(byteOption);
rootCommand.Add(maxLengthOption);
rootCommand.Add(verboseOption);
rootCommand.Add(filesFromOption);
rootCommand.Add(totalOption);
rootCommand.Add(filesArgument);

rootCommand.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
{
    bool words = parseResult.GetValue(wordOption);
    bool lines = parseResult.GetValue(lineOption);
    bool characters = parseResult.GetValue(charOption);
    bool bytes = parseResult.GetValue(byteOption);
    bool maxLineLength = parseResult.GetValue(maxLengthOption);
    bool verbose = parseResult.GetValue(verboseOption);
    string[] files = parseResult.GetValue(filesArgument) ?? [];
    string? filesFrom = parseResult.GetValue(filesFromOption);
    string? totalWhen = parseResult.GetValue(totalOption);

    // A file list and file operands are two spellings of the same request.
    if (filesFrom is not null && files.Length > 0)
    {
        await Console.Error.WriteLineAsync("file operands cannot be combined with --files0-from");
        return 1;
    }

    // The GNU total modes, adopted verbatim: auto, always, only, never.
    if (totalWhen is not null && totalWhen is not ("auto" or "always" or "only" or "never"))
    {
        await Console.Error.WriteLineAsync(
            $"invalid argument '{totalWhen}' for '--total'. Valid arguments are: auto, always, only, never.");
        return 1;
    }

    TotalMode totalMode = totalWhen switch
    {
        "always" => TotalMode.Always,
        "only" => TotalMode.Only,
        "never" => TotalMode.Never,
        _ => TotalMode.Auto
    };

    // One Count Request per run. A flagless run asks for words, lines, and bytes.
    CountRequest request = words || lines || characters || bytes || maxLineLength
        ? new CountRequest(Words: words, Lines: lines, Bytes: bytes, Characters: characters, MaximumLineLength: maxLineLength)
        : new CountRequest(Words: true, Lines: true, Bytes: true, Characters: false);

    IReadOnlyList<string> names;
    if (filesFrom is null)
    {
        names = files;
    }
    else
    {
        Stream nameList;
        try
        {
            nameList = filesFrom == "-" ? Console.OpenStandardInput() : File.OpenRead(filesFrom);
        }
        catch (Exception exception) when (IsOpenFailure(exception))
        {
            await Console.Error.WriteLineAsync(
                $"wcount: cannot open '{filesFrom}' for reading: {DescribeOpenFailure(exception)}");
            return 1;
        }

        using (nameList)
        {
            names = await ReadNulSeparatedNamesAsync(nameList, ct);
        }
    }

    // The Composition Root opens every input as a stream and translates nothing
    // further: paths become file streams, and an input that cannot be opened
    // becomes a per-input error the runner prints without stopping the run.
    List<CountInput> inputs = [];
    foreach (string name in names)
    {
        if (name.Length == 0)
        {
            inputs.Add(CountInput.FailedToOpen(name, "invalid zero-length file name"));
            continue;
        }

        try
        {
            inputs.Add(CountInput.FromStream(name, File.OpenRead(name)));
        }
        catch (Exception exception) when (IsOpenFailure(exception))
        {
            inputs.Add(CountInput.FailedToOpen(name, DescribeOpenFailure(exception)));
        }
    }

    // With no files named, the standard input base stream is the one input, and
    // its row prints as counts with no label.
    if (filesFrom is null && inputs.Count == 0)
    {
        inputs.Add(CountInput.FromStream(string.Empty, Console.OpenStandardInput()));
    }

    CountingEngine engine = new();

    return await CountRunner.RunAsync(engine, request, totalMode, inputs,
        Console.Out, Console.Error, verbose, ct);
});

ParseResult parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync(new InvocationConfiguration(), CancellationToken.None);

// Reads a GNU-style file list: names separated by the NUL character. NUL — not
// the newline — ends a name, so names may contain newlines, and the list's
// final NUL ends the last name without inventing an empty one.
static async Task<IReadOnlyList<string>> ReadNulSeparatedNamesAsync(Stream stream, CancellationToken ct)
{
    using StreamReader reader = new(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true,
        bufferSize: 1024, leaveOpen: false);
    string content = await reader.ReadToEndAsync(ct);

    string[] names = content.Split('\0');
    return names.Length > 0 && names[^1].Length == 0 ? names[..^1] : names;
}

// The failures opening an input can reasonably produce; anything else keeps
// its own message so nothing is swallowed.
static bool IsOpenFailure(Exception exception) =>
    exception is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException;

static string DescribeOpenFailure(Exception exception) => exception switch
{
    FileNotFoundException or DirectoryNotFoundException => "No such file or directory",
    UnauthorizedAccessException => "Permission denied",
    _ => exception.Message
};
