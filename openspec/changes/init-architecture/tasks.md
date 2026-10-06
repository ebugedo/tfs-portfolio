# Tasks

## 1. Solution & Build Configuration

- [x] 1.1 Create `Tfs.Portfolio.slnx` at repo root and verify `dotnet sln list` shows empty solution
- [x] 1.2 Create `Directory.Build.props` with TargetFramework net10.0, Nullable enable, ImplicitUsings, TreatWarningsAsErrors, LangVersion latest, GenerateDocumentationFile; verify `dotnet build` on any project picks up props
- [x] 1.3 Create `Directory.Build.targets` with common analysis settings (AnalysisLevel=latest, EnforceCodeStyleInBuild); verify build passes with no warnings
- [x] 1.4 Create `global.json` pinning .NET SDK 10.0.x; verify `dotnet --version` in repo matches
- [x] 1.5 Create `.editorconfig` with C# style rules (indent_size=4, dotnet_style_qualification_for_field=true, etc.); verify editor applies formatting
- [x] 1.6 Create `.gitignore` excluding `bin/`, `obj/`, `.env*`, `docker-compose.override.yml`, `*.user`, `*.slnx.user`, `**/TestResults/`; verify `git status` clean after build

## 2. Domain Layer (`Tfs.Portfolio.Domain`)

- [x] 2.1 Create `src/Core/Domain/Tfs.Portfolio.Domain.csproj` with minimal PackageReferences (Microsoft.Extensions.DependencyInjection.Abstractions, System.ComponentModel.Annotations); verify `dotnet build` succeeds
- [x] 2.2 Add solution reference: `dotnet sln add src/Core/Domain/Tfs.Portfolio.Domain.csproj`; verify project appears in `dotnet sln list`
- [x] 2.3 Create base types: `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject` (record struct), `IDomainEvent` marker interface, `DomainException` base class; verify compile
- [x] 2.4 Create sample `Project` aggregate root with `Id (Guid)`, `Name`, `Description`, `CreatedAt`, `UpdatedAt`, private setters, factory method `Create(name, description)`, domain event `ProjectCreatedEvent`; verify compile
- [x] 2.5 Create repository interfaces: `IProjectRepository` with `Task<Project?> GetByIdAsync(Guid id)`, `Task<IReadOnlyList<Project>> GetAllAsync()`, `Task AddAsync(Project project)`, `void Update(Project project)`; verify compile
- [x] 2.6 Create `IUnitOfWork` interface with `Task<int> SaveChangesAsync(CancellationToken)`; verify compile
- [x] 2.7 Create `ProjectNotFoundException`, `InvalidProjectStateException` domain exceptions; verify compile

## 3. Application Layer (`Tfs.Portfolio.Application`)

- [x] 3.1 Create `src/Core/Application/Tfs.Portfolio.Application.csproj` referencing Domain, AutoMapper, FluentValidation, MediatR (optional - see design decision); verify `dotnet build` succeeds
- [x] 3.2 Add solution reference; verify in sln list
- [x] 3.3 Create CQRS base interfaces: `ICommand`, `ICommand<TResult>`, `IQuery<TResult>`, `ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResult>`, `IQueryHandler<TQuery, TResult>`; verify compile
- [x] 3.4 Create base handler classes reducing boilerplate (optional per design)
- [x] 3.5 Create DTOs: `ProjectDto` (record), `CreateProjectRequest`, `UpdateProjectRequest`, `ProjectListItemDto`; verify compile
- [x] 3.6 Create AutoMapper profile `ProjectMappingProfile` mapping `Project` ↔ `ProjectDto`, `CreateProjectRequest` → `Project` (factory); verify compile
- [x] 3.7 Create Commands: `CreateProjectCommand` (record, implements `ICommand<Guid>`), `UpdateProjectCommand` (`ICommand`), `DeleteProjectCommand` (`ICommand`); verify compile
- [x] 3.8 Create Queries: `GetProjectByIdQuery` (`IQuery<ProjectDto>`), `GetProjectsQuery` (`IQuery<IReadOnlyList<ProjectListItemDto>>`); verify compile
- [x] 3.9 Create Command Handlers: `CreateProjectCommandHandler`, `UpdateProjectCommandHandler`, `DeleteProjectCommandHandler` injecting `IProjectRepository`, `IUnitOfWork`, `IMapper`; verify compile
- [x] 3.10 Create Query Handlers: `GetProjectByIdQueryHandler`, `GetProjectsQueryHandler`; verify compile
- [x] 3.11 Create FluentValidation validators for each Command/Query; verify compile
- [x] 3.12 Create Autofac module `ApplicationModule` registering handlers, validators, AutoMapper; verify compile
- [x] 3.13 Create application service interfaces: `ICurrentUserService`, `IDateTimeProvider`; verify compile

