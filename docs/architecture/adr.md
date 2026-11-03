# Architecture Decision Records

This document centralizes all architecture decisions made in the project.

## Status

10 architecture decisions recorded.

## Index

| # | Title | Status | Date |
|---|-------|--------|------|
| ADR-001 | Architectural Pattern: Clean Architecture + DDD + CQRS | accepted | 2026-10-29 |
| ADR-002 | Solution and Directory Structure: src/ and test/ with layer directories | accepted | 2026-10-29 |
| ADR-003 | Domain Layer: Zero external dependencies, Guid identity, Value Objects, Domain Events | accepted | 2026-10-29 |
| ADR-004 | Application Layer: CQRS Commands/Queries as records, Handlers, DTOs, AutoMapper, FluentValidation | accepted | 2026-10-29 |
| ADR-005 | Persistence Layer: EF Core + Npgsql + PostgreSQL, Fluent API, Repositories, UnitOfWork, Migrations | accepted | 2026-10-29 |
| ADR-006 | API Layer: ASP.NET Core Web API, Autofac DI, Serilog, Scalar/OpenAPI, X-API-Key auth | accepted | 2026-10-29 |
| ADR-007 | Containerization: Multi-stage Dockerfile, non-root user, Docker Compose for local dev, GHCR | accepted | 2026-10-29 |
| ADR-008 | Production Deployment: VPS direct Docker CLI, separate PostgreSQL container, Nginx reverse proxy | accepted | 2026-10-29 |
| ADR-009 | Testing Strategy: xUnit + Moq + Bogus unit tests, Testcontainers + WebApplicationFactory + Respawn integration tests | accepted | 2026-10-29 |
| ADR-010 | Build and Naming Conventions: .NET 10, Tfs.Portfolio root namespace, Tfs.Portfolio.slnx | accepted | 2026-10-29 |

---

## ADR-001: Architectural Pattern: Clean Architecture + DDD + CQRS

**Status:** accepted

**Date:** 2026-10-29

**Context**

The project needs to isolate core business rules from infrastructure, frameworks, and database dependencies to ensure maintainability, testability, and independence from external technologies.

**Decision**

Adopt Clean Architecture combined with Domain-Driven Design (DDD) and Command Query Responsibility Segregation (CQRS) as the guiding architectural pattern.

**Consequences**

- Core business logic resides in the Domain layer with no dependency on infrastructure or frameworks.
- Business use cases are orchestrated in the Application layer via CQRS commands and queries.
- Infrastructure concerns (database, external APIs) live in the Infrastructure layer.
- Presentation concerns (API endpoints) live in the Presentation layer.
- Dependencies point inward: outer layers depend on inner layers, never the reverse.

**Alternatives Considered**

- Layered architecture (traditional N-tier): rejected because it allows dependencies to flow in both directions and couples business logic to infrastructure.
- Hexagonal architecture (Ports and Adapters): considered but not adopted as a named pattern; Clean Architecture + DDD + CQRS achieves similar goals with clearer layer boundaries for this codebase.

---

## ADR-002: Solution and Directory Structure: src/ and test/ with layer directories

**Status:** accepted

**Date:** 2026-10-29

**Context**

Production code and test code must be organized in a predictable, consistent structure that reflects the architectural layers.

**Decision**

Organize all production code under `src/` following Clean Architecture layer directories, and all test projects under `test/` with `UnitTests` and `IntegrationTests` directories:

```
src/
├── Core/
│   ├── Domain/
│   └── Application/
├── Infrastructure/
│   └── Persistence/
└── Presentation/
    └── WebAPI/
test/
├── UnitTests/
└── IntegrationTests/
```

**Consequences**

- Clear separation of concerns by layer.
- Developers can locate code by its architectural responsibility.
- Test projects mirror the production structure but are isolated in `test/`.

**Alternatives Considered**

- Single `src/` folder without layer subdirectories: rejected because it obscures architectural boundaries.
- Placing tests alongside source (e.g., `src/**/Tests`): rejected because it mixes test and production code.

---

## ADR-003: Domain Layer: Zero external dependencies, Guid identity, Value Objects, Domain Events

**Status:** accepted

**Date:** 2026-10-29

**Context**

The Domain layer must contain enterprise business logic (entities, aggregates, value objects, domain events, exceptions, repository interfaces) completely independent of infrastructure and frameworks.

