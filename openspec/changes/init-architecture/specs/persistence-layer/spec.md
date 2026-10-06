# Spec Delta

## Purpose

Defines the Persistence layer implementing data access with Entity Framework Core: DbContext, Fluent API configurations, repositories, UnitOfWork, and migrations—isolating database concerns from Domain and Application layers.

## ADDED Requirements

### Requirement: Persistence project references Domain, Application, and EF Core only
The system SHALL ensure `Tfs.Portfolio.Infrastructure.Persistence` references `Tfs.Portfolio.Domain`, `Tfs.Portfolio.Application`, `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`—no ASP.NET Core.

#### Scenario: Persistence project references are correct
- **WHEN** inspecting `Tfs.Portfolio.Infrastructure.Persistence.csproj`
- **THEN** references Domain, Application, EF Core, Npgsql—no AspNetCore

### Requirement: DbContext is defined with DbSets for aggregates
The system SHALL provide `ApplicationDbContext : DbContext` with `DbSet<Project>`, `DbSet<Client>`, etc., for each aggregate root.

#### Scenario: DbContext exposes aggregate sets
- **WHEN** inspecting `ApplicationDbContext`
- **THEN** it has `public DbSet<Project> Projects { get; set; }` for each aggregate

### Requirement: Entity configurations use Fluent API in separate classes
The system SHALL configure each entity using `IEntityTypeConfiguration<T>` in separate classes under `Configurations/`—no Data Annotations on Domain entities.

#### Scenario: Entity configuration is separate from Domain
- **WHEN** inspecting `ProjectConfiguration : IEntityTypeConfiguration<Project>`
- **THEN** it configures table name, primary key, indexes, owned types—Domain entity has no attributes

### Requirement: Repositories implement Domain interfaces
The system SHALL implement repository interfaces (e.g., `ProjectRepository : IProjectRepository`) using EF Core, exposing only Domain types.

#### Scenario: Repository returns Domain entities
- **WHEN** calling `projectRepository.GetByIdAsync(id)`
- **THEN** it returns `Task<Project?>` (Domain entity), not DTO or EF proxy

### Requirement: UnitOfWork coordinates transactions
The system SHALL provide `UnitOfWork : IUnitOfWork` wrapping `DbContext.SaveChangesAsync()` for transaction boundary.

#### Scenario: UnitOfWork commits changes
- **WHEN** handler calls `await unitOfWork.SaveChangesAsync(cancellationToken)`
- **THEN** all tracked changes persist in a single transaction

### Requirement: Migrations are generated and versioned
The system SHALL generate EF Core migrations via `dotnet ef migrations add <Name>` in the Persistence project, committed to source control.

#### Scenario: Migration applies cleanly
- **WHEN** running `dotnet ef database update` on clean database
- **THEN** schema matches Domain model with correct tables, columns, indexes, FKs

### Requirement: Soft delete uses query filters where required
The system SHALL implement soft delete via global query filters (`HasQueryFilter(e => !e.IsDeleted)`) on entities that require it.

#### Scenario: Soft-deleted entities excluded by default
- **WHEN** querying `context.Projects.ToListAsync()`
- **THEN** only non-deleted projects return; deleted ones require explicit `IgnoreQueryFilters()`

### Requirement: PostgreSQL-specific types are used appropriately
The system SHALL use `Npgsql` types (e.g., `uuid`, `jsonb`, `timestamp with time zone`) via Fluent API for optimal PostgreSQL storage.

#### Scenario: Entity uses PostgreSQL-native types
- **WHEN** inspecting migration for `Project` entity
- **THEN** `Id` is `uuid`, `CreatedAt` is `timestamp with time zone`, metadata uses `jsonb`

### Requirement: Persistence layer compiles independently
The system SHALL ensure `dotnet build` on `Tfs.Portfolio.Infrastructure.Persistence` succeeds with Domain and Application references.

#### Scenario: Persistence builds with Domain and Application
- **WHEN** running `dotnet build src/Infrastructure/Persistence/Tfs.Portfolio.Infrastructure.Persistence.csproj`
- **THEN** build succeeds with zero errors