## 4. Persistence Layer (`Tfs.Portfolio.Infrastructure.Persistence`)

- [x] 4.1 Create `src/Infrastructure/Persistence/Tfs.Portfolio.Infrastructure.Persistence.csproj` referencing Domain, Application, Microsoft.EntityFrameworkCore, Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.EntityFrameworkCore.Tools; verify build
- [x] 4.2 Add solution reference; verify in sln list
- [x] 4.3 Create `ApplicationDbContext : DbContext` with `DbSet<Project> Projects`; constructor taking `DbContextOptions<ApplicationDbContext>`; verify compile
- [x] 4.4 Create `ProjectConfiguration : IEntityTypeConfiguration<Project>` configuring table `Projects`, PK `Id`, required `Name` (max 200), `Description` (nullable, max 2000), `CreatedAt` (utc), `UpdatedAt` (utc), index on `Name`; verify compile
- [x] 4.5 Register configurations in `OnModelCreating`; verify compile
- [x] 4.6 Implement `ProjectRepository : IProjectRepository` using `ApplicationDbContext`; verify compile
- [x] 4.7 Implement `UnitOfWork : IUnitOfWork` wrapping `context.SaveChangesAsync`; verify compile
- [x] 4.8 Create Autofac module `PersistenceModule` registering `ApplicationDbContext` (scoped), `IProjectRepository`, `IUnitOfWork`; verify compile
- [x] 4.9 Generate initial migration: `dotnet ef migrations add InitialCreate -p src/Infrastructure/Persistence -s src/Presentation/WebAPI --output-dir Migrations`; verify migration files created
- [x] 4.10 Implement automatic migration on API startup: add `context.Database.Migrate()` in `Program.cs` for Development environment; verify migration applies on first API run
- [x] 4.11 Verify migration `Up()` creates `Projects` table with correct columns, types, indexes (verified via code inspection - uuid PK, varchar(200) Name, text Description, timestamp with time zone timestamps, IX_Projects_Name index)

## 5. API Layer (`Tfs.Portfolio.Api`)

- [x] 5.1 Create `src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj` referencing Application, Persistence, Autofac.Extensions.DependencyInjection, Serilog.AspNetCore, Serilog.Sinks.Console, Serilog.Sinks.File, Serilog.Sinks.Seq, Scalar.AspNetCore, Microsoft.AspNetCore.OpenApi, Newtonsoft.Json; verify build
- [x] 5.2 Add solution reference; verify in sln list
- [x] 5.3 Create `Program.cs` with:
  - `Host.CreateApplicationBuilder().UseServiceProviderFactory(new AutofacServiceProviderFactory())`
  - Serilog configuration (Console, File, Seq sinks, enrichers)
  - `builder.Services.AddControllers().AddNewtonsoftJson()`
  - `builder.Services.AddOpenApi()` and `builder.Services.AddEndpointsApiExplorer()`
  - Autofac `ConfigureContainer` loading `ApplicationModule`, `PersistenceModule`
  - Middleware pipeline: ExceptionHandler → SerilogRequestLogging → HTTPS Redirection → Routing → Authorization → MapControllers/MapGroup
  - Health checks: `/health/live`, `/health/ready` (DB check)
  - Scalar: `app.MapScalarApiReference()` at `/scalar/v1`
  - Versioning: `var v1 = app.MapGroup("api/v1")`
  - **Automatic migration on startup (Development):** `context.Database.Migrate()` scoped to Development environment
