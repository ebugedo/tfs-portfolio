# Spec Delta

## MODIFIED Requirements

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

## ADDED Requirements

### Requirement: Client endpoints
The system SHALL provide endpoints under `/api/v1/clients`:
- GET /clients → returns 200 with IReadOnlyList<ClientListItemDto>
- GET /clients/{id:guid} → returns 200 with ClientDto or 404 ProblemDetails
- POST /clients → returns 201 with ClientDto and Location header, or 400 ProblemDetails
- PUT /clients/{id:guid} → returns 204 or 400/404 ProblemDetails
- DELETE /clients/{id:guid} → returns 204 or 404 ProblemDetails

#### Scenario: GET /clients returns paginated list
- **WHEN** GET /api/v1/clients is called
- **THEN** returns 200 with array of ClientListItemDto (id, name, email, isActive, createdAt)

#### Scenario: POST /clients with invalid data returns 400
- **WHEN** POST /api/v1/clients with empty name
- **THEN** returns 400 ProblemDetails with validation errors

### Requirement: Sector endpoints
The system SHALL provide endpoints under `/api/v1/sectors`:
- GET /sectors → returns 200 with IReadOnlyList<SectorListItemDto>
- GET /sectors/{id:guid} → returns 200 with SectorDto or 404 ProblemDetails
- POST /sectors → returns 201 with SectorDto and Location header, or 400 ProblemDetails
- PUT /sectors/{id:guid} → returns 204 or 400/404 ProblemDetails
- DELETE /sectors/{id:guid} → returns 204 or 404 ProblemDetails

### Requirement: Service endpoints
The system SHALL provide endpoints under `/api/v1/services`:
- GET /services → returns 200 with IReadOnlyList<ServiceListItemDto>
- GET /services/{id:guid} → returns 200 with ServiceDto or 404 ProblemDetails
- POST /services → returns 201 with ServiceDto and Location header, or 400 ProblemDetails
- PUT /services/{id:guid} → returns 204 or 400/404 ProblemDetails
- DELETE /services/{id:guid} → returns 204 or 404 ProblemDetails
- GET /services/category/{category} → returns 200 with filtered services

### Requirement: CompanyProfile endpoint (singleton)
The system SHALL provide endpoints under `/api/v1/company-profile`:
- GET /company-profile → returns 200 with CompanyProfileDto or 404 if not created
- PUT /company-profile → returns 200 with updated CompanyProfileDto or 400 ProblemDetails

#### Scenario: GET /company-profile returns singleton
- **WHEN** GET /api/v1/company-profile is called after creation
- **THEN** returns 200 with CompanyProfileDto

#### Scenario: PUT /company-profile updates singleton
- **WHEN** PUT /api/v1/company-profile with valid data
- **THEN** returns 200 with updated CompanyProfileDto

### Requirement: Enhanced Project endpoints with filters
The system SHALL extend `/api/v1/projects` with query parameters:
- GET /projects?clientId={guid}&sectorId={guid}&technology={string}&status={enum} → filtered results
- POST /projects accepts StartDate, DurationMonths, ClientId, SectorId, Technologies
- PUT /projects/{id} accepts Status change
- PATCH /projects/{id}/status → ChangeProjectStatusCommand (alternative to PUT for status)
- POST /projects/{id}/services/{serviceId} → associates service (204)
- DELETE /projects/{id}/services/{serviceId} → removes association (204)

#### Scenario: GET /projects filters by client and status
- **WHEN** GET /api/v1/projects?clientId=xxx&status=Active
- **THEN** returns 200 with projects matching both filters

#### Scenario: POST /projects validates relationships
- **WHEN** POST /api/v1/projects with non-existent ClientId
- **THEN** returns 400 ProblemDetails with "Client not found"

#### Scenario: Project-service association endpoints
- **WHEN** POST /api/v1/projects/{id}/services/{serviceId}
- **THEN** returns 204 if successful, 404 if project or service not found

### Requirement: All new endpoints registered in ApplicationModule and Program.cs
The system SHALL register ClientEndpoints, SectorEndpoints, ServiceEndpoints, CompanyProfileEndpoints, and extended ProjectEndpoints in the v1 MapGroup via extension methods.

#### Scenario: All endpoints documented in Scalar
- **WHEN** navigating to /scalar/v1
- **THEN** all new endpoints visible with request/response schemas