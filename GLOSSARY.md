# Glossary

The domain vocabulary for WCount. Each term states what it is and, where it matters, what it is not.

## CLI Framework
The abstraction layer responsible for parsing command-line arguments, routing to execution logic, and providing help/version output. The current CLI Framework is System.CommandLine. Not: XenoAtom.CommandLine.

## Execution Mode
One of three routing paths determined by the presence or absence of arguments: Interactive (stdin input, no files provided), Default (file input, no flags provided, counts words/lines/bytes), or Configured (file input with specific flags, counts only what was requested). Not: command or verb (WCount has no subcommands).

## Counting Engine
The pure logic layer that produces exactly what its Count Request asks for — words, lines, characters, or bytes. It accepts no per-count booleans and no encoding parameter at its interface; encoding comes from the input source. It has no knowledge of CLI parsing, routing, or output formatting. Not: CLI layer or presentation code.

## Count Request
The single value naming which counts (lines, words, characters, bytes) a counting operation was asked to produce. The CLI Framework builds one per run; the Counting Engine fulfills it. Not: the CLI flags themselves, and not the counting results.

## CLI Contract
The exact user-facing behavior of the CLI: the set of recognized flags (-w, -l, -m, -c, -v), the output format (column-aligned numbers, dynamic spacing, filename at end, "Total" row for multi-file), exit codes (0 for success, 1 for error), and stdin piping support. Any change to the CLI Contract is a breaking change.

## Baseline
A byte-for-byte snapshot of CLI output captured from a known-good version, used as the expected value in regression tests. Baselines are generated before a migration and must not be modified unless the CLI Contract is intentionally changed.

## Composition Root
The single entry point (Program.cs) where CLI Framework types are constructed and where parsed values are translated into plain BCL types (TextReader, TextWriter, CancellationToken) before being passed to the Counting Engine and output helpers. No CLI Framework type may leak past the Composition Root.
