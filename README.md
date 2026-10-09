# WCount

A Unix-`wc`-style counting tool for .NET: the `wcount` CLI counts lines, words, characters, bytes, and maximum line length in files or piped stdin using `wc`-compatible semantics.

## Projects within this repo

### WCount (CLI)
The shipped product: an installable `dotnet tool` (command `wcount`, NuGet package `WCount`) with GNU `wc`-compatible flags (`-w`, `-l`, `-m`, `-c`, `-L`, `-v`, `--files0-from`, `--total`).

### WCountLib
The counting engine behind the `wcount` CLI. It counts the lines, words, characters, bytes, and/or maximum line length of a `Stream` through a single entry point (`CountingEngine.CountAsync`). The WCountLib package line is frozen at its last shipped version (3.0.0) and receives no new releases.

| Project Name | License | Description | 
|-|-|-|
| WCount (CLI)  | MPL 2.0 | The `wcount` command-line tool: wc-compatible counting of lines, words, characters, bytes, and maximum line length in files or piped stdin. |
| WCountLib     | MPL 2.0 | The counting engine library behind the CLI, counting those same values from a `Stream` (package line frozen at 3.0.0). |