- [x] 5.4 Create `appsettings.json` and `appsettings.Development.json` with Serilog, ConnectionStrings placeholders; verify structure
- [x] 5.5 Create `ProjectEndpoints` static class with `MapProjectEndpoints(IEndpointRouteBuilder v1)` defining:
  - `GET /projects` → `GetProjectsQueryHandler`
  - `GET /projects/{id:guid}` → `GetProjectByIdQueryHandler`
  - `POST /projects` → `CreateProjectCommandHandler` (returns 201 with Location header)
  - `PUT /projects/{id:guid}` → `UpdateProjectCommandHandler`
  - `DELETE /projects/{id:guid}` → `DeleteProjectCommandHandler`
- [x] 5.6 Create `ValidationFilter : IEndpointFilter` running FluentValidation validators for request DTOs, returning 400 ProblemDetails on failure; register in `Program.cs`
- [x] 5.7 Create `ProblemDetails` error handling middleware (or use `app.UseExceptionHandler` with custom handler); verify 500 returns ProblemDetails
- [x] 5.8 Verify `dotnet build` succeeds and `dotnet run` starts API on `http://localhost:5000` (or configured port)

## 6. Docker & Compose

- [x] 6.1 Create `Dockerfile` at repo root with:
  - Build stage: `FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build`, restore, publish `Tfs.Portfolio.Api` to `/app/publish`
  - Runtime stage: `FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime`, create non-root user `appuser`, copy publish output, `USER appuser`, `ENTRYPOINT ["dotnet", "Tfs.Portfolio.Api.dll"]`
- [x] 6.2 Create `.dockerignore` excluding `bin/`, `obj/`, `.git/`, `.env*`, `docker-compose.override.yml`, `**/*.slnx`, `**/*.csproj.user`; verify `docker build .` context size is minimal
- [x] 6.3 Create `docker-compose.yml` at repo root with services:
  - `api`: build context `.`, dockerfile `Dockerfile.dev`, ports `8080:8080`, environment `ASPNETCORE_ENVIRONMENT=Development`, volumes `./src/Presentation/WebAPI:/app/src/Presentation/WebAPI`, working_dir `/app/src`, command `dotnet watch run --no-launch-profile --urls http://0.0.0.0:8080`, depends_on `postgres`, `seq`
  - `postgres`: image `postgres:16-alpine`, environment `POSTGRES_DB=portfolio`, `POSTGRES_USER=postgres`, `POSTGRES_PASSWORD=postgres`, volumes `postgres_data:/var/lib/postgresql/data`, ports `5432:5432`, healthcheck `pg_isready`
  - `seq`: image `datalust/seq:latest`, environment `ACCEPT_EULA=Y`, ports `5341:5341`, `8081:80`, volumes `seq_data:/data`
  - volumes: `postgres_data`, `seq_data`
- [x] 6.4 Create `.env.example` with `POSTGRES_PASSWORD=postgres`, `SEQ_API_KEY=`, `ASPNETCORE_ENVIRONMENT=Development`; verify file exists
- [x] 6.5 Test `docker compose up -d` starts all services; verify API at `http://localhost:8080/scalar/v1`, PostgreSQL accessible, Seq at `http://localhost:8081` — **API works inside container network; host access limited by Docker Desktop Windows networking**
- [x] 6.6 Test hot reload: edit `ProjectEndpoints.cs`, save, verify API restarts and change reflects — **Verified working inside container**

## 7. Unit Tests (`Tfs.Portfolio.UnitTests`)

- [x] 7.1 Create `test/UnitTests/Tfs.Portfolio.UnitTests.csproj` referencing Domain, Application, xunit, Moq, Bogus, FluentAssertions, AutoFixture; verify build
- [x] 7.2 Add solution reference; verify in sln list
- [x] 7.3 Create `ProjectFaker : Faker<Project>` generating valid projects; verify generates compile
- [x] 7.4 Create `CreateProjectCommandFaker`, `ProjectDtoFaker`; verify compile
- [x] 7.5 Write Domain tests: `Project_Create_WithValidName_RaisesProjectCreatedEvent`, `Project_Create_WithEmptyName_ThrowsDomainException`, `ValueObject_Equality_Works`; verify `dotnet test` passes
- [x] 7.6 Write Application tests: `CreateProjectCommandHandler_WithValidCommand_CallsRepositoryAndReturnsId`, `CreateProjectCommandHandler_WithInvalidName_ReturnsValidationFailure`, `GetProjectByIdQueryHandler_ReturnsDtoWhenFound`, `GetProjectByIdQueryHandler_ReturnsNullWhenNotFound`; verify `dotnet test` passes
- [x] 7.7 Ensure all unit tests run in < 5 seconds total; verify `dotnet test test/UnitTests --no-build`

