# Spec Delta

## Purpose

Defines the API layer (Presentation/WebAPI) exposing HTTP endpoints: ASP.NET Core Web API with Autofac DI, Serilog logging, Scalar/OpenAPI documentation, middleware pipeline, and health checks—implementing the REST contract.

## ADDED Requirements

### Requirement: API project references Application, Persistence, and presentation packages only
The system SHALL ensure `Tfs.Portfolio.Api` references `Tfs.Portfolio.Application`, `Tfs.Portfolio.Infrastructure.Persistence`, `Autofac.Extensions.DependencyInjection`, `Serilog.AspNetCore`, `Scalar.AspNetCore`, `Microsoft.AspNetCore.OpenApi`, `Newtonsoft.Json`—no Domain direct reference (via Application).

#### Scenario: API project references are correct
- **WHEN** inspecting `Tfs.Portfolio.Api.csproj`
- **THEN** references Application, Persistence, Autofac, Serilog, Scalar, OpenAPI, Newtonsoft.Json

### Requirement: Program.cs configures Autofac as DI container
The system SHALL use `Host.CreateApplicationBuilder().UseServiceProviderFactory(new AutofacServiceProviderFactory())` and register modules in `ConfigureContainer`.

#### Scenario: Autofac resolves dependencies
- **WHEN** a controller requests `IMediator` or `IProjectRepository`
- **THEN** Autofac provides the registered implementation from Application/Persistence modules

### Requirement: Serilog is configured for structured logging
The system SHALL configure Serilog in `Program.cs` with Console, File, and Seq sinks; enrich with `FromLogContext`, `WithMachineName`, `WithThreadId`, `WithProcessId`; enable `Serilog.RequestLogging`.

#### Scenario: Logs are structured and enriched
- **WHEN** an HTTP request is processed
- **THEN** logs include correlation ID, machine name, thread ID, request path, status code, duration

### Requirement: Scalar/OpenAPI provides API documentation
The system SHALL enable `app.MapOpenApi()` and `app.MapScalarApiReference()` for interactive API docs at `/scalar/v1`.

#### Scenario: API docs are accessible
- **WHEN** navigating to `/scalar/v1` in browser
- **THEN** Scalar UI loads with all endpoints documented

### Requirement: Endpoints follow REST conventions at /api/v1/
The system SHALL map endpoints under `MapGroup("api/v1")` with plural, lowercase, kebab-case resource names (e.g., `/api/v1/projects`, `/api/v1/project-tasks`).

#### Scenario: Endpoint URLs follow convention
- **WHEN** listing all endpoints
- **THEN** all are under `/api/v1/` with plural kebab-case paths

### Requirement: Endpoints return ProblemDetails for errors
The system SHALL return RFC 7807 `ProblemDetails` for all error responses (400, 404, 500) with `type`, `title`, `status`, `detail`, `instance`.

#### Scenario: Validation error returns ProblemDetails
- **WHEN** POST `/api/v1/projects` with invalid body
- **THEN** response is 400 with `ProblemDetails` containing validation errors

### Requirement: Middleware pipeline includes standard concerns
The system SHALL configure middleware in order: Exception handling → Serilog Request Logging → HTTPS Redirection → Routing → Authorization → Endpoints.

#### Scenario: Exception middleware catches unhandled errors
- **WHEN** an unhandled exception occurs in a controller
- **THEN** it returns 500 ProblemDetails without stack trace in production

### Requirement: Health checks expose liveness and readiness
The system SHALL add `app.MapHealthChecks("/health/live")` and `app.MapHealthChecks("/health/ready")` with DB check on ready.

#### Scenario: Readiness check verifies database
- **WHEN** calling GET `/health/ready`
- **THEN** returns 200 if DB reachable, 503 if not

### Requirement: API versioning uses route-based MapGroup
The system SHALL use `MapGroup("api/v1")` for versioning; future versions use `MapGroup("api/v2")`.

#### Scenario: Version is in route
- **WHEN** calling GET `/api/v1/projects`
- **THEN** it routes to v1 handler; v2 would be separate group

### Requirement: JSON serialization uses Newtonsoft.Json
The system SHALL configure `AddControllers().AddNewtonsoftJson()` for all API serialization with camelCase contract resolver.

#### Scenario: JSON response uses camelCase
- **WHEN** GET `/api/v1/projects` returns data
- **THEN** property names are `projectId`, `projectName`, `createdAt` (camelCase)

### Requirement: API layer compiles independently
The system SHALL ensure `dotnet build` on `Tfs.Portfolio.Api` succeeds with Application and Persistence references.

#### Scenario: API builds with dependencies
- **WHEN** running `dotnet build src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj`
- **THEN** build succeeds with zero errors