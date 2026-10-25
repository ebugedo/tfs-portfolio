# Testing Standards

## Requirement

Every code change MUST include a review of all existing tests affected by the modification, and new tests MUST be created when the change introduces or alters behavior not covered by current tests.

## Scope

Applies to all changes under `openspec/changes/<change>/` that modify source code, including:

- Bug fixes
- New features
- Refactors
- Serialization/deserialization changes (e.g., JSON converters)

## Obligations

1. **Review existing tests:** Identify all unit, integration, and functional tests that exercise the modified code paths.
2. **Update tests if needed:** Adjust assertions and arrangements to match the new expected behavior.
3. **Create new tests:** Add tests covering any new behavior, edge cases, or previously unhandled scenarios introduced by the change.
4. **Run the full test suite:** Execute `dotnet test` and confirm all tests pass before considering the change complete.

## Documentation

The `tasks.md` of each OpenSpec change MUST include at least one task for test review and one task for new test creation when applicable.
