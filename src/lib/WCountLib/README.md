## WCount Library
Count the number of lines, words, characters, and/or bytes in specified files, strings, or IEnumerables of strings.

This is the single supporting library for the `wcount` CLI. The interfaces that formerly lived in the `WCountLib.Abstractions` project (`IWordCounter`, `ICharacterCounter`, `IByteCounter`, `ITextReaderLogic`, `WCountInfo`) are folded into this project.

## How to Use the Project
Add the nuget package to your project through your IDE or download [WCountLib](https://nuget.org/packages/wcountlib) through Nuget.

The library provides `TextReaderLogic` / `ITextReaderLogic` (in `WCountLib.Logic`) for stream-level counting, allowing consumers to count files and `TextReader` streams without the CLI.

The WCountLib package line is frozen at its last shipped version (3.0.0): no new versions will be published. The `wcount` CLI (the `WCount` package) is the single shipped product.

### Compatibility
This project should work with any .NET Standard 2.0, .NET Standard 2.1, .NET 8, or newer .NET projects but support for edge cases or niche use cases is not guaranteed.

### Licensing
The WCount library is licensed under the MPL 2.0 license.

If you'd like to contribute to the project, please visit the [GitHub Repo](https://github.com/alastairlundy/WCount/).