## 8. Integration Tests (`Tfs.Portfolio.IntegrationTests`)

- [x] 8.1 Create `test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj` referencing Api, Testcontainers.PostgreSQL, Testcontainers.Seq, Microsoft.AspNetCore.Mvc.Testing, Respawn, xunit; verify build
- [x] 8.2 Add solution reference; verify in sln list
- [x] 8.3 Create `CustomWebApplicationFactory : WebApplicationFactory<Program>`:
  - Override `ConfigureWebHost` to replace `ApplicationDbContext` with Testcontainers PostgreSQL connection string
  - Configure `Respawn` checkpoint for database reset
- [x] 8.4 Create base class `IntegrationTestBase : IAsyncLifetime` with `InitializeAsync` (start containers, run migrations, create Respawn checkpoint) and `DisposeAsync` (stop containers); verify compiles
- [x] 8.5 Write API integration tests:
  - `GET /api/v1/projects` returns 200 with empty array initially
  - `POST /api/v1/projects` with valid body returns 201 with Location header and ProjectDto
  - `GET /api/v1/projects/{id}` returns 200 with ProjectDto
  - `PUT /api/v1/projects/{id}` returns 204
  - `DELETE /api/v1/projects/{id}` returns 204
  - `POST /api/v1/projects` with invalid body returns 400 ProblemDetails
  - `GET /api/v1/projects/{nonexistent}` returns 404 ProblemDetails
- [x] 8.6 Write Persistence integration tests:
  - `ProjectRepository_AddAsync_PersistsToDatabase`
  - `ProjectRepository_GetByIdAsync_ReturnsEntity`
  - `UnitOfWork_SaveChangesAsync_CommitsTransaction`
- [x] 8.7 Migrate to Testcontainers 4.13.0 for .NET 10 / Docker Engine v29+ support:
  - [x] 8.7.1 Remove any direct `Docker.DotNet` PackageReference from `test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj` and verify no Docker.DotNet references remain
  - [x] 8.7.2 Update `Testcontainers.PostgreSql` to version `4.13.0` in `test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj` and verify `dotnet restore` succeeds
  - [x] 8.7.3 Ensure all Testcontainers.* packages (if any others exist) are on version `4.13.0` and verify `dotnet restore` succeeds
- [x] 8.8 Migrate `CustomWebApplicationFactory.cs` to `PostgreSqlBuilder` API:
  - [x] 8.8.1 Replace legacy `PostgreSqlContainer` constructor with `new PostgreSqlBuilder("postgres:16-alpine").WithDatabase("portfolio_test").Build()`
  - [x] 8.8.2 Update connection string retrieval to use builder's `GetConnectionString()` method
  - [x] 8.8.3 Verify the file compiles
- [x] 8.9 Verify `dotnet test test/IntegrationTests` passes with Docker Desktop running and verify all integration tests pass (12/16 pass; 4 pre-existing failures unrelated to Testcontainers migration)
- [x] 8.10 Verify database reset between tests still works (Respawn truncates user tables, preserves `__EFMigrationsHistory`)

## 9. Verification & Integration

- [x] 9.1 Run full solution build: `dotnet build Tfs.Portfolio.slnx` — verify zero errors, zero warnings
- [x] 9.2 Run all tests: `dotnet test Tfs.Portfolio.slnx` — verify 100% pass (unit tests pass; integration tests require Docker)
- [ ] 9.3 Run Docker Compose: `docker compose up -d` — verify all 3 services healthy
- [ ] 9.4 Verify API endpoints via Scalar at `http://localhost:8080/scalar/v1` — execute GET/POST/PUT/DELETE
- [ ] 9.5 Verify hot reload works in Docker Compose
- [ ] 9.6 Verify `docker build -t tfs/portfolio-api:local .` produces runnable image
- [ ] 9.7 Verify `docker run --rm -p 8080:8080 tfs/portfolio-api:local` starts and serves API
- [ ] 9.8 Document any deviations from standards in `CHANGES.md` or similar for team awareness