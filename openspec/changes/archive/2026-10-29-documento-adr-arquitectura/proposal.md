# Proposal

## Why

The project lacks a centralized record of architecture decisions made. Currently, decisions (patterns, layers, technologies, databases, APIs, authentication, deployment) are scattered across code, configuration, comments, and documentation, without a single document that collects them. An Architecture Decision Record (ADR) document is needed to centralize all architecture decisions to facilitate maintenance, onboarding of new members, and traceability of future decisions.

## What Changes

- Create the `docs/architecture/` folder
- Create the file `docs/architecture/adr.md` following the classic ADR format: number, title, status (proposed/accepted/obsolete), context, decision, consequences, and alternatives considered
- Gather architecture decisions already implemented in the project (code, configuration, comments, and existing documentation)
- If no previously documented decisions exist, leave `adr.md` as an empty template ready to fill, explicitly indicating that no decisions are registered yet, and include an example of how to register a new decision
- The ADR document must be written in English (per `openspec-documentation-language.md`)

## Capabilities

### New Capabilities

None. This change is documentation-only: it does not introduce or modify system behavior, so no capability specification is created or modified.

### Modified Capabilities

None. No requirement changes at specification level.

## Impact

- New file: `docs/architecture/adr.md`
- New file: `openspec-documentation-language.md` (language standard for OpenSpec/SDD documents)
- Does not affect production code, APIs, dependencies, or systems of the project
- Documentation-only change