**Decision**

The `Tfs.Portfolio.Domain` project references only .NET standard libraries and `Microsoft.Extensions.DependencyInjection.Abstractions` for repository interfaces. It uses `Guid` or `long` for primary keys, implements Value Objects as `readonly record struct` or `readonly record class` with value-based equality, defines Domain Events as `record` types implementing `IDomainEvent`, and provides a `DomainException` base class with specific exceptions for business rule violations. Repository interfaces are defined in Domain using only Domain types in signatures.

**Consequences**

- Domain entities have no knowledge of EF Core, ASP.NET Core, or MediatR.
- Business rules are enforced internally by aggregates with no public setters on entity properties.
- Repository interfaces are infrastructure-agnostic.
- Domain events capture business occurrences for eventual consistency or auditing.

**Alternatives Considered**

- Adding MediatR to Domain for domain event dispatching: rejected to keep Domain free of framework dependencies.
- Using Data Annotations on Domain entities: rejected; Fluent API configurations live in Persistence instead.

---

## ADR-004: Application Layer: CQRS Commands/Queries as records, Handlers, DTOs, AutoMapper, FluentValidation

**Status:** accepted

**Date:** 2026-10-29

**Context**

The Application layer orchestrates business use cases without infrastructure concerns, acting as the bridge between the API layer and the Domain layer.

**Decision**

`Tfs.Portfolio.Application` references only `Tfs.Portfolio.Domain`, `AutoMapper`, `FluentValidation`, and .NET standard libraries. Commands are `record` types implementing `ICommand` or `ICommand<TResult>` for write operations. Queries are `record` types implementing `IQuery<TResult>` for read operations. Handlers orchestrate domain logic (load aggregates, invoke methods, persist via repository interfaces, return DTOs). DTOs are `record` types for API contracts. AutoMapper profiles map between Domain entities and Application DTOs. FluentValidation validates Commands and Queries.

**Consequences**

- Application layer has no direct dependency on EF Core or ASP.NET Core.
- CQRS separates read and write concerns explicitly.
- Validation is centralized in FluentValidation validators.
- Mapping between layers is explicit via AutoMapper profiles.

**Alternatives Considered**

- Using MediatR in Application for command/query dispatching: considered but not required since handlers are invoked directly or via DI.
- Returning Domain entities directly from Application: rejected; DTOs are used to decouple API contracts from domain model.

---

## ADR-005: Persistence Layer: EF Core + Npgsql + PostgreSQL, Fluent API, Repositories, UnitOfWork, Migrations

**Status:** accepted

**Date:** 2026-10-29

**Context**

Data access must isolate database concerns from Domain and Application layers, using a relational database with proper schema management.

**Decision**

`Tfs.Portfolio.Infrastructure.Persistence` references `Tfs.Portfolio.Domain`, `Tfs.Portfolio.Application`, `Microsoft.EntityFrameworkCore`, and `Npgsql.EntityFrameworkCore.PostgreSQL`. It provides `ApplicationDbContext : DbContext` with `DbSet` for each aggregate root (`Projects`, `Clients`, `Sectors`, `Services`, `CompanyProfiles`). Entity configurations use `IEntityTypeConfiguration<T>` classes under `Configurations/` (no Data Annotations on Domain entities). Repositories implement Domain interfaces using EF Core, exposing only Domain types. `UnitOfWork` wraps `DbContext.SaveChangesAsync()` for transaction boundaries. EF Core migrations are generated and committed. Soft delete uses global query filters. PostgreSQL-specific types (`uuid`, `jsonb`, `timestamp with time zone`) are used via Fluent API. Migrations are applied automatically on API startup in all environments.

**Consequences**

- Domain entities are free of data annotations.
- Database schema is version-controlled via migrations.
- Repository pattern abstracts EF Core from upper layers.
- PostgreSQL-native types optimize storage (e.g., `jsonb` for value objects).

**Alternatives Considered**

- Dapper or micro-ORM: rejected; EF Core provides stronger typing, migrations, and change tracking.
- Data Annotations on Domain entities: rejected; keeps Domain free of persistence concerns.
- SQL Server or MySQL: rejected; PostgreSQL was chosen for JSONB support and robust feature set.

---

## ADR-006: API Layer: ASP.NET Core Web API, Autofac DI, Serilog, Scalar/OpenAPI, X-API-Key auth

**Status:** accepted

**Date:** 2026-10-29

**Context**

The Presentation layer exposes HTTP endpoints implementing the REST contract, with dependency injection, structured logging, API documentation, and authentication for write operations.

**Decision**

`Tfs.Portfolio.Api` is an ASP.NET Core Web API project using Autofac as the DI container (`Host.CreateApplicationBuilder().UseServiceProviderFactory(new AutofacServiceProviderFactory())`). Serilog is configured for structured logging with Console, File, and Seq sinks. Scalar/OpenAPI provides interactive API documentation at `/scalar/v1` in all environments. Endpoints follow REST conventions under `/api/v1/` with plural, lowercase, kebab-case resource names. RFC 7807 `ProblemDetails` are returned for all error responses. A valid `X-API-Key` header is required on all POST/PUT/DELETE/PATCH requests to protected endpoints; GET endpoints are public. Health checks expose `/health/live` and `/health/ready` (with DB check on ready). JSON serialization uses `Newtonsoft.Json` with camelCase contract resolver.

**Consequences**

- API layer references Application and Persistence but not Domain directly (via Application).
- Autofac modules register services from Application and Persistence layers.
- Write operations are protected by API key; read operations are publicly accessible.
- API documentation is available in production, not only development.

**Alternatives Considered**

- Built-in Microsoft DI: rejected; Autofac was chosen for module-based registration.
- Swagger UI: rejected; Scalar was chosen for modern, interactive API docs.
- JWT authentication: rejected; X-API-Key header is simpler for service-to-service and internal API protection.

---

## ADR-007: Containerization: Multi-stage Dockerfile, non-root user, Docker Compose for local dev, GHCR

**Status:** accepted

**Date:** 2026-10-29

**Context**

The application must be containerizable for consistent development, testing, and production environments, with secure image practices.

**Decision**

A multi-stage `Dockerfile` produces a minimal runtime image (build stage with SDK image, runtime stage with ASP.NET Runtime image, copying only published output). A non-root user is created and the container runs as that user. `.dockerignore` excludes build artifacts and secrets. `docker-compose.yml` orchestrates local development with `api` and `postgres` services (Seq optional). Source code is mounted as a volume with `dotnet watch` for hot reload. Images are published to GHCR (`ghcr.io/<owner>/<repo>`) with commit SHA and `latest` tags. `.env.development` holds local secrets (gitignored); `.env.example` is the versioned template. PostgreSQL data persists in a named volume for local development.

**Consequences**

- Developers can start the full stack with `docker compose up -d`.
- Hot reload via `dotnet watch` speeds up local development.
- Images are small (<200MB runtime) and secure (non-root user).
- Secrets are never committed to version control.

**Alternatives Considered**

- Single-stage Dockerfile: rejected; multi-stage produces significantly smaller images.
- Running as root: rejected; non-root user is a security best practice.

---

## ADR-008: Production Deployment: VPS direct Docker CLI, separate PostgreSQL container, Nginx reverse proxy

**Status:** accepted

**Date:** 2026-10-29

**Context**

Production deployment must run on a VPS using direct Docker CLI commands (not Docker Compose), with PostgreSQL as a separate container and a reverse proxy for public HTTPS access.

**Decision**

PostgreSQL runs in a dedicated container named `postgres` on the VPS, connected to a shared Docker network `nginx-net`, with a persistent volume for data (`postgres:16-alpine`). The API container named `portfolio-api` runs on the same `nginx-net` network, binding to localhost only (`127.0.0.1`) on the configured port. Nginx proxies public HTTPS requests to `http://127.0.0.1:<host-port>`. The API container uses `--restart unless-stopped`. Seq logging is disabled in production (`Seq__ServerUrl=""`, `Seq__ApiKey=""`). `ASPNETCORE_ENVIRONMENT=Production` is set. GHCR authentication uses `GHCR_PAT` on VPS for image pulls. Deployment uses `docker pull`, `docker stop`, `docker rm`, `docker run`, `docker image prune` over SSH.

**Consequences**

- Production does not use Docker Compose or Seq.
- API is not directly exposed to the internet; Nginx handles HTTPS termination.
- Containers automatically recover after crashes or host reboots.
- PostgreSQL data persists across container restarts.

