# Tasks

## 1. Documentation structure

- [x] 1.1 Create `docs/` directory at repository root and verify it exists
- [x] 1.2 Create `docs/architecture/diagrams/` directory for PlantUML C4 diagrams and verify it exists

## 2. Repository user guide

- [x] 2.1 Write `docs/user-guide.md` covering system scope, domain, objectives, high-level architecture, setup instructions, technology stack, and system boundaries; verify the file exists and contains all required sections

## 3. API consumer guide

- [x] 3.1 Write `docs/api-guide.md` covering getting started, X-API-Key authentication, main endpoints (`/api/v1/clients`, `/api/v1/sectors`, `/api/v1/services`, `/api/v1/company-profile`, `/api/v1/projects`), request/response examples, error codes (ProblemDetails), and how to access Scalar/Swagger at `/scalar/v1`; verify the file exists and contains all required sections

## 4. C4 architecture diagrams (PlantUML)

- [x] 4.1 Write `docs/architecture/diagrams/c4-context.puml` showing system context with external users and dependencies; verify the file parses as valid PlantUML syntax
- [x] 4.2 Write `docs/architecture/diagrams/c4-containers.puml` showing containers (API container, PostgreSQL, Nginx, Docker, GHCR); verify the file parses as valid PlantUML syntax
- [x] 4.3 Write `docs/architecture/diagrams/c4-components.puml` showing components within the API container (Domain, Application, Infrastructure, Presentation layers); verify the file parses as valid PlantUML syntax
- [x] 4.4 Write `docs/architecture/diagrams/c4-code.puml` showing key classes and interfaces (controllers, handlers, repositories, entities, value objects); verify the file parses as valid PlantUML syntax

## 5. Review and verification

- [x] 5.1 Review all Markdown and PlantUML files for accuracy against the current codebase (API endpoints, stack, deployment setup); verify no outdated claims remain
- [x] 5.2 Verify all documentation is written in English per repository standard; verify no non-English content remains
