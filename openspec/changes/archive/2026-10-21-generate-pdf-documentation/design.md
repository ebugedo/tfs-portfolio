# Design

## Context

See proposal.md - Why. Current state:
- Repository root contains `README.md` with basic setup, architecture overview, technology stack, and deployment notes.
- A `docs/` directory exists from the previous change (`generate-system-documentation`) containing `user-guide.md`, `api-guide.md`, and PlantUML C4 diagrams under `docs/architecture/diagrams/`.
- Markdown documentation files exist from this change: `docs/Tfs.Portfolio-Documentation.md` (English) and `docs/Tfs.Portfolio-Documentation.es.md` (Spanish).
- Project stack: .NET 10, ASP.NET Core Web API, Entity Framework Core 9 + Npgsql, Scalar for OpenAPI, Autofac, FluentValidation, Serilog, xUnit, Testcontainers.
- API at `localhost:8082` (development) and `https://api-portfolio.timeforsoftware.com/scalar/v1` (production). Authentication via `X-API-Key` header on non-GET methods.
- OpenCode with OpenSpec (spec-driven flow) and free Zen models of OpenCode were used for planning and development — this fact is documented in the documentation files.
- C4 architecture diagrams exist under `docs/architecture/diagrams/` (c4-context.puml, c4-containers.puml, c4-components.puml, c4-code.puml).
- Source code repository: https://github.com/ebugedo/tfs-portfolio

## Goals / Non-Goals

**Goals:**
- Produce Markdown source files (`docs/Tfs.Portfolio-Documentation.md` and `docs/Tfs.Portfolio-Documentation.es.md`) covering all 13 required sections in English and Spanish.
- Produce LinkedIn post documents (`docs/Tfs.Portfolio-LinkedIn.md` in English and `docs/Tfs.Portfolio-LinkedIn.es.md` in Spanish) announcing the API built with OpenCode, SDD (OpenSpec), free Zen models, running in production, with links to GitHub repo and production Scalar docs, changing "Documentación interactiva" to "API en producción"
- Title reflects the fundamental idea: API built with OpenCode, SDD (with OpenSpec), free models, professional API in production.
- Include the OpenCode + OpenSpec + free Zen models fact explicitly in the Methodology section of both language versions.
- Organize virtues into three differentiated groups (OpenCode/OpenSpec/Zen Models, SDD/Clean/DDD, Technologies) in both languages.
- Organize problems into three differentiated groups (OpenCode/OpenSpec/Zen Models, SDD/Clean/DDD, Technologies) in both languages.
- Reference C4 architecture documentation under docs/architecture/diagrams/
- Reference source code repository: https://github.com/ebugedo/tfs-portfolio
- Reference production API documentation: https://api-portfolio.timeforsoftware.com/scalar/v1
- Documents are single-language per file (English titles in English file, Spanish titles in Spanish file).
- No cover page — documents are intended for web presentation.

**Non-Goals:**
- No source code changes.
- No changes to API behavior, endpoints, or authentication logic.
- No CI/CD pipeline changes.
- No PDF generation.
- No CSS or visual design.
- No cover page sections.

## Decisions

**Document location: `docs/` at repository root.**
- Keeps the Markdown sources alongside existing documentation (`user-guide.md`, `api-guide.md`, PlantUML diagrams).
- Alternative considered: `docs/pdf/`. Rejected — a flat `docs/` structure is simpler and more discoverable.

**Format: Markdown only.**
- Markdown is universally readable and editable; Markdown files are easy to maintain and version-control.
- Alternative considered: HTML or PDF output. Rejected — Markdown sources are the canonical deliverable for this change.

**Language: English and Spanish (bilingual, single-language per file).**
- Produces two language versions: English (`docs/Tfs.Portfolio-Documentation.md`) and Spanish (`docs/Tfs.Portfolio-Documentation.es.md`).
- Both versions contain identical content structure (13 sections).
- Each document uses only one language for titles (English titles in English file, Spanish titles in Spanish file).
- Existing docs (`user-guide.md`, `api-guide.md`) are in English; this documentation provides both English and Spanish to serve all stakeholders.

**File names:**
- English source: `docs/Tfs.Portfolio-Documentation.md`
- Spanish source: `docs/Tfs.Portfolio-Documentation.es.md`
- Using the project name (`Tfs.Portfolio`) in the filename for clarity.

**No cover page.**
- Documents are intended for web presentation, not PDF.
- No cover page sections included in either language version.

**Maintenance: documentation updated manually with feature PRs.**
- No automated enforcement; contributors update docs when endpoints or architecture change.

## Risks / Trade-offs

- [Markdown drifts from code] → Contributors must update Markdown sources when code changes; PR review should include doc checks.
- [Markdown not rendered in all viewers] → Markdown sources remain readable as plain text even without a renderer.
- [Bilingual maintenance overhead] → Both English and Spanish versions must be kept in sync when content changes; translation drift is a risk.
- [External link rot] → Links to GitHub repo and production API may change over time; review periodically.

## Migration Plan

1. Update `docs/Tfs.Portfolio-Documentation.md` covering all 13 sections in English with new references and no cover page.
2. Update `docs/Tfs.Portfolio-Documentation.es.md` covering all 13 sections in Spanish with new references and no cover page.
3. Create `docs/Tfs.Portfolio-LinkedIn.md` (English) and `docs/Tfs.Portfolio-LinkedIn.es.md` (Spanish) with LinkedIn post content announcing the API built with OpenCode, SDD (OpenSpec), free Zen models, including repository URL and production API link, changing "Documentación interactiva" to "API en producción"
4. Update the title to reflect: API built with OpenCode, SDD (OpenSpec), free Zen models, professional API in production.
4. Include OpenCode + OpenSpec + free Zen models fact in Section 4 (Metodología / Methodology) of both versions.
5. Reference C4 architecture documentation, GitHub repo, and production API URL.
6. Remove cover page sections from both files.
7. Use single-language titles per document.
8. Verify both Markdown files render correctly and all sections are present.

**Rollback:** Delete `docs/Tfs.Portfolio-Documentation.md` and `docs/Tfs.Portfolio-Documentation.es.md`. No code or infrastructure changes to revert.

## Open Questions

None.
