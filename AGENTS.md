# WCount agent notes

WCount is a Unix-`wc`-style counting tool for .NET. The shipped product is the `wcount` CLI: it counts lines, words, characters, bytes, and maximum line length in files or piped stdin using `wc`-compatible semantics (flags `-w` words, `-l` lines, `-m` chars, `-c` bytes, `-L` max line length, `-v` verbose, `--files0-from` NUL-separated file list, `--total` WHEN; no subcommands). The `WCountLib` project under `src/lib/` implements the counting engine the CLI calls into.

## Solution & commands

- The only solution is `src/WCount.slnx`. There is no project or solution at the repo root, so bare `dotnet build` / `dotnet test` fail with MSB1003 — always target `src/WCount.slnx` or `--project <csproj>`.
- Match CI (`.github/workflows/build-and-test.yml`): `dotnet restore src/WCount.slnx` → `dotnet build src/WCount.slnx -c Release --no-restore` → `dotnet test src/WCount.slnx -c Release --no-build`.
- Single test: this repo is TUnit on Microsoft.Testing.Platform (`global.json` pins the runner), so VSTest `--filter` doesn't apply. Use a treenode filter:
  `dotnet test --project tests/WCountCli.Testing/WCountCli.Testing.csproj -c Release --no-build --treenode-filter "/*/*/CliIntegrationTests/Help_ReturnsUsage"`
  Paths are `/<Assembly>/<Namespace>/<Class>/<Test>` with `*` wildcards.
- CLI integration tests spawn the built `wcount.dll` via `dotnet exec`, so rebuild before `--no-build` runs or you test a stale CLI.

## Projects & direction

- Dependency graph: `WCountCli` → `WCountLib`. WCountCli is the shipped product (assembly `wcount`, NuGet package `WCount`); adding public library surface is a cost, not a default (`docs/agents/domain.md`).
- The CLI-first simplification folded `WCountLib.Abstractions` into `WCountLib`: `CountRequest` and `CountResult` replace `CountSelection` and `WCountInfo`, the engine exposes one entry `CountAsync(Stream, CountRequest, CancellationToken)`, and `ITextReaderLogic` plus the counter Dependency Injection registrations are gone.

## Package versions (CPM)

- Central Package Management with **two independent** `Directory.Packages.props` — one under `src/`, one under `tests/` — and versions are not shared (e.g. src pins `System.CommandLine`, tests pins `TUnit`). Add or bump a `PackageVersion` in the file governing that project's tree, then reference the package with no version.

## Fixtures & baselines

- `test-files/` fixtures and `tests/WCountCli.Testing/TestData/Baselines/` golden files are pinned `-text` in `.gitattributes`; never let git or an editor rewrite their line endings — exact bytes are under test and `CRLF.txt` must keep its CRLFs. CI runs Linux + Windows precisely to catch this drift.
- Baselines are known-good CLI output. Don't edit them unless the CLI Contract changes intentionally (a named decision + baseline regeneration, per GLOSSARY "Baseline"). `CliTestRunner.Normalise` swaps the fixture dir for `{TESTFILES}` and unifies newlines, so one LF baseline serves both OSes.
- New fixtures go in `test-files/`; the test csproj copies fixtures + baselines next to the test assembly and tests resolve paths via `AppContext.BaseDirectory` — never reference repo-root paths (breaks in git worktrees).

## Architecture rules

- Read `GLOSSARY.md` first and use its vocabulary (CLI Contract, Count Request, Counting Engine, Composition Root, Baseline); check `docs/adr/` for decisions in your area, and surface conflicts with ADRs explicitly instead of silently overriding (`docs/agents/domain.md`).
- The CLI Contract (flags `-w -l -m -c -L -v`, `--files0-from`, `--total`, column-aligned output, Total row only for multi-file runs, exit codes 0/1, stdin when no files) is the breaking-change boundary — any change to it is breaking and needs a named decision.
- No CLI-framework (System.CommandLine) types may leak past `Program.cs` (the Composition Root); parsed values become plain BCL types before reaching the Counting Engine.
- Don't reintroduce a word-detection seam: ADR-0001 deleted `IWordDetector`/`WordDetector`; word counting is wc-token counting (`string.Split`) inside the Counting Engine (`CountingEngine`).

## Code conventions

- Source files under `src/` carry the MPL-2.0 header comment (copy from `src/WCountCli/Program.cs`); test files don't.
- `ImplicitUsings` is disabled in WCountLib and both test projects; shared usings live in each project's `GlobalUsings.cs` — add new ones there, not per-file.
- `LangVersion` is pinned per project (14 in WCountLib); don't use newer language features in that project.

## Agent skills

### Issue tracker

GitHub Issues via the `gh` CLI. See `docs/agents/issue-tracker.md`.

### Triage labels

Default canonical names (needs-triage, needs-info, ready-for-agent, ready-for-human, wontfix). See `docs/agents/triage-labels.md`.

### Domain docs

Single-context: one GLOSSARY.md + docs/adr/ at the repo root. See `docs/agents/domain.md`.
