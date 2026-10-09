# Spec Delta

## MODIFIED Requirements

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

## ADDED Requirements

### Requirement: Unit tests for new Domain aggregates
The system SHALL provide unit tests for Client, Sector, Service, CompanyProfile aggregates covering: Create with valid/invalid data, Update, Deactivate (Client), status transitions (Project), singleton enforcement (CompanyProfile), value object validation.

#### Scenario: Client aggregate tests
- **WHEN** running `Client_Create_WithValidData_RaisesClientCreatedEvent`
- **THEN** verifies Client state, Id generated, IsActive=true, event raised
- **WHEN** running `Client_Create_WithEmptyName_ThrowsInvalidClientStateException`
- **THEN** verifies exception thrown with correct message
- **WHEN** running `Client_Deactivate_SetsIsActiveFalseAndRaisesEvent`
- **THEN** verifies IsActive=false, UpdatedAt set, ClientDeactivatedEvent raised

#### Scenario: Sector aggregate tests
- **WHEN** running `Sector_Create_WithValidData_RaisesSectorCreatedEvent`
- **THEN** verifies Sector state and event
- **WHEN** running `Sector_Create_WithNameTooLong_ThrowsException`
- **THEN** verifies exception for >100 chars

#### Scenario: Service aggregate tests
- **WHEN** running `Service_Create_WithValidCategory_RaisesEvent`
- **THEN** verifies Service state and event

#### Scenario: CompanyProfile singleton tests
- **WHEN** running `CompanyProfile_Create_FirstTime_Succeeds`
- **THEN** verifies instance created
- **WHEN** running `CompanyProfile_Create_SecondTime_ThrowsException`
- **THEN** verifies CompanyProfileAlreadyExistsException thrown

#### Scenario: Project enhanced tests
- **WHEN** running `Project_ChangeStatus_ValidTransition_Succeeds`
- **THEN** verifies status change and ProjectStatusChangedEvent
- **WHEN** running `Project_ChangeStatus_InvalidTransition_ThrowsException`
- **THEN** verifies InvalidProjectStateException
- **WHEN** running `Project_AddTechnology_DuplicateName_ThrowsException`
- **THEN** verifies duplicate prevention

#### Scenario: Value object tests
- **WHEN** running `YearMonth_Create_InvalidMonth_ThrowsException`
- **THEN** verifies ArgumentException for month 13
- **WHEN** running `Technology_Equality_CaseInsensitive_ReturnsTrue`
- **THEN** verifies "React" equals "react"
- **WHEN** running `Url_Create_InvalidFormat_ThrowsException`
- **THEN** verifies ArgumentException
- **WHEN** running `ContactInfo_Create_InvalidEmail_ThrowsException`
- **THEN** verifies ArgumentException

### Requirement: Unit tests for new Application handlers
The system SHALL provide tests for all new command/query handlers: CreateClient, GetClient, GetClients, CreateSector, GetSector, GetSectors, CreateService, GetService, GetServices, GetServicesByCategory, GetCompanyProfile, UpdateCompanyProfile, CreateProject (enhanced), ChangeProjectStatus, AddProjectService, GetProjects (filtered), GetProjectsByClient, GetProjectsBySector, GetProjectsByTechnology, GetProjectsByStatus.

#### Scenario: Handler tests mock repositories
- **WHEN** running `CreateClientCommandHandler_WithValidCommand_CallsRepositoryAndReturnsDto`
- **THEN** verifies repository.AddAsync called, UnitOfWork.SaveChangesAsync called, ClientDto returned

#### Scenario: Query handler tests with filters
- **WHEN** running `GetProjectsQueryHandler_WithClientIdFilter_ReturnsFiltered`
- **THEN** verifies repository.GetByClientIdAsync called with correct Id

### Requirement: Bogus fakers for new entities and commands
The system SHALL provide Faker classes: ClientFaker, SectorFaker, ServiceFaker, CompanyProfileFaker, YearMonthFaker, TechnologyFaker, UrlFaker, ContactInfoFaker, and corresponding command/request fakers.

#### Scenario: Fakers generate valid test data
- **WHEN** `new ClientFaker().Generate()` is called
- **THEN** returns Client with valid Name, Email, LogoUrl, random Guid

### Requirement: Integration tests for new API endpoints
The system SHALL provide integration tests for all new endpoints:
- GET/POST/PUT/DELETE /api/v1/clients
- GET/POST/PUT/DELETE /api/v1/sectors
- GET/POST/PUT/DELETE /api/v1/services + GET by category
- GET/PUT /api/v1/company-profile
- Enhanced GET/POST/PUT /api/v1/projects with filters
- POST/DELETE /api/v1/projects/{id}/services/{serviceId}

