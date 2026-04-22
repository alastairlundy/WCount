using System.Collections.Concurrent;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Text;
using DotExtensions.Versions;
using Microsoft.Extensions.DependencyInjection;
using WCountLib.Abstractions.Counters.Segments;
using WCountLib.Abstractions.Detectors.Segments;
using WCountLib.Counters.Segments;
using WCountLib.Detectors.Segments;
using XenoAtom.Terminal;

IServiceCollection services = new ServiceCollection();

services.AddSingleton<ISegmentWordDetector, SegmentWordDetector>();
services.AddSingleton<ISegmentWordCounter, SegmentWordCounter>();
services.AddSingleton<ISegmentCharacterCounter, SegmentCharacterCounter>();
services.AddSingleton<ISegmentByteCounter, SegmentByteCounter>();

IServiceProvider serviceProvider = services.BuildServiceProvider();

RootCommand rootCommand = new("A cross-platform word counting program and wc alternative.");

Argument<string> filePathArgument = new("filepath")
{
    Description = "The file path of the file to be read.",
    DefaultValueFactory = result => "",
    Arity =  ArgumentArity.ZeroOrMore
};

Option<bool> printWordCount = new("-w")
{
    Description = "Count the number of words in a file or string.",
    DefaultValueFactory =  result => false
};

Option<bool> printCharacterCount = new("-m")
{
    Description = "Count the number of characters in a file or string.",
    DefaultValueFactory =  result => false
};

Option<bool> printLineCount = new("-l")
{
    Description = "Count the number of lines in a file or string.",
    DefaultValueFactory =  result => false
};

Option<bool> printByteCount = new("-c")
{
    Description = "Count the number of bytes in a file or string.",
    DefaultValueFactory =  result => false
};

Command versionCommand = new("--version", "Prints the version of WCount CLI to the console.")
{
    Aliases = { "-v" }
};

rootCommand.Add(versionCommand);
rootCommand.Add(filePathArgument);
rootCommand.Add(printWordCount);
rootCommand.Add(printCharacterCount);
rootCommand.Add(printLineCount);
rootCommand.Add(printByteCount);

ParseResult parseResult = rootCommand.Parse(args);

if (parseResult.Errors.Count > 0)
{
    Terminal.Error.WriteLine("Error(s) found:");
    
    foreach (ParseError error in parseResult.Errors)
    {
        Terminal.Error.WriteLine(error.Message);
    }

    return 1;
}

string text;

string? filePath = parseResult.GetValue(filePathArgument);

if (filePath is null || (filePath.Length == 0))
{
    Terminal.Open();

    StringBuilder stringBuilder = new();
    
    await foreach(TerminalEvent terminalEvent in Terminal.ReadEventsAsync())
    {
        string str = terminalEvent.ToString();

        if (!string.IsNullOrEmpty(str))
        {
            stringBuilder.Append(str);
        }
    }
    
    Terminal.Close();
    
    text = stringBuilder.ToString();
}
else
{
    try
    {
        filePath = Path.GetFullPath(filePath);

        if (!File.Exists(filePath))
        {
            Terminal.Error.WriteLine($"File not found: '{filePath}'");
            return 1;
        }

        text = await File.ReadAllTextAsync(filePath);
    }
    catch (Exception exception)
    {
        Terminal.Error.WriteLine(exception.Message);
        return 1;
    }
}

int exitCode;

switch (parseResult.CommandResult.Command.Name.ToLower())
{
    case "--version":
        // Use DotExtensions for Graceful parsing and a user-friendly version string
        string version = Version.GracefulParse(ThisAssembly.Info.Version).ToReadableString();
        
        Terminal.Out.WriteLine($"WCount CLI v{version}");
        exitCode = 0;
        break;
    default:

        IList<Option> options = parseResult.CommandResult.Command.Options;
        
        
        if()
        
        break;
}


return exitCode;

int DefaultCommand()
{
    
}

int CustomCommand()
{
    
}