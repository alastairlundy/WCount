## WCount Library
Count the number of lines, words, characters, bytes, and/or maximum line length in a stream.

This is the single supporting library for the `wcount` CLI. It exposes one entry point: `CountingEngine.CountAsync(Stream, CountRequest, CancellationToken)` (in `WCountLib.Logic`), which returns a `CountResult` carrying only the counts the request asked for. The former `WCountLib.Abstractions` project and its interfaces (`IWordCounter`, `ICharacterCounter`, `IByteCounter`, `ITextReaderLogic`, `WCountInfo`) no longer exist — the engine is a concrete class with no dependency-injection seams.

## How to Use the Project
Add the nuget package to your project through your IDE or download [WCountLib](https://nuget.org/packages/wcountlib) through Nuget.

Build a `CountRequest` naming the counts you want, open the input as a `Stream`, and call the engine:

```csharp
CountingEngine engine = new();
CountResult result = await engine.CountAsync(
    File.OpenRead("input.txt"),
    new CountRequest(Words: true, Lines: true, Bytes: true, Characters: false));

Console.WriteLine($"{result.Lines} {result.Words} {result.Bytes}");
```

The engine detects the text encoding from the stream's byte-order mark (defaulting to UTF-8), counts raw bytes exactly as they come, and follows GNU `wc` semantics: newline-only lines, whitespace-token words, and display-width maximum line length.

The WCountLib package line is frozen at its last shipped version (3.0.0): no new versions will be published. The `wcount` CLI (the `WCount` package) is the single shipped product.

### Compatibility
This project targets .NET 10 (`net10.0`). Support for edge cases or niche use cases is not guaranteed.

### Licensing
The WCount library is licensed under the MPL 2.0 license.

If you'd like to contribute to the project, please visit the [GitHub Repo](https://github.com/alastairlundy/WCount/).
