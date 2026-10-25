# Proposal

## Why

The `TfsWebUrl` property is a value object that represents validated URLs. The `TfsWebUrlJsonConverter` writes a plain string when serializing, but data already persisted in PostgreSQL JSONB columns contains a JSON object `{"value": "...", "isValid": true}`. When reading those records, the deserializer finds an object `{` where it expected a string, causing:

```
Newtonsoft.Json.JsonReaderException: Unexpected character encountered while parsing value: {. Path '', line 1, position 1.
```

This prevents reading any entity containing `TfsWebUrl` (client, sector, service, project, company) when the value is not null.

## What Changes

- Modify `TfsWebUrlJsonConverter.ReadJson` to accept both plain string and JSON object `{"value": "...", "isValid": true}`, extracting the value from the `value` property when it is an object.
- Update planning documentation (proposal, design, tasks).

## Capabilities

### Modified Capabilities

- `persistence-layer`: The serialization/deserialization behavior of `TfsWebUrl` in PostgreSQL JSONB columns changes to accept both plain string and object `{"value": "...", "isValid": true}`. A delta spec is needed under `specs/persistence-layer/spec.md`.

## Impact

- Source file: `src/Presentation/WebAPI/Json/TfsWebUrlJsonConverter.cs`
- Affected layer: Infrastructure/Persistence (Entity Framework Core + PostgreSQL JSONB)
- Affected endpoints: all that return entities with `TfsWebUrl` (Client, Sector, Service, Project, CompanyProfile)
- No database schema changes, only serialization when reading/writing JSONB
