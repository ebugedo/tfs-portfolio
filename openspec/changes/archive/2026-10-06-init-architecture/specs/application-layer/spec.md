# Spec Delta

## Purpose

Defines the Application layer containing business use cases implemented as CQRS commands/queries, DTOs, AutoMapper profiles, validation, and application service interfaces—orchestrating domain logic without infrastructure concerns.

## ADDED Requirements

### Requirement: Application project references only Domain and standard libraries
The system SHALL ensure `Tfs.Portfolio.Application` references `Tfs.Portfolio.Domain`, `AutoMapper`, `FluentValidation`, and .NET standard libraries—no EF Core, no ASP.NET Core, no infrastructure.

#### Scenario: Application project references are clean
- **WHEN** inspecting `Tfs.Portfolio.Application.csproj`
- **THEN** no PackageReference to EntityFrameworkCore, AspNetCore, or infrastructure packages

### Requirement: Commands represent state-changing operations
The system SHALL define Commands as `record` types implementing `ICommand` or `ICommand<TResult>` for write operations, named `Create<Entity>Command`, `Update<Entity>Command`, `Delete<Entity>Command`.

#### Scenario: Command has correct structure
- **WHEN** inspecting `CreateProjectCommand`
- **THEN** it is a record with input properties and implements `ICommand<Guid>` (returns new entity ID)

### Requirement: Queries represent read operations
The system SHALL define Queries as `record` types implementing `IQuery<TResult>` for read operations, named `Get<Entity>ByIdQuery`, `Get<Entity>ListQuery`, `Search<Entity>Query`.

#### Scenario: Query has correct structure
- **WHEN** inspecting `GetProjectByIdQuery`
- **THEN** it is a record with `Guid Id` property and implements `IQuery<ProjectDto>`

### Requirement: Handlers implement business use cases
The system SHALL provide handlers for each Command/Query that orchestrate domain logic: load aggregates, invoke methods, persist via repository interfaces, return DTOs.

#### Scenario: Command handler executes use case
- **WHEN** `CreateProjectCommandHandler.Handle` is invoked
- **THEN** it creates a `Project` aggregate, calls `projectRepository.AddAsync()`, saves via `IUnitOfWork`, returns `ProjectDto`

### Requirement: DTOs are defined for API contracts
The system SHALL define Data Transfer Objects (DTOs) as `record` types in `Tfs.Portfolio.Application.<Aggregate>.Dtos` for requests and responses.

#### Scenario: DTO matches API contract
- **WHEN** inspecting `CreateProjectRequest` and `ProjectDto`
- **THEN** they use camelCase property names, contain only serializable data

### Requirement: AutoMapper profiles map between layers
The system SHALL provide `AutoMapper.Profile` classes mapping Domain entities ↔ Application DTOs, registered in the Application layer.

#### Scenario: Mapping works both ways
- **WHEN** `mapper.Map<ProjectDto>(projectEntity)` is called
- **THEN** all properties map correctly; reverse mapping works for create/update

### Requirement: Validation uses FluentValidation
The system SHALL validate Commands and Queries using FluentValidation validators (`AbstractValidator<TCommand>`), registered in the Application layer.

#### Scenario: Invalid command fails validation
- **WHEN** `CreateProjectCommand` with empty Name is validated
- **THEN** validation fails with "Name is required" error

### Requirement: Application service interfaces are defined
The system SHALL define interfaces for cross-cutting application services (e.g., `ICurrentUserService`, `IDateTimeProvider`) in Application layer.

#### Scenario: Application service interface is implemented in Infrastructure
- **WHEN** `ICurrentUserService` is injected into a handler
- **THEN** it resolves to Infrastructure implementation at runtime

### Requirement: Application layer compiles independently
The system SHALL ensure `dotnet build` on `Tfs.Portfolio.Application` succeeds with only Domain as project reference.

#### Scenario: Application builds with Domain only
- **WHEN** running `dotnet build src/Core/Application/Tfs.Portfolio.Application.csproj`
- **THEN** build succeeds with zero errors