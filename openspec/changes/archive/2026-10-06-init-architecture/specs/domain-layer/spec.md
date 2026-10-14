# Spec Delta

## Purpose

Defines the Domain layer containing enterprise business logic: entities, aggregates, value objects, domain events, domain exceptions, and repository interfaces—completely independent of infrastructure and frameworks.

## ADDED Requirements

### Requirement: Domain project has no external dependencies
The system SHALL ensure `Tfs.Portfolio.Domain` references only .NET standard libraries and `Microsoft.Extensions.DependencyInjection.Abstractions` for repository interfaces—no EF Core, no ASP.NET Core, no MediatR.

#### Scenario: Domain project references are clean
- **WHEN** inspecting `Tfs.Portfolio.Domain.csproj`
- **THEN** no PackageReference to EntityFrameworkCore, AspNetCore, MediatR, or infrastructure packages

### Requirement: Entities and Aggregates use standard identity types
The system SHALL use `Guid` or `long` (identity) for primary keys on all entities and aggregate roots.

#### Scenario: Entity has Guid primary key
- **WHEN** defining a new aggregate root
- **THEN** it has `public Guid Id { get; private set; }` (or `long` for identity)

### Requirement: Value Objects are immutable and implement equality
The system SHALL implement Value Objects as `readonly record struct` or `readonly record class` with value-based equality.

#### Scenario: Value Object equality works correctly
- **WHEN** comparing two Value Objects with same values
- **THEN** they are equal; with different values, they are not equal

### Requirement: Domain Events capture business occurrences
The system SHALL define Domain Events as `record` types implementing `IDomainEvent` (marker interface) raised by aggregates.

#### Scenario: Aggregate raises domain event
- **WHEN** an aggregate performs a significant business action
- **THEN** it calls `AddDomainEvent(new OrderCreatedEvent(...))` and events are retrievable via `GetDomainEvents()`

### Requirement: Domain Exceptions represent business rule violations
The system SHALL provide a `DomainException` base class and specific exceptions (e.g., `ProjectNotFoundException`, `InvalidProjectStateException`) for business rule failures.

#### Scenario: Domain exception is thrown for rule violation
- **WHEN** an aggregate method validates a business rule that fails
- **THEN** it throws a specific `DomainException` derivative with a clear message

### Requirement: Repository interfaces are defined in Domain
The system SHALL define repository interfaces (e.g., `IProjectRepository`, `IClientRepository`) in the Domain layer with only Domain types in signatures.

#### Scenario: Repository interface uses only Domain types
- **WHEN** inspecting `IProjectRepository`
- **THEN** methods return `Task<Project?>`, `Task<IReadOnlyList<Project>>`, etc.—no EF Core or DTO types

### Requirement: Aggregates enforce invariants internally
The system SHALL ensure aggregates expose only methods that maintain consistency; no public setters on entity properties.

#### Scenario: Aggregate state cannot be corrupted externally
- **WHEN** attempting to set an entity property directly
- **THEN** it fails to compile (private/init setters only) or is not exposed

### Requirement: Domain layer compiles independently
The system SHALL ensure `dotnet build` on `Tfs.Portfolio.Domain` succeeds without other projects.

#### Scenario: Domain builds in isolation
- **WHEN** running `dotnet build src/Core/Domain/Tfs.Portfolio.Domain.csproj`
- **THEN** build succeeds with zero errors