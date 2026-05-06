        # WCount AI Agent Guide

        Purpose: concise, actionable information an AI coding agent needs to be productive in this repository. Split into CLI and library guidance, then project/build/test conventions.

        ## Big picture
        WCount is composed of two kinds of projects under `src/`:
        - CLI: `WCountCli` — a small command-line front-end that parses arguments, resolves services via DI and delegates file/stdin processing to `TextReaderLogic`.
        - Libraries: `WCountLib.Abstractions` (interfaces) and `WCountLib` (implementations). Libraries are packaged separately and intended to be reusable.

        Typical data flow (concrete example): `Program.cs` (CLI) parses flags and files → resolves `ITextReaderLogic` → `TextReaderLogic.ReadTextReaderAsync` reads input in 8KB buffers (`char[8192]`) → for each chunk it calls counters (`IWordCounter`, `ICharacterCounter`, `IByteCounter`) → results aggregated into `WCountInfo` and printed by `ResultPrintingHelper`.

        ## CLI (WCountCli) – what agents must know
        - Entry point: `WCountCli/Program.cs`. DI registrations live here (singletons: `IWordDetector`, `IWordCounter`, `ICharacterCounter`, `IByteCounter`, `ITextReaderLogic`). Prefer resolving interfaces, not concrete types.
        - Arguments: uses `XenoAtom.CommandLine`. Examples from `Program.cs`: `-w` (words), `-l` (lines), `-m` (chars), `-c` (bytes), `-v` (verbose). When running manually forward CLI args after `--` when using `dotnet run`.
        - Text I/O: `TextReaderLogic` reads stdin via `Terminal.In` and files via `File.OpenText(...)`. Breakpoints for debugging: `TextReaderLogic.ReadTextReaderAsync` and `ReadTextChunk`.
        - Output formatting: `WCountCli/Helpers/ResultPrintingHelper.cs` and `FormattingHelpers.cs` control human-readable output and localization strings (`Localizations/Resources.resx`).

        ## Libraries (WCountLib and WCountLib.Abstractions)
        - Abstractions: `WCountLib.Abstractions` contains interfaces (e.g., `IWordCounter`, `ICharacterCounter`, `IByteCounter`, `IWordDetector`) — implementations must remain compatible with the contracts.
        - Implementation notes:
          - `WordCounter` uses `EnhancedLinq.SplitBy` and `Partitioner.Create` + `Parallel.ForEach` to count segments concurrently. See `WCountLib/Counters/WordCounter.cs` for the pattern.
          - `WordDetector` encapsulates what counts as a word; it has multiple overloads (`string`, `char[]`, `IEnumerable<char>`). If you change word detection, check both counters and tests.
          - `TextReaderLogic` performs chunked counting and handles platform line endings (CRLF vs LF) in `ReadTextChunk` — changes here affect cross-platform behavior.

        ## Build, pack, run and test (commands agents should use)
        Use the dotnet CLI from the repository `src/` directory. Example commands (PowerShell):

        ```powershell
        # build everything under src
        dotnet build

        # build a single project
        dotnet build .\WCountCli\WCountCli.csproj

        # run the CLI (pass args after --)
        dotnet run --project .\WCountCli -- -w -l path\to\file.txt

        # produce NuGet package for the CLI (Release)
        dotnet pack .\WCountCli -c Release

        # run tests (if a tests/ folder exists next to src/ or project-specific tests)
        dotnet test
        dotnet test .\tests\MyProject.Tests\MyProject.Tests.csproj
        ```

        Notes:
        - Projects are organized as separate subfolders under `src/` (e.g., `WCountCli`, `lib\WCountLib`). If this repository also contains a `tests/` folder, tests are typically separated from `src/` and reference the projects under `src/`.
        - If tests pass before you change code, your change must preserve that behavior. Do not modify test code to make behavior match; instead adjust implementation or add regressions-free tests.

        ## Conventions & gotchas
        - Central package management: `Directory.Packages.props` pins NuGet versions. Update versions there for cross-project consistency.
        - Global usings: `GlobalUsings.cs` files provide common imports for each project — add new global usings there rather than per-file.
        - Nullable/reference semantics: projects enable nullable (`<Nullable>enable</Nullable>`). Many aggregates use `long?` for optional counts (see `WCountInfo`).
        - Encoding & bytes: byte counting uses `Encoding.Default` (system encoding) by design; changing this alters byte counts on different machines.
        - Parallel patterns: `WordCounter` increments a shared counter using `Interlocked` after partitioned work. When editing, preserve thread-safety.
        - Line ending logic: `ReadTextChunk` tracks a `hasCharWasCR` flag to detect CRLF sequences on Windows — be careful when refactoring this logic.

        ## When you edit code — recommended checklist for agents
        1. Run `dotnet build` at `src/` and fix compilation issues.
        2. Run `dotnet test` (if tests exist). If tests fail and they passed before, investigate the implementation change; do not change tests to make them pass.
        3. Run the CLI on representative inputs (small files and stdin) to verify output formatting and totals. Example:

        ```powershell
        dotnet run --project .\WCountCli -- -w -l sample.txt
        ```

        4. If you change public API surfaces (abstractions), update dependent projects and consider package versioning.

        ## Key files to inspect first
        - `WCountCli/Program.cs` — DI registration and argument handling
        - `WCountCli/Logic/TextReaderLogic.cs` — chunked reading, encoding and line handling
        - `lib/WCountLib/Counters/WordCounter.cs` — parallel counting pattern
        - `lib/WCountLib/Detectors/WordDetector.cs` — rules that define what a "word" is
        - `Directory.Packages.props` — central package versions
        - `GlobalUsings.cs` files — shared imports per project

        Keep this file concise: prefer linking specific files above when you need to change behavior. If you add repo-level policies (formatting, testing), reflect them here so future agents follow the same checks.