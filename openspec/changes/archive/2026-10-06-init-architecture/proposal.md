# Proposal

## Why

This project needs a foundational Clean Architecture solution structure to begin development. Currently, there is no solution, no projects, and no codebase. Establishing the base architecture now—following the OpenSpec standards for Clean Architecture + DDD + CQRS, .NET 10, and the defined naming conventions—provides a consistent, maintainable foundation for all future features.

## What Changes

- Create solution file `Tfs.Portfolio.slnx` (XML-based)
- Create 4 production projects under `src/` following Clean Architecture layers:
  - `Tfs.Portfolio.Domain` (Core/Domain)
  - `Tfs.Portfolio.Application` (Core/Application)
  - `Tfs.Portfolio.Infrastructure.Persistence` (Infrastructure/Persistence)
  - `Tfs.Portfolio.Api` (Presentation/WebAPI)
- Create 2 test projects under `test/`:
  - `Tfs.Portfolio.UnitTests`
  - `Tfs.Portfolio.IntegrationTests`
- Configure shared build props (`Directory.Build.props`, `Directory.Build.targets`), `global.json`, `.editorconfig`, `.gitignore`
- Create multi-stage `Dockerfile` and `docker-compose.yml` for local development (API + PostgreSQL + Seq)
- Configure CI/CD pipeline basics (GitHub Actions workflow structure)
- **Update integration tests to Testcontainers 4.13.0** for .NET 10 and Docker Engine v29+ compatibility:
  - Migrate from legacy `PostgreSqlContainer` to `PostgreSqlBuilder` API
  - Remove manual `Docker.DotNet` package references (Testcontainers manages transitively)
  - Ensure all Testcontainers packages aligned to 4.13.0 for binary compatibility

## Capabilities

### New Capabilities

- `solution-structure`: Establishes the solution, project layout, naming conventions, and build configuration per OpenSpec standards
- `domain-layer`: Domain entities, aggregates, value objects, domain events, domain exceptions, and repository interfaces
- `application-layer`: CQRS commands/queries/handlers, DTOs, AutoMapper profiles, validation, and application service interfaces
- `persistence-layer`: EF Core DbContext, entity configurations (Fluent API), repositories, UnitOfWork, and migrations
- `api-layer`: ASP.NET Core Web API with Autofac, Serilog, Scalar/OpenAPI, middleware pipeline, and health checks
- `containerization`: Multi-stage Dockerfile, docker-compose for dev (API + PostgreSQL + Seq), .dockerignore
- `testing-foundation`: Unit test project (xUnit + Moq + Bogus) and integration test project (Testcontainers + WebApplicationFactory)

### Modified Capabilities

- (none - greenfield project)

## Impact

- **Code**: New solution with 6 projects, ~50+ initial files
- **APIs**: RESTful endpoints at `/api/v1/<plural-resource>` with ProblemDetails errors
- **Dependencies**: .NET 10, Autofac, AutoMapper, EF Core + Npgsql, Serilog, Scalar, xUnit, Moq, Bogus, **Testcontainers 4.13.0**
- **Infrastructure**: Docker images to GHCR, GitHub Actions CI/CD, VPS deployment via SSH + Docker CLI
- **Developer Experience**: Hot reload via `dotnet watch` in Docker Compose, consistent code style via `.editorconfig`