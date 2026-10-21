# Proposal

## Why

The Tfs.Portfolio repository lacks structured Markdown documentation describing the project for stakeholders, onboarding, and external review. This change creates Markdown source files covering the system scope, technology stack, API endpoints, deployment infrastructure, development methodology, virtues, challenges, and conclusions of the Tfs.Portfolio API — a professional API built with OpenCode, SDD (with OpenSpec), and free Zen models, running in production.

## What Changes

- Generate `docs/Tfs.Portfolio-Documentation.md` and `docs/Tfs.Portfolio-Documentation.es.md` covering all 13 required sections in both English and Spanish:
- Generate `docs/Tfs.Portfolio-LinkedIn.md` (English) and `docs/Tfs.Portfolio-LinkedIn.es.md` (Spanish) with LinkedIn post content announcing the API built with OpenCode + OpenSpec (SDD) + free Zen models, including repository URL and production API link, changing "Documentación interactiva" to "API en producción"
  1. Introduction and System Scope — References GitHub repo and production API
  2. Technology Stack
  3. Methodology and Development Tools — OpenCode, OpenSpec, free Zen models, C4 architecture reference
  4. API Endpoints — References production Scalar at https://api-portfolio.timeforsoftware.com/scalar/v1
  5. Deployment and Infrastructure
  6. Development Virtues — Group 1: OpenCode + OpenSpec + Free Zen Models
  7. Development Virtues — Group 2: SDD, Clean Architecture and DDD
  8. Development Virtues — Group 3: Detailed Technologies
  9. Problems and Challenges — Group 1: OpenCode + OpenSpec + Free Zen Models
  10. Problems and Challenges — Group 2: SDD, Clean Architecture and DDD
  11. Problems and Challenges — Group 3: Detailed Technologies
  12. Conclusions and Recommendations
  13. Appendix: Glossary of Terms
- Include the undocumented fact that OpenCode with OpenSpec (spec-driven flow: proposal → specs → design → tasks → apply → archive) and free Zen models of OpenCode were used for planning and development — this information is not present in any project file and must be added explicitly.
- Write the documents in both English and Spanish using Markdown with clear headings, tables, and professional structure.
- Remove cover page — documents are intended for web presentation, not PDF.
- Use single-language titles per document (English titles in English file, Spanish titles in Spanish file).
- Reference the C4 architecture documentation under docs/architecture/diagrams/
- Reference the source code repository: https://github.com/ebugedo/tfs-portfolio
- Reference the production API documentation: https://api-portfolio.timeforsoftware.com/scalar/v1

## Capabilities

### New Capabilities

None. This change introduces no new system capabilities and modifies no existing system behavior.

### Modified Capabilities

None. No existing capability's requirements change.

## Impact

- Documentation: new Markdown source files under `docs/` in English and Spanish.
- No changes to source code, APIs, dependencies, or infrastructure.
- CI/CD: no changes required.