# Changes and Deviations from Standards

This document records known deviations from the OpenSpec standards and architectural decisions made during the initial architecture implementation (`init-architecture` change).

## Known Deviations

### 1. API Port Configuration (Containerization)
**Standard**: `docker-compose.yml` should expose API on port 8080
**Deviation**: API service uses port 8082 on host (`"8082:8080"`) due to port conflict with Apache HTTP server running on host port 8080
**Location**: `docker-compose.yml:7`
**Impact**: Developers must use `http://localhost:8082` instead of `http://localhost:8080` for local development
**Resolution**: Document port in README; consider changing default port in future

### 2. DELETE Endpoint Behavior (API Layer)
**Standard**: DELETE `/api/v1/projects/{id}` should return 204 and actually remove the resource
**Deviation**: Returns 204 but does not delete the entity from database
**Root Cause**: `DeleteProjectCommandHandler` uses `project with { }` (record copy) instead of calling a repository Delete method
**Location**: `src/Core/Application/Projects/Handlers/ProjectCommandHandlers.cs:117`
**Files to Fix**:
- `src/Core/Domain/Projects/Repositories/IProjectRepository.cs` - Add `DeleteAsync(Guid id)` method
- `src/Infrastructure/Persistence/Repositories/ProjectRepository.cs` - Implement `DeleteAsync`
- `src/Core/Application/Projects/Handlers/ProjectCommandHandlers.cs` - Call repository.DeleteAsync

### 3. 404 Response Format (API Layer)
**Standard**: All error responses (400, 404, 500) must return RFC 7807 ProblemDetails
**Deviation**: GET `/api/v1/projects/{id}` with non-existent ID returns 404 with empty body instead of ProblemDetails
**Root Cause**: Endpoint uses `Results.NotFound()` which returns empty 404
**Location**: `src/Presentation/WebAPI/Program.cs:176`
**Fix**: Change to `Results.Problem(statusCode: 404, title: "Not Found", detail: "Project not found", instance: $"/api/v1/projects/{id}")`

### 4. Soft Delete Not Implemented (Persistence Layer)
**Standard**: Entities requiring soft delete should use global query filter `HasQueryFilter(e => !e.IsDeleted)`
**Deviation**: `Project` entity has no `IsDeleted` property; soft delete not configured
**Location**: `src/Core/Domain/Projects/Entities/Project.cs`, `src/Infrastructure/Persistence/Configurations/ProjectConfiguration.cs`
**Status**: Deferred - will be implemented when first feature requires it (per design decision)

### 5. Seq Authentication Configuration (Containerization)
**Standard**: Seq should be configured via `.env.development` (gitignored) with `.env.example` as template
**Deviation**: `SEQ_FIRSTRUN_NOAUTHENTICATION=true` is set in `docker-compose.yml` but not documented in `.env.example`
**Location**: `docker-compose.yml:48`, `.env.example`
**Fix**: Add `SEQ_FIRSTRUN_NOAUTHENTICATION=true` to `.env.example`

### 6. Testcontainers.Seq Reference (Testing Foundation)
**Standard**: Proposal mentions `Testcontainers.Seq` for integration tests
**Deviation**: `test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj` does not reference `Testcontainers.Seq`; Seq is only used in Docker Compose for local development
**Impact**: Integration tests don't test Seq logging; Seq logging verified manually via local compose

### 7. Entity Record Type (Domain Layer)
**Standard**: Entities use `abstract record Entity<TId>` with `protected init` Id
**Deviation**: `Project` is a `sealed record` inheriting from `AggregateRoot<Guid>` which inherits from `Entity<Guid>`
**Note**: This follows the chosen pattern (AggregateRoot as record) but differs from some DDD implementations that use classes for entities. Documented as intentional design choice.

## Pre-existing Test Failures (Not Blocking)

The following integration test failures are pre-existing and not related to the Testcontainers 4.x migration:

| Test | Issue |
|------|-------|
| `ProjectRepositoryTests.Update_UpdatesExistingProject` | Entity tracking conflict in `ProjectRepository.Update` |
| `ProjectsApiTests.CreateProject_WithValidBody_ReturnsCreatedWithLocationAndDto` | Response body has null Name |
| `ProjectsApiTests.GetProjectById_WhenNotFound_ReturnsNotFoundWithProblemDetails` | Empty 404 body (see deviation #3) |
| `ProjectsApiTests.DeleteProject_WhenExists_ReturnsNoContent` | DELETE doesn't actually delete (see deviation #2) |

## Future Work

1. Fix DELETE endpoint to actually delete (add repository Delete method)
2. Fix 404 to return ProblemDetails
3. Add soft delete support when needed
4. Update `.env.example` with Seq configuration
5. Consider adding `Testcontainers.Seq` to integration tests if Seq logging needs automated verification

---

*Generated as part of task 9.8 in `init-architecture` change*