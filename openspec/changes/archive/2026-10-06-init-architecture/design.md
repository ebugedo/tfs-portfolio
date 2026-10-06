# Design

## Context

Greenfield project: no existing solution, projects, or code. The proposal establishes the need for a Clean Architecture + DDD + CQRS foundation on .NET 10 with specific naming conventions (`Tfs.Portfolio` root namespace, `Tfs.Portfolio.slnx`, Docker image `tfs/portfolio-api`, API at `/api/v1/`). All standards are defined in `openspec/standars/`.

## Goals / Non-Goals

**Goals:**
- Create a buildable, testable solution with 6 projects following Clean Architecture layers
- Establish all build configuration, Docker, and test infrastructure upfront
- Ensure each layer compiles independently with correct dependency direction
- Provide working Docker Compose for local development with hot reload
- Configure CI-ready structure (GitHub Actions workflows placeholder)

**Non-Goals:**
- Implement any business domain logic (Project, Client, etc.) — only the architectural scaffolding
- Implement authentication/authorization (JWT, policies) — only DI registration points
- Create GitHub Actions workflow files — only document the structure
- Deploy to VPS — only document the deployment approach
- API versioning beyond v1 route structure

## Decisions

### 1. CQRS Implementation: Manual Interfaces over MediatR
**Decision:** Use custom interfaces `ICommand`, `ICommand<TResult>`, `IQuery<TResult>` with handler interfaces `ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResult>`, `IQueryHandler<TQuery, TResult>` instead of MediatR.

**Rationale:**
- Zero external dependency for core application logic
- Explicit handler registration in Autofac modules (clearer dependency graph)
- Easier to debug and understand for team unfamiliar with MediatR
- MediatR can be added later without changing command/query definitions

**Alternative considered:** MediatR — adds abstraction layer; good for larger teams but unnecessary overhead for this foundation.

### 2. API Style: Minimal APIs with Vertical Slice Endpoints
**Decision:** Use `MapGroup("api/v1")` with endpoint route handlers (Minimal APIs) organized by feature folder, not Controllers.

**Rationale:**
- Aligns with "Vertical Slice Endpoints" option in `api-and-http-contracts.md`
- Less boilerplate than Controllers; each endpoint self-contained
- Better matches CQRS: endpoint → command/query → handler
- Native AOT-friendly for future optimization

**Alternative considered:** Controllers — more familiar but more ceremony; Vertical Slice is the modern ASP.NET Core direction.

### 3. Validation: FluentValidation with IEndpointFilter
**Decision:** Register FluentValidation validators in Application layer; apply via `AddEndpointsApiExplorer()` + custom `IEndpointFilter` in API layer that runs validators for request DTOs.

**Rationale:**
- Keeps validation rules in Application layer (with commands/queries)
- `IEndpointFilter` runs before handler, returns 400 ProblemDetails automatically
- No need for `[Validate]` attributes on handlers

### 4. Autofac Modules per Layer
**Decision:** Each layer (Application, Persistence, API) provides an `Autofac.Module` registering its services; API layer composes them in `ConfigureContainer`.

**Rationale:**
- Explicit, modular registration matching Clean Architecture boundaries
- Easy to swap implementations (e.g., test doubles in UnitTests)
- Autofac chosen per `tech-stack.md` standard

### 5. EF Core Configuration: Fluent API Only, No Data Annotations
**Decision:** All entity configuration in `IEntityTypeConfiguration<T>` classes in Persistence layer; Domain entities are plain C# classes with no attributes.

**Rationale:**
- Enforces separation: Domain knows nothing about persistence
- Fluent API is more powerful and explicit
- Required by `database-and-migrations.md` standard

### 6. Docker Multi-Stage: Build in SDK, Runtime in ASP.NET Image
**Decision:** 
- Stage 1: `mcr.microsoft.com/dotnet/sdk:10.0` — restore, build, publish
- Stage 2: `mcr.microsoft.com/dotnet/aspnet:10.0` — copy published output, non-root user, entrypoint

**Rationale:**
- Minimal runtime image (~150MB vs ~1GB SDK)
- Security: non-root user, no build tools in production
- Standard per `deployment-and-ci-cd.md`

### 7. Docker Compose: Hot Reload via Volume Mount + dotnet watch
**Decision:** `docker-compose.yml` mounts `src/Presentation/WebAPI` as volume, runs `dotnet watch run --no-launch-profile` in container.

**Rationale:**
- Developer productivity: sub-second feedback loop
- Matches production runtime (same base image, same entrypoint concept)
- Volume mount excludes `bin/`, `obj/` via `.dockerignore`

