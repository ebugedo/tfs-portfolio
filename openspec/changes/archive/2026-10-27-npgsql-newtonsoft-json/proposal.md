# Proposal

## Why

When the API reads/writes a `Project`, EF Core + Npgsql must serialize `Project.Technologies` (`IReadOnlyList<Technology>`) to a PostgreSQL `jsonb` column. Npgsql's default System.Text.Json serializer throws:

```
System.NotSupportedException: Type 'List`1' required dynamic JSON serialization, which requires an explicit opt-in; call 'EnableDynamicJson' on 'NpgsqlDataSourceBuilder' or NpgsqlConnection.GlobalTypeMapper ...
```

This happens because `src/Presentation/WebAPI/Program.cs` configures `ApplicationDbContext` with `options.UseNpgsql(connectionString)` but never opts into dynamic JSON nor switches to Newtonsoft.Json. The integration test factory already calls `EnableDynamicJson()` (`CustomWebApplicationFactory.cs:62`), so tests pass but the API fails.

Per the project standard (`openspec/standards/tech-stack.md:6`), **Newtonsoft.Json is used wherever possible**. Because Npgsql 10 does not expose a `UseJsonNet()` switch, driver-level JSONB serialization opts into dynamic JSON via `EnableDynamicJson()`, matching the existing integration test factory (`CustomWebApplicationFactory.cs:62`).

## What Changes

- Call `NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson()` once at WebAPI startup in `Program.cs`, before `AddDbContext<ApplicationDbContext>` is used.

## Capabilities

### Modified Capabilities

- `persistence-layer`: Npgsql JSONB serialization now opts into dynamic JSON (per updated tech-stack standard: Newtonsoft wherever possible; Npgsql 10 lacks a Newtonsoft switch), so `List<Technology>` and other `List<T>` properties serialize without throwing `System.NotSupportedException`. A delta spec is needed under `specs/persistence-layer/spec.md`.

## Impact

- Source file: `src/Presentation/WebAPI/Program.cs`
- Affected layer: Infrastructure/Persistence (Entity Framework Core + PostgreSQL JSONB)
- Affected endpoints: all that return/write `Project` (with `Technologies` jsonb column)
- No database schema changes
