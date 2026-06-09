# Context

## Glossary

### CLI Framework
The abstraction layer responsible for parsing command-line arguments, routing to execution logic, and providing help/version output. The current CLI Framework is System.CommandLine 2.0.8. Not: XenoAtom.CommandLine (deprecated).

### Execution Mode
One of three routing paths determined by the presence or absence of arguments: Interactive (stdin input, no files provided), Default (file input, no flags provided, counts words/lines/chars), or Configured (file input with specific flags, counts only what was requested). Not: command or verb (WCount has no subcommands).

### Counting Engine
The pure logic layer that counts exactly what it is told to count — words, lines, characters, or bytes — based solely on boolean flags. It has no knowledge of CLI parsing, routing, or I/O sources. Not: CLI layer or I/O handler.

### CLI Contract
The exact user-facing behavior of the CLI: the set of recognized flags (-w, -l, -m, -c, -v), the output format (column-aligned numbers, dynamic spacing, filename at end, "Total" row for multi-file), exit codes (0 for success, 1 for error), and stdin piping support. Any change to the CLI Contract is a breaking change.

### Baseline
A byte-for-byte snapshot of CLI output captured from a known-good version, used as the expected value in regression tests. Baselines are generated before a migration and must not be modified unless the CLI Contract is intentionally changed.

### Composition Root
The single entry point (Program.cs) where CLI Framework types are constructed and where parsed values are translated into plain BCL types (TextReader, TextWriter, CancellationToken) before being passed to the Counting Engine and output helpers. No CLI Framework type may leak past the Composition Root.

## Invariants

<!-- Things that must always be true. -->

## Out of scope

<!-- Explicit non-goals. -->