#### Scenario: Client integration tests
- **WHEN** running `CreateClient_WithValidBody_ReturnsCreatedWithDto`
- **THEN** verifies 201, Location header, ClientDto in body
- **WHEN** running `GetClient_WhenNotFound_Returns404ProblemDetails`
- **THEN** verifies 404 with ProblemDetails

#### Scenario: CompanyProfile integration tests
- **WHEN** running `GetCompanyProfile_WhenNotCreated_Returns404`
- **THEN** verifies 404
- **WHEN** running `UpdateCompanyProfile_UpdatesSingleton`
- **THEN** verifies 200 with updated data

#### Scenario: Project filter integration tests
- **WHEN** running `GetProjects_WithClientIdFilter_ReturnsFiltered`
- **THEN** creates projects for different clients, queries with filter, verifies only matching returned

### Requirement: Integration tests for new repository implementations
The system SHALL provide persistence integration tests for ClientRepository, SectorRepository, ServiceRepository, CompanyProfileRepository, and extended ProjectRepository queries.

#### Scenario: Repository integration tests
- **WHEN** running `ClientRepository_AddAsync_PersistsToDatabase`
- **THEN** verifies entity saved and retrievable
- **WHEN** running `ProjectRepository_GetByClientIdAsync_ReturnsFiltered`
- **THEN** verifies query filters by ClientId correctly

### Requirement: Integration tests verify value object JSONB persistence
The system SHALL include integration tests that verify nullable `TfsWebUrl` value objects persist and load correctly through EF Core JSONB columns.

#### Scenario: Client with LogoUrl persists and loads correctly
- **WHEN** a `Client` entity with a non-null `LogoUrl` is added via `ClientRepository.AddAsync` and saved
- **THEN** the entity is retrieved via `ClientRepository.GetByIdAsync` with the same `LogoUrl` value
- **THEN** `LogoUrl.IsValid` returns `true` and `LogoUrl.Value` matches the original URL

#### Scenario: Client with null LogoUrl persists and loads correctly
- **WHEN** a `Client` entity with `null` `LogoUrl` is added via `ClientRepository.AddAsync` and saved
- **THEN** the entity is retrieved via `ClientRepository.GetByIdAsync` with `LogoUrl` being `null`

#### Scenario: CompanyProfile with TfsWebUrl and LogoUrl persists and loads correctly
- **WHEN** a `CompanyProfileEntity` with non-null `TfsWebUrl` and `LogoUrl` is added via `CompanyProfileRepository.AddAsync` and saved
- **THEN** the entity is retrieved via `CompanyProfileRepository.GetAsync` with both URLs correctly populated
- **THEN** both URLs have `IsValid = true` and correct values

#### Scenario: CompanyProfile with null URLs persists and loads correctly
- **WHEN** a `CompanyProfileEntity` with `null` `TfsWebUrl` and `LogoUrl` is added and saved
- **THEN** the entity is retrieved with both URL properties being `null`

### Requirement: Integration tests use valid test data with proper FK relationships
The system SHALL ensure all integration tests that create entities with foreign key relationships (e.g., Projects with ClientId, SectorId) first create the referenced entities (Clients, Sectors) to satisfy database constraints.

#### Scenario: Project integration tests create required Client and Sector entities first
- **WHEN** an integration test creates a `Project` entity with `ClientId` and `SectorId`
- **THEN** the test first creates and persists valid `Client` and `Sector` entities
- **THEN** the `Project` references the created entities' IDs
- **THEN** no PostgreSQL foreign key constraint violation (23503) occurs

#### Scenario: Test fixtures provide helper methods for creating valid entity graphs
- **WHEN** integration tests need to create Projects with relationships
- **THEN** test fixtures or base classes provide helper methods like `CreateTestClientAsync()`, `CreateTestSectorAsync()`, `CreateTestProjectAsync(clientId, sectorId)`
- **THEN** these helpers ensure all FK references are valid before saving

### Requirement: Integration test projects build and pass code analysis with zero errors
The system SHALL ensure all integration test projects compile and pass code analysis (StyleCop, Roslyn analyzers) with zero errors and zero warnings.

#### Scenario: Integration test projects pass StyleCop and Roslyn analyzers
- **WHEN** building the solution with `dotnet build Tfs.Portfolio.slnx`
- **THEN** all projects compile with zero errors and zero warnings
- **THEN** StyleCop rules (SA1028, SA1137, SA1202, SA1611) pass
- **THEN** Roslyn analyzers (CA1002, CA1823, CS0108) pass
- **THEN** no CS0103 undefined variable errors
- **THEN** no CS1503 argument type mismatch errors
- **THEN** no CS0108 member hiding without `new` keyword

#### Scenario: Integration test projects have no unused code
- **WHEN** running code analysis
- **THEN** no CA1823 unused field warnings
- **THEN** no CA1849 async blocking warnings
- **THEN** no CS0618 obsolete member usage warnings