### 8. Integration Tests: Testcontainers + Respawn
**Decision:** Use `Testcontainers.PostgreSql` 4.13.0 for real DB, `Respawn` for fast truncation between tests, `WebApplicationFactory` for in-process API host. Migrate to `PostgreSqlBuilder` API (Testcontainers 4.10+).

**Rationale:**
- Real PostgreSQL catches EF Core/SQL issues unit tests miss
- Respawn is faster than recreate/drop for test isolation
- In-process host (WebApplicationFactory) is faster than full container for API tests
- Testcontainers 4.13.0 supports .NET 10 and Docker Engine v29+ (Docker Desktop 4.38+)
- Testcontainers manages `Docker.DotNet` transitively — no manual version pinning needed
- Since 4.7.0, all Testcontainers.* packages must share the same version for binary compatibility

**Alternative considered:** Testcontainers 3.x with manual Docker.DotNet pinning — rejected; 3.x lacks .NET 10 and Docker Engine v29+ support, manual pinning is fragile.

### 9. Solution File: .slnx (XML) Format
**Decision:** Use `dotnet new sln --format slnx` to create `Tfs.Portfolio.slnx`.

**Rationale:**
- Required by `tech-stack.md`: "Use `.slnx` (XML-based solution file) format. Do NOT use legacy `.sln` format."
- Better diff/merge in Git; supported by all modern tooling

### 10. Directory.Build.props: Centralized Build Config
**Decision:** Single `Directory.Build.props` at repo root with:
- `<TargetFramework>net10.0</TargetFramework>`
- `<Nullable>enable</Nullable>`
- `<ImplicitUsings>enable</ImplicitUsings>`
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`
- `<LangVersion>latest</LangVersion>`
- `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
- Common package versions via `<PackageVersion>` properties

**Rationale:**
- Single source of truth for all projects
- Enables consistent warnings/errors across solution
- Easy to update .NET version in one place

### 11. EF Core Migrations: Automatic in Development, Manual in Production
**Decision:** Apply migrations automatically on API startup in Development environment using `context.Database.Migrate()`; use manual migration (`dotnet ef database update` or CI/CD pipeline) in Production.

**Rationale:**
- Eliminates need for PostgreSQL during development setup (migration runs on first API run)
- Developer productivity: no manual migration step required
- Production safety: manual control prevents accidental schema changes during deployment
- Follows EF Core best practices for environment-specific migration strategies

**Alternative considered:** Automatic migrations in all environments — rejected due to risk of concurrent migration attempts in scaled deployments and lack of rollback control.

## Risks / Trade-offs

| Risk | Mitigation |
|------|------------|
| Manual CQRS interfaces require more boilerplate than MediatR | Provide base classes `CommandHandlerBase<TCommand>`, `QueryHandlerBase<TQuery, TResult>` in Application to reduce repetition |
| Minimal APIs lack some Controller features (model binding, filters) | Use `IEndpointFilter` for validation; `BindAsync` for complex binding; migrate to Controllers only if needed |
| Testcontainers slows CI (container startup) | Use `Testcontainers` only in IntegrationTests; UnitTests run in milliseconds; parallelize in CI |
| Hot reload in Docker Compose may have file-watching issues on Windows | Document `DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER=1` and `DOTNET_WATCH_SUPPRESS_MSBUILD_INCREMENTALISM=1`; fallback to `dotnet run` outside container if needed |
| Autofac adds complexity vs built-in DI | Required by standard; document module pattern clearly; built-in DI insufficient for property injection and module composition |
| No authentication in foundation may require refactoring later | Register `ICurrentUserService` interface in Application; implement dummy in API; swap for JWT implementation when needed |

## Open Questions

1. **Project entity definition**: Should `Project` aggregate be included as a sample in Domain, or keep Domain completely empty until first feature? (Decision: Include minimal `Project` aggregate as reference implementation)

2. **Database seeding**: Should initial migration include seed data (e.g., default roles, admin user)? (Defer to first feature requiring it)

3. **API response envelope**: Wrap all responses in `{ data, meta }` envelope or return DTOs directly? (Standard says ProblemDetails for errors; success returns DTO directly — follow that)

4. **Logging correlation ID**: Use `Serilog.Enrichers.Span` for distributed tracing or simple `HttpContext.TraceIdentifier`? (Start simple with TraceIdentifier; upgrade when distributed tracing needed)

5. **Health check DB command**: Use `SELECT 1` or EF Core `CanConnectAsync`? (Use `CanConnectAsync` for EF-managed connection pooling)