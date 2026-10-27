# Design

## Context

- `Project.Technologies` is `IReadOnlyList<Technology>` stored in a `jsonb` column via value conversion (`ProjectConfiguration.cs:75-84`).
- Npgsql EF Core defaults to System.Text.Json for JSONB, which rejects `List<T>` unless dynamic JSON is opted into.
- Standard `tech-stack.md:6` mandates Newtonsoft.Json for all serialization in the solution.
- Controllers already use Newtonsoft.Json (`Program.cs:61`) and persistence configurations already use `JsonConvert`.

## Goals / Non-Goals

**Goals:**
- Eliminate `System.NotSupportedException` for `List`1` dynamic JSON serialization in the WebAPI.
- Use Newtonsoft.Json wherever possible (per updated `tech-stack.md:6`); Npgsql driver-level JSONB uses dynamic JSON only because Npgsql 10 lacks a Newtonsoft switch.

**Non-Goals:**
- No database schema or migration changes.
- No switch of MVC serialization away from Newtonsoft.Json (already configured).

## Decisions

**Decision: Call `NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson()` once at WebAPI startup.**
- **Rationale:** Npgsql 10 does not expose `UseJsonNet()` on `INpgsqlTypeMapper`; `EnableDynamicJson()` is the supported opt-in recommended by the `System.NotSupportedException` and already used by `test/IntegrationTests/CustomWebApplicationFactory.cs:62`. Per the updated `tech-stack.md:6` ("Newtonsoft.Json wherever possible"), this is the best available option: Newtonsoft.Json is used wherever the stack allows it, and Npgsql's driver-level JSONB serialization falls back to dynamic JSON only because no Newtonsoft switch exists in Npgsql 10.
- **Placement:** In `Main`, before `builder.Build()` / before `AddDbContext<ApplicationDbContext>`.
- **Alternative:** `UseJsonNet()`. Rejected because it does not exist in Npgsql 10.

## Risks / Trade-offs

- Global type mapper mutation affects all Npgsql connections in the process; acceptable for a single-tenant API process.
- `Npgsql` types are transitively referenced via `Npgsql.EntityFrameworkCore.PostgreSQL`; add a direct PackageReference to WebAPI if the compiler requires it.

## Migration Plan

No database migration. Deploy the code change; existing jsonb rows continue to work.

## Open Questions

None.
