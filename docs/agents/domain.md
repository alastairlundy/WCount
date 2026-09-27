# Domain Docs

How the engineering skills should consume this repo's domain documentation when exploring the codebase.

## Single-context layout

This is a single-context repo. The domain vocabulary lives in `GLOSSARY.md` at the repo root.

### Before exploring, read these

- **`GLOSSARY.md`** at the repo root.
- **`docs/adr/`** - read ADRs that touch the area you're about to work in.

If any of these files don't exist, **proceed silently**. Don't flag their absence; don't suggest creating them upfront. The producer skill (`/technical-grilling`) creates them lazily when terms or decisions actually get resolved.

### File structure

```
/
├── GLOSSARY.md
├── AGENTS.md
├── src/
│   ├── WCount.slnx
│   ├── WCountCli/              ← the CLI app; the primary deliverable
│   └── lib/
│       ├── WCountLib/          ← implementations
│       └── WCountLib.Abstractions/
├── tests/
├── test-files/                 ← CLI test assets and regression baselines
└── docs/
    ├── adr/
    └── agents/
```

### Project dependency graph

`WCountCli` → `WCountLib` → `WCountLib.Abstractions`.

The CLI app is the priority. The libraries are packaged separately and intended to be reusable, but no consumer in this repo requires that surface — treat additions to it as a cost, not a default.

## Use the glossary's vocabulary

When your output names a domain concept (in an issue title, a refactor proposal, a hypothesis, a test name), use the term as defined in `GLOSSARY.md`. Don't drift to synonyms the glossary explicitly avoids.

If the concept you need isn't in the glossary yet, that's a signal - either you're inventing language the project doesn't use (reconsider) or there's a real gap (note it for `/technical-grilling`).

## Flag ADR conflicts

If your output contradicts an existing ADR, surface it explicitly rather than silently overriding:

> _Contradicts ADR-0007 (event-sourced orders) - but worth reopening because…_
