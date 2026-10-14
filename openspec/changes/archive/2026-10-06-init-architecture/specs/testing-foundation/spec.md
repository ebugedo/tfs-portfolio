# Spec Delta

## Purpose

Defines the testing foundation: unit test project (xUnit + Moq + Bogus) for Domain/Application logic, and integration test project (Testcontainers + WebApplicationFactory) for API and database behavior.

## ADDED Requirements

### Requirement: Unit test project references Domain and Application only
The system SHALL ensure `Tfs.Portfolio.UnitTests` references `Tfs.Portfolio.Domain`, `Tfs.Portfolio.Application`, `xunit`, `Moq`, `Bogus`, `FluentAssertions`, `AutoFixture`—no EF Core, no ASP.NET Core.

#### Scenario: Unit test project references are clean
- **WHEN** inspecting `Tfs.Portfolio.UnitTests.csproj`
- **THEN** references Domain, Application, xUnit, Moq, Bogus, FluentAssertions—no infrastructure

### Requirement: Unit tests cover Domain aggregates and value objects
The system SHALL provide tests for aggregate invariants, value object equality, domain event raising, and domain exception throwing.

#### Scenario: Aggregate invariant test passes
- **WHEN** running `CreateProject_WithValidName_RaisesProjectCreatedEvent`
- **THEN** test verifies aggregate state and domain event

### Requirement: Unit tests cover Application handlers
The system SHALL provide tests for Command/Query handlers using Moq to mock repository interfaces and IUnitOfWork.

#### Scenario: Command handler test verifies repository interaction
- **WHEN** running `CreateProjectCommandHandler_WithValidCommand_CallsRepositoryAndReturnsDto`
- **THEN** test verifies `AddAsync` called once, `SaveChangesAsync` called once, correct DTO returned

### Requirement: Bogus generates test data
The system SHALL use Bogus `Faker<T>` to generate realistic test entities, commands, and DTOs.

#### Scenario: Faker produces valid entities
- **WHEN** `new ProjectFaker().Generate()` is called
- **THEN** returns `Project` with valid Name, Description, random Guid Id

### Requirement: Integration test project uses Testcontainers 4.13.0
The system SHALL ensure `Tfs.Portfolio.IntegrationTests` references `Testcontainers.PostgreSql` version 4.13.0, `Microsoft.AspNetCore.Mvc.Testing`, `Respawn`, `xunit`.

#### Scenario: Integration test project references are correct
- **WHEN** inspecting `Tfs.Portfolio.IntegrationTests.csproj`
- **THEN** references `Testcontainers.PostgreSql` 4.13.0, WebApplicationFactory, Respawn, xUnit
- **AND** no direct reference to `Docker.DotNet` exists (managed transitively by Testcontainers)

### Requirement: Testcontainers uses PostgreSqlBuilder API
The system SHALL use `PostgreSqlBuilder("postgres:16-alpine")` with `.WithDatabase("portfolio_test")` and `.Build()` for container configuration, compatible with Testcontainers 4.10+.

#### Scenario: Container configured with builder pattern
- **WHEN** `CustomWebApplicationFactory` initializes the test container
- **THEN** uses `new PostgreSqlBuilder("postgres:16-alpine").WithDatabase("portfolio_test").Build()`
- **AND** retrieves connection string via builder's `GetConnectionString()` method

### Requirement: WebApplicationFactory configures test host
The system SHALL provide `CustomWebApplicationFactory : WebApplicationFactory<Program>` that replaces DbContext with Testcontainers PostgreSQL and configures test services.

#### Scenario: Test host uses real PostgreSQL in container
- **WHEN** integration test sends HTTP request
- **THEN** it hits real API pipeline with real database in Testcontainer

### Requirement: Respawn resets database between tests
The system SHALL use `Respawn` to truncate all tables (except migrations history) before each test for isolation.

#### Scenario: Database is clean for each test
- **WHEN** running two integration tests sequentially
- **THEN** second test sees no data from first test

### Requirement: Integration tests cover API endpoints
The system SHALL provide tests for GET/POST/PUT/DELETE `/api/v1/projects` verifying status codes, response bodies, and ProblemDetails errors.

#### Scenario: POST endpoint creates resource
- **WHEN** POST `/api/v1/projects` with valid JSON
- **THEN** returns 201 with Location header and ProjectDto in body

### Requirement: Integration tests cover database persistence
The system SHALL verify that repository operations (Add, Update, Delete, Query) persist correctly to PostgreSQL.

#### Scenario: Repository saves and retrieves entity
- **WHEN** test calls `repository.AddAsync(project)` then `repository.GetByIdAsync(id)`
- **THEN** returned entity matches saved entity

### Requirement: Test projects compile and run independently
The system SHALL ensure both test projects build and `dotnet test` passes.

#### Scenario: All tests pass
- **WHEN** running `dotnet test test/UnitTests` and `dotnet test test/IntegrationTests`
- **THEN** both complete with 100% pass rate (initially with placeholder tests)