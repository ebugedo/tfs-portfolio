# C# & Code Quality Standards

## Language
- **Code identifiers** (classes, interfaces, methods, properties, variables, enums, parameters) MUST be in **English** (e.g., `Client`, `Project`, `GetByIdAsync`, `IsActive`).
- **Comments & documentation** SHOULD be in English.
- **Domain terms** in ubiquitous language may remain in Spanish in comments/docs, but code symbols use English equivalents.
- **API contracts** (JSON properties, query params, headers) use camelCase English.

## Naming Conventions
- **Classes, Interfaces, Methods & Properties:** PascalCase (e.g., `GetUserByIdQuery`, `IUserRepository`).
- **Interfaces:** Prefix with `I` (e.g., `IUnitOfWork`).
- **Private Fields:** camelCase with leading underscore (e.g., `_dbContext`).
- **Local Variables & Parameters:** camelCase (e.g., `userId`, `command`).

## Project, Solution & Namespace Conventions
- **Root Namespace:** `Tfs.Portfolio` (organization.product)
- **Solution File:** `Tfs.Portfolio.slnx` (XML-based, NOT legacy .sln)
- **Project Names & Assembly Names:** Match folder structure, PascalCase, no "Project" suffix
  - Domain: `Tfs.Portfolio.Domain` → `src/Core/Domain/Tfs.Portfolio.Domain.csproj`
  - Application: `Tfs.Portfolio.Application` → `src/Core/Application/Tfs.Portfolio.Application.csproj`
  - Persistence: `Tfs.Portfolio.Infrastructure.Persistence` → `src/Infrastructure/Persistence/Tfs.Portfolio.Infrastructure.Persistence.csproj`
  - WebAPI: `Tfs.Portfolio.Api` → `src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj`
  - UnitTests: `Tfs.Portfolio.UnitTests` → `test/UnitTests/Tfs.Portfolio.UnitTests.csproj`
  - IntegrationTests: `Tfs.Portfolio.IntegrationTests` → `test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj`

## Namespace Conventions (Folder = Namespace)
| Layer | Namespace Pattern | Example |
|-------|-------------------|---------|
| Domain | `Tfs.Portfolio.Domain.<Aggregate>.<Concept>` | `Tfs.Portfolio.Domain.Projects.Entities` |
| Application | `Tfs.Portfolio.Application.<Aggregate>.<Command/Query/Dto/Mapping>` | `Tfs.Portfolio.Application.Projects.Commands.CreateProject` |
| Persistence | `Tfs.Portfolio.Infrastructure.Persistence.<Context/Config/Repositories>` | `Tfs.Portfolio.Infrastructure.Persistence.Configurations` |
| WebAPI | `Tfs.Portfolio.Api.<Feature/Controllers/Middleware>` | `Tfs.Portfolio.Api.Controllers` |

## Docker & Container Conventions
- **Image Name:** `tfs/portfolio-api` (lowercase, kebab-case)
- **Registry:** `ghcr.io/<owner>/<repo>` (GHCR standard)
- **Tagging:** Semantic version + commit SHA + latest
  - `tfs/portfolio-api:10.0.0` (semantic)
  - `tfs/portfolio-api:abc123f` (commit SHA)
  - `tfs/portfolio-api:latest` (rolling)

## HTTP API Endpoint Conventions
- **Base Path:** `/api/v1/<plural-resource>` (lowercase, plural)
  - Example: `/api/v1/projects`, `/api/v1/clients`
- **Versioning:** Route-based via `MapGroup("api/v1")`
- **Resource Naming:** Plural, lowercase, kebab-case if multi-word
  - `/api/v1/project-tasks` (not `projectTasks` or `ProjectTasks`)

## Error & Exception Handling
- Use custom Domain Exceptions (`DomainException`) for business rule violations.
- Never catch generic `System.Exception` without rethrowing or handling appropriately.
- Validation failures must return standard HTTP 400 (ProblemDetails) in Presentation Layer.

## Async & Concurrency
- All I/O operations (database access, external APIs) MUST be asynchronous using `async`/`await`.
- Append `Async` to all asynchronous method names (e.g., `GetByIdAsync`).