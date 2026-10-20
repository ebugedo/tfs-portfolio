# Design

## Context

See proposal.md - Why. Current state:
- Repository root contains `README.md` with basic setup, architecture overview, technology stack, and deployment notes, but no dedicated docs folder exists.
- No `docs/` directory exists in the repository.
- API is documented via Scalar/OpenAPI at `/scalar/v1` (runtime), but no static API consumer guide exists.
- C4 architecture documentation does not exist; no PlantUML files are present.
- Project stack: .NET 10, ASP.NET Core Web API, Entity Framework Core 9 + Npgsql, Scalar for OpenAPI, Autofac, FluentValidation, Serilog, xUnit, Testcontainers.
- API at `localhost:8082`, Scalar at `/scalar/v1`. Authentication via `X-API-Key` header on non-GET methods.

## Goals / Non-Goals

**Goals:**
- Produce a repository user scope guide in Markdown under `docs/`.
- Produce an API consumer integration guide in Markdown under `docs/`.
- Produce C4 model documentation with PlantUML diagrams under `docs/architecture/diagrams/` (Context, Containers, Components, Code).
- All documentation written in English (repository standard).

**Non-Goals:**
- No source code changes.
- No changes to API behavior, endpoints, or authentication logic.
- No CI/CD pipeline changes.
- No automated diagram generation or validation in CI.

## Decisions

**Documentation location: `docs/` at repository root.**
- Keeps user-facing docs alongside the code they describe, visible to cloners and contributors.
- Alternative considered: `docs/` under `openspec/`. Rejected because `openspec/` is for planning artifacts only, not shipped documentation.

**C4 diagram location: `docs/architecture/diagrams/`.**
- Groups all architecture artifacts in one discoverable path.
- PlantUML `.puml` source files stored alongside any rendered PNG/SVG outputs.

**Format: Markdown for prose, PlantUML for diagrams.**
- Markdown is universally readable on GitHub and in the repo.
- PlantUML is text-based, version-controlled, and editable; diagrams can be rendered on demand.
- Alternative considered: Mermaid. Rejected because the user specified PlantUML.

**Document structure:**
- `docs/user-guide.md` — repository user scope guide.
- `docs/api-guide.md` — API consumer integration guide.
- `docs/architecture/diagrams/c4-context.puml` — System Context diagram.
- `docs/architecture/diagrams/c4-containers.puml` — Container diagram.
- `docs/architecture/diagrams/c4-components.puml` — Component diagram.
- `docs/architecture/diagrams/c4-code.puml` — Code diagram (key classes/interfaces).

**Maintenance: documentation updated manually with feature PRs.**
- No automated enforcement; contributors update docs when endpoints or architecture change.

## Risks / Trade-offs

- [Documentation drifts from code] → Contributors must update `docs/api-guide.md` when endpoints change; PR review should include doc checks.
- [PlantUML diagrams not rendered by default on GitHub] → `.puml` files remain readable as text; viewers can render via PlantUML plugins or online tools.
- [API guide becomes stale with Scalar] → Scalar at `/scalar/v1` is the source of truth for endpoint schemas; `docs/api-guide.md` provides conceptual guidance, not exhaustive schema details.

## Migration Plan

1. Create `docs/` directory at repository root.
2. Write `docs/user-guide.md`.
3. Write `docs/api-guide.md`.
4. Create `docs/architecture/diagrams/` directory.
5. Write PlantUML C4 diagrams (`c4-context.puml`, `c4-containers.puml`, `c4-components.puml`, `c4-code.puml`).
6. Review all documentation for accuracy against the current codebase.

**Rollback:** Delete `docs/` directory and `docs/architecture/diagrams/`. No code or infrastructure changes to revert.

## Open Questions

None.