**Alternatives Considered**

- Docker Compose on VPS: rejected; direct Docker CLI gives more control over individual container lifecycle.
- Exposing API directly on public port: rejected; Nginx reverse proxy provides SSL termination and additional security.

---

## ADR-009: Testing Strategy: xUnit + Moq + Bogus unit tests, Testcontainers + WebApplicationFactory + Respawn integration tests

**Status:** accepted

**Date:** 2026-10-29

**Context**

The project needs automated testing at multiple levels: fast unit tests for Domain/Application logic, and integration tests for API and database behavior.

**Decision**

`Tfs.Portfolio.UnitTests` references `Tfs.Portfolio.Domain`, `Tfs.Portfolio.Application`, `xunit`, `Moq`, `Bogus`, `FluentAssertions`, `AutoFixture` (no EF Core, no ASP.NET Core). Unit tests cover aggregate invariants, value object equality, domain event raising, domain exception throwing, and Application handlers (mocking repository interfaces and `IUnitOfWork`). Bogus `Faker<T>` generates realistic test data.

`Tfs.Portfolio.IntegrationTests` references `Testcontainers.PostgreSql` 4.13.0, `Microsoft.AspNetCore.Mvc.Testing`, `Respawn`, `xUnit`. It uses `PostgreSqlBuilder("postgres:16-alpine")` with `WebApplicationFactory<Program>` for a test host with real PostgreSQL in a container. Respawn truncates all tables between tests for isolation. Integration tests cover API endpoints, database persistence, and value object JSONB persistence.

**Consequences**

- Unit tests are fast and infrastructure-free.
- Integration tests validate real database behavior with PostgreSQL in Testcontainers.
- Test isolation is ensured via Respawn database truncation.
- Bogus generates consistent, realistic test data.

**Alternatives Considered**

- In-memory database for integration tests: rejected; Testcontainers with real PostgreSQL catches database-specific issues.
- NSubstitute instead of Moq: rejected; Moq was chosen for the project's mocking convention.

---

## ADR-010: Build and Naming Conventions: .NET 10, Tfs.Portfolio root namespace, Tfs.Portfolio.slnx

**Status:** accepted

**Date:** 2026-10-29

**Context**

The solution needs consistent build settings, SDK versioning, and naming conventions across all projects.

**Decision**

The solution file `Tfs.Portfolio.slnx` uses the XML-based format (not legacy `.sln`) and lists all 6 projects. The root namespace is `Tfs.Portfolio` for all projects. Projects are named `Tfs.Portfolio.<Layer>` matching the folder structure (`Tfs.Portfolio.Domain`, `Tfs.Portfolio.Application`, `Tfs.Portfolio.Infrastructure.Persistence`, `Tfs.Portfolio.Api`, `Tfs.Portfolio.UnitTests`, `Tfs.Portfolio.IntegrationTests`). Namespaces mirror folder structure. `Directory.Build.props` and `Directory.Build.targets` provide common build settings (TargetFramework net10.0, Nullable enable, ImplicitUsings, TreatWarningsAsErrors, LangVersion latest). `global.json` pins the .NET SDK version. `.editorconfig` enforces consistent formatting. `.gitignore` excludes `bin/`, `obj/`, `.env*`, `docker-compose.override.yml`, user secrets.

**Consequences**

- All projects share consistent build settings and target .NET 10.
- Naming is predictable and maps directly to folder structure.
- SDK version is pinned for reproducible builds.
- Warnings are treated as errors to enforce code quality.

**Alternatives Considered**

- Legacy `.slnx` format: rejected; XML-based `.slnx` is the modern format.
- Different root namespaces per layer: rejected; single root namespace `Tfs.Portfolio` simplifies naming and discovery.

---

## How to register a new decision

Use the following template for each new architecture decision. Copy the block below, increment the number, and fill in each section.

### ADR-011: <Title>

**Status:** proposed / accepted / obsolete

**Date:** YYYY-MM-DD

**Context**

Describe the issue or concern that motivated this decision. What forces are at play (technical, business, organizational)?

**Decision**

Describe the proposed change or approach. What is the decision?

**Consequences**

Describe the resulting context after applying the decision. Include positive, negative, and neutral consequences.

**Alternatives Considered**

List alternative approaches that were considered and why they were not chosen.
