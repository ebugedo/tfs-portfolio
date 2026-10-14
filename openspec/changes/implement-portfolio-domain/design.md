# Design

## Context

See `proposal.md` for motivation. This change implements the complete portfolio domain for a software consulting company on top of the existing Clean Architecture + DDD + CQRS foundation. The current codebase has only a reference `Project` aggregate; this design extends it with `Client`, `Sector`, `Service`, `CompanyProfile` aggregates, shared value objects, and full CRUD APIs.

**Existing foundation to leverage:**
- Domain layer: `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `IDomainEvent`, `DomainException`, `IUnitOfWork`, `IProjectRepository`
- Application layer: CQRS interfaces (`ICommand`, `IQuery<T>`, handlers), `ApplicationModule` (Autofac), AutoMapper profiles, FluentValidation
- Persistence layer: `ApplicationDbContext`, `ProjectConfiguration`, `ProjectRepository`, `UnitOfWork`, `PersistenceModule`, initial migration
- API layer: Minimal APIs with `MapGroup("/api/v1")`, `ValidationFilter`, Serilog, Scalar, ProblemDetails, health checks
- Tests: UnitTests (xUnit + Moq + Bogus), IntegrationTests (Testcontainers + WebApplicationFactory + Respawn)

*Global standards (tech stack, conventions, patterns) are defined in `openspec/standars/` and apply implicitly.*

## Goals / Non-Goals

**Goals:**
- Implement 4 new aggregates (Client, Sector, Service, CompanyProfile) and enhance Project with timeline, technologies, relationships, status workflow
- Add 4 shared value objects (YearMonth, Technology, Url, ContactInfo) in Domain.Common
- Implement full CQRS for all aggregates: commands, queries, handlers, DTOs, validators, mappers
- Configure EF Core persistence with Fluent API: new tables, JSONB for technologies, FKs, join table for Project-Service
- Expose RESTful Minimal API endpoints under `/api/v1/` for all aggregates with filtering
- Achieve 100% test coverage for new domain logic and API endpoints

**Non-Goals:**
- Authentication/authorization implementation (only DI registration points per existing `ICurrentUserService`)
- API versioning beyond v1 route structure
- Soft delete implementation (entities use IsActive boolean instead)
- Background jobs, event publishing infrastructure, or distributed tracing
- Frontend/UI components

## Decisions

### 1. Value Objects Location: `Domain.Common.ValueObjects`
**Decision:** Place shared value objects (`YearMonth`, `Technology`, `Url`, `ContactInfo`) in `Tfs.Portfolio.Domain.Common.ValueObjects` namespace.

**Rationale:** These are used across multiple aggregates (Project, Client, CompanyProfile, Service). Keeping them in Domain.Common avoids circular dependencies and follows DDD pattern of shared kernel.

**Alternative considered:** Duplicate in each aggregate's namespace - rejected due to duplication and inconsistency risk.

### 2. Technology Storage: JSONB in Projects Table
**Decision:** Store `Technologies` collection as `jsonb` column in Projects table using EF Core owned entity or value converter.

**Rationale:** Technologies are aggregate-internal, not independently queried across projects. JSONB provides flexibility for schema evolution, avoids join table overhead, and PostgreSQL jsonb indexing supports querying by technology name if needed later.

**Alternative considered:** Separate `ProjectTechnology` table with many-to-many - rejected as over-engineering for value objects that don't have independent identity.

### 3. Project-Service Relationship: Explicit Join Table
**Decision:** Create `ProjectServices` join table with composite PK (ProjectId, ServiceId) for the many-to-many relationship.

**Rationale:** Services are independent aggregates with their own lifecycle and category. A join table correctly models the many-to-many and allows future payload (e.g., `AssignedAt`, `Notes`) without schema changes.

**Alternative considered:** Store ServiceIds as JSONB in Projects - rejected because Services are aggregates, not value objects, and referential integrity matters.

### 4. CompanyProfile Singleton Enforcement: Database Constraint + Domain Guard
**Decision:** Enforce singleton at both domain level (factory method throws if exists) and database level (unique constraint or check constraint on CompanyProfiles table).

**Rationale:** Defense in depth. Domain guard provides clear exception message; DB constraint prevents race conditions in concurrent deployments.

**Alternative considered:** Only domain guard - rejected due to potential race conditions. Only DB constraint - rejected due to poor error messaging.

### 5. Project Status Workflow: Domain-Enforced Transitions
**Decision:** Implement status transitions in `Project.ChangeStatus(ProjectStatus newStatus)` method with explicit validation rules.

**Rationale:** Keeps business rules in the aggregate, prevents invalid state transitions, raises `ProjectStatusChangedEvent` with old/new status for audit trail.

**Alternative considered:** Allow any transition, validate in application layer - rejected as it leaks domain logic out of the aggregate.

### 6. YearMonth Value Object: Month/Year Only (No Day)
**Decision:** `YearMonth` stores only month (1-12) and year (int). Provides `AddMonths(int)` for end date calculation.

**Rationale:** Project start dates are specified as month/year only per requirements. Avoids day-related ambiguity and timezone issues.

**Alternative considered:** Use `DateTime` with day=1 - rejected as it implies precision that doesn't exist in the domain.

### 7. Url Value Object: Nullable Support
**Decision:** `Url` supports null/empty via `IsValid` property, allowing optional logo/web URLs.

**Rationale:** LogoUrl and WebUrl are optional per requirements. Nullable Url avoids null checks throughout code while maintaining validation.

**Alternative considered:** Use `string?` with validation in commands - rejected as it pushes validation out of the value object.

### 8. API Filtering: Query Parameters on GET Endpoints
**Decision:** Implement filtering via query parameters on `GET /api/v1/projects` (e.g., `?clientId=xxx&status=Active`).

**Rationale:** RESTful, cacheable, follows existing pattern. Simple to implement and document in OpenAPI/Scalar.

**Alternative considered:** Separate filter endpoints or POST with filter body - rejected as less RESTful and not cacheable.

### 9. Migration Strategy: Single Additive Migration
**Decision:** Generate one migration (`AddPortfolioDomainEntities`) creating all new tables, columns, FKs, indexes, and join table.

**Rationale:** Atomic schema change, easier to review and rollback. All entities are interrelated (FKs), so single migration avoids ordering issues.

**Alternative considered:** Separate migrations per aggregate - rejected due to FK dependencies between aggregates.

### 10. Test Data Generation: Bogus Fakers for All New Types
**Decision:** Create `Faker<T>` classes for all new aggregates, value objects, commands, and DTOs in UnitTests.Common.

**Rationale:** Consistent test data generation, reduces boilerplate, enables property-based testing patterns.

## Risks / Trade-offs

| Risk | Mitigation |
|------|------------|
| **JSONB Technologies query performance** | Add GIN index on Technologies column if filtering by technology becomes frequent |
| **Project-Service join table growth** | Monitor row count; archive old associations if needed |
| **Singleton CompanyProfile race condition** | DB constraint + domain guard; accept rare `DbUpdateException` on concurrent create |
| **Status transition complexity** | Document transition matrix in code comments; add integration tests for all valid/invalid transitions |
| **Value object equality edge cases** | Comprehensive unit tests for YearMonth, Technology, Url, ContactInfo equality and validation |
| **Migration rollback complexity** | Test migration up/down on staging; keep migration pure additive (no data transformation) |
| **API endpoint proliferation** | Group related endpoints under feature folders; use consistent naming conventions |

## Migration Plan

1. **Generate migration**: `dotnet ef migrations add AddPortfolioDomainEntities -p src/Infrastructure/Persistence -s src/Presentation/WebAPI --output-dir Migrations`
2. **Review migration**: Verify all tables, columns, FKs, indexes, JSONB types, join table
3. **Test migration locally**: `docker compose up -d` → `dotnet ef database update` → verify schema in pgAdmin
4. **Run tests**: `dotnet test` (unit + integration) to verify all new logic
5. **Deploy**: Migration runs automatically on API startup in Development (existing `context.Database.Migrate()`)
6. **Production**: Run migration manually via CI/CD pipeline before deploying new API version

**Rollback**: `dotnet ef database update <previous-migration>` - pure additive migration so rollback is safe (drops new tables/columns).

## Open Questions

1. **Soft delete vs IsActive**: Current design uses `IsActive` boolean. Should we implement soft delete (IsDeleted + query filter) for audit trail requirements?
2. **Technology categorization extensibility**: Category enum is fixed. Should we make it a lookup table for user-defined categories?
3. **Project timeline precision**: `DurationMonths` is integer. Do we need partial months (e.g., 3.5 months)?
4. **CompanyProfile multilingual support**: Description/WebUrl might need localization. Defer to future i18n work.

## Known Build Issues (Pre-existing)

The following StyleCop and code analysis issues were discovered during implementation. These are pre-existing code quality issues unrelated to the FK constraint fix and need to be addressed in a separate refactoring effort:

| Issue | Location | Count | Fix |
|-------|----------|-------|-----|
| SA1137 (indentation) | `ProjectsApiTests.cs` | 50+ | Fix indentation consistency |
| SA1028 (trailing whitespace) | Multiple test files | 15+ | Remove trailing whitespace |
| SA1202 (member ordering) | `ProjectRepositoryTests.cs`, `ExtendedProjectRepositoryTests.cs` | 2 | Move public methods before private helpers |
| CS0108 (hiding members) | `ProjectRepositoryTests`, `ExtendedProjectRepositoryTests`, `ProjectsApiTests` | 4 | Add `new` keyword or rename |
| CA1823 (unused fields) | `ProjectsApiTests.cs` | 3 | Remove unused `ClientsBaseUrl`, `SectorsBaseUrl`, `ServicesBaseUrl` |
| CA1002 (List<T> parameter) | `IntegrationTestBase.CreateTestProjectAsync` | 1 | Use `IReadOnlyList<T>` or `IEnumerable<T>` |
| CA1823 (unused fields) | Various | Multiple | Remove unused fields |
| SA1611 (missing param docs) | `IntegrationTestFakers.cs` | 2 | Add XML documentation |
| CS0103 (undefined variable) | `ExtendedProjectRepositoryTests.cs` lines 378-380 | 1 | Fix undefined `projects` variable |
| CS1503 (argument type mismatch) | `ProjectsApiTests.cs` | 6 | Fix `ServiceCategory` enum usage |

## ClientsApiTests Fixes (New)

The following fixes are required to resolve the failing ClientsApiTests integration tests:

### 1. TfsWebUrl JSON Serialization - Null Values
**Issue**: Null `TfsWebUrl?` values serialize as `{"value":null,"isValid":false}` instead of `null`.
**Location**: `TfsWebUrlJsonConverter.WriteJson`
**Root Cause**: Converter receives `TfsWebUrl` with `Value=null` (not null reference) and writes object instead of null.
**Fix**: Modify `WriteJson` to check `url.Value == null` and write null.
**Status**: In progress - converter updated but test still failing.

### 2. TfsWebUrl Null Deserialization
**Issue**: Null JSON values deserialize to `TfsWebUrl{Value=null, IsValid=false}` instead of `null`.
**Location**: `TfsWebUrlJsonConverter.ReadJson`
**Root Cause**: `ReadJson` returns `default(TfsWebUrl)` for null input instead of `null`.
**Fix**: Return `null` for null/empty input when target type is `TfsWebUrl?`.
**Status**: In progress - converter updated but test still failing.

### 3. Missing DeactivateClientCommandHandler Registration
**Issue**: `DeactivateClientCommandHandler` not registered in Autofac container.
**Location**: `src/Core/Application/Common/Modules/ApplicationModule.cs`
**Fix**: Register handler in `ApplicationModule.Load()`.
**Status**: Not started.

### 4. LogoUrl Null Serialization in ClientDto
**Issue**: Null `LogoUrl` serializes as `{"value":null,"isValid":false}` instead of `null`.
**Location**: `ClientDto.LogoUrl`, `ClientListItemDto.LogoUrl`, `CompanyProfileDto.TfsWebUrl`, `CompanyProfileDto.LogoUrl`
**Fix**: Ensure `TfsWebUrlJsonConverter` handles null correctly; verify `[JsonConverter]` attributes on DTO properties.
**Status**: In progress - attributes added to DTOs, global converter registered.

### 5. Missing DeactivateClientCommandHandler Registration in Autofac
**Issue**: `DeactivateClientCommandHandler` not registered in Autofac container.
**Location**: `src/Core/Application/Common/Modules/ApplicationModule.cs`
**Fix**: Add `builder.RegisterType<DeactivateClientCommandHandler>().As<IDeactivateClientCommandHandler>();` (or similar).
**Status**: Not started.

**Mitigation**: These issues block ClientsApiTests (37 failing). They will be addressed in Section 27 tasks.