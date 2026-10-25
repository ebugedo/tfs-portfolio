# Design

## Context

See proposal.md - Why. Current state:
- `TfsWebUrlJsonConverter.WriteJson` writes `url.Value` (a plain string) to JSON, producing `"https://example.com"` in JSONB columns.
- `TfsWebUrlJsonConverter.ReadJson` reads `reader.Value?.ToString()` expecting a string, but some persisted rows contain an object `{"value": "...", "isValid": true}` instead.
- Reading such rows throws `Newtonsoft.Json.JsonReaderException: Unexpected character encountered while parsing value: {.`
- No source code change has been made yet to handle both formats.

## Goals / Non-Goals

**Goals:**
- Make `TfsWebUrlJsonConverter.ReadJson` accept both string and object JSON formats when deserializing from JSONB.
- Preserve backward compatibility with already-stored object-formatted values.
- Keep `WriteJson` writing plain strings (as per existing spec).

**Non-Goals:**
- No changes to database schema.
- No changes to `TfsWebUrl` value object itself.
- No changes to `WriteJson` behavior (it correctly writes strings).

## Decisions

**Decision: Make `ReadJson` handle both string and object JSON formats.**
- **Rationale:** The database may contain rows written by older code or external tools that stored `{"value": "...", "isValid": true}` instead of a plain string. The reader must be tolerant of both formats without failing.
- **Alternative:** Require all existing data to be migrated to string format before deploying. Rejected because it requires a data migration and breaks if any object-formatted data remains.

## Risks / Trade-offs

- [New JSONB rows written as objects by external tools] → ReadJson now handles both formats, so reading remains robust.
- [Future code reverts to writing objects] → ReadJson already handles object input, so no regression.

## Migration Plan

No database migration is required. The fix is purely in C# deserialization logic. Existing object-formatted JSONB values will be read correctly after deploying the fixed `TfsWebUrlJsonConverter`.

## Open Questions

None.
