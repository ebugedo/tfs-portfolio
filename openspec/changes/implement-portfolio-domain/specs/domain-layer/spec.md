# Spec Delta

## MODIFIED Requirements

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
The system SHALL provide a `DomainException` base class and specific exceptions (e.g., `ProjectNotFoundException`, `InvalidProjectStateException`, `ClientNotFoundException`, `InvalidClientStateException`, `SectorNotFoundException`, `InvalidSectorStateException`, `ServiceNotFoundException`, `InvalidServiceStateException`, `CompanyProfileAlreadyExistsException`) for business rule failures.

#### Scenario: Domain exception is thrown for rule violation
- **WHEN** an aggregate method validates a business rule that fails
- **THEN** it throws a specific `DomainException` derivative with a clear message

### Requirement: Repository interfaces are defined in Domain
The system SHALL define repository interfaces (e.g., `IProjectRepository`, `IClientRepository`, `ISectorRepository`, `IServiceRepository`, `ICompanyProfileRepository`) in the Domain layer with only Domain types in signatures.

#### Scenario: Repository interface uses only Domain types
- **WHEN** inspecting `IClientRepository`
- **THEN** methods return `Task<Client?>`, `Task<IReadOnlyList<Client>>`, etc.—no EF Core or DTO types

#### Scenario: Repository interfaces include new query methods
- **WHEN** inspecting `IProjectRepository`
- **THEN** includes `GetByClientIdAsync`, `GetBySectorIdAsync`, `GetByTechnologyAsync`, `GetByStatusAsync`, `GetActiveAsync`

#### Scenario: ICompanyProfileRepository enforces singleton
- **WHEN** inspecting `ICompanyProfileRepository`
- **THEN** has `GetAsync()` returning single instance, `AddAsync` throws if exists

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

## ADDED Requirements

### Requirement: Client aggregate with contact info and logo
The system SHALL provide a Client aggregate root with Name, LogoUrl (Url VO), Email, Phone, Address, IsActive, CreatedAt, UpdatedAt, factory method Create, Update method, and Deactivate method.

#### Scenario: Client created with valid data
- **WHEN** Client.Create("Acme Corp", "contact@acme.com", "https://acme.com/logo.png") is called
- **THEN** returns Client with generated Id, IsActive=true, CreatedAt=UtcNow

#### Scenario: Client creation validates required fields
- **WHEN** Client.Create with empty Name or invalid Email
- **THEN** throws InvalidClientStateException with descriptive message

#### Scenario: Client update modifies properties
- **WHEN** Client.Update("New Name", "new@email.com", ...) is called
- **THEN** properties updated, UpdatedAt=UtcNow, ClientUpdatedEvent raised

#### Scenario: Client deactivation sets IsActive false
- **WHEN** Client.Deactivate() is called
- **THEN** IsActive=false, UpdatedAt=UtcNow, ClientDeactivatedEvent raised

### Requirement: Sector aggregate for project categorization
The system SHALL provide a Sector aggregate root with Name, Description, IsActive, CreatedAt, factory method Create, and Update method.

#### Scenario: Sector created with valid data
- **WHEN** Sector.Create("FinTech", "Financial technology sector") is called
- **THEN** returns Sector with generated Id, IsActive=true, CreatedAt=UtcNow

#### Scenario: Sector creation validates name
- **WHEN** Sector.Create with empty Name or >100 chars
- **THEN** throws InvalidSectorStateException with descriptive message

### Requirement: Service aggregate with category
The system SHALL provide a Service aggregate root with Name, Description, Category (Development/Consulting/Design/DevOps/Training), IsActive, CreatedAt, factory method Create, and Update method.

#### Scenario: Service created with valid category
- **WHEN** Service.Create("Web Development", "Building web apps", ServiceCategory.Development) is called
- **THEN** returns Service with generated Id, IsActive=true, CreatedAt=UtcNow

### Requirement: CompanyProfile singleton aggregate
The system SHALL provide a CompanyProfile aggregate root with CompanyName, ContactEmail, ContactPhone, WebUrl, Address, Description, LogoUrl, CreatedAt, UpdatedAt, factory method Create (throws if exists), and Update method.

#### Scenario: CompanyProfile created once
- **WHEN** CompanyProfile.Create is called first time
- **THEN** returns instance with generated Id

#### Scenario: CompanyProfile creation throws when exists
- **WHEN** CompanyProfile.Create is called second time
- **THEN** throws CompanyProfileAlreadyExistsException

### Requirement: Project aggregate enhanced with timeline, technologies, relationships
The system SHALL extend Project with StartDate (YearMonth), DurationMonths (1-120), Technologies (list of Technology VO), ClientId, SectorId, Status (Draft/Active/OnHold/Completed/Cancelled), Services collection, ChangeStatus method, and AddService/RemoveService methods.

#### Scenario: Project created with timeline and relationships
- **WHEN** Project.Create with StartDate, DurationMonths, ClientId, SectorId
- **THEN** returns Project with computed EndDate, Status=Draft

#### Scenario: Project status transition validates workflow
- **WHEN** Project.ChangeStatus from Draft to Active
- **THEN** status changes, ProjectStatusChangedEvent raised
- **WHEN** Project.ChangeStatus from Active to Draft
- **THEN** throws InvalidProjectStateException

#### Scenario: Project technologies prevent duplicates
- **WHEN** Project.AddTechnology with duplicate name
- **THEN** throws InvalidProjectStateException

#### Scenario: Project services prevent duplicate associations
- **WHEN** Project.AddService with already-associated service
- **THEN** throws InvalidProjectStateException

### Requirement: Shared value objects in Domain.Common
The system SHALL provide YearMonth, Technology, Url, and ContactInfo as readonly record structs in Domain.Common.ValueObjects.

#### Scenario: YearMonth validates month range
- **WHEN** YearMonth.Create(13, 2024)
- **THEN** throws ArgumentException

#### Scenario: Technology equality by name case-insensitive
- **WHEN** Technology with names "React" and "react" compared
- **THEN** returns true

#### Scenario: Url validates URI format
- **WHEN** Url.Create("invalid")
- **THEN** throws ArgumentException

#### Scenario: ContactInfo validates email format
- **WHEN** ContactInfo.Create("invalid-email", "phone", "address")
- **THEN** throws ArgumentException

### Requirement: Additional domain events for new aggregates
The system SHALL provide ClientCreatedEvent, ClientUpdatedEvent, ClientDeactivatedEvent, SectorCreatedEvent, SectorUpdatedEvent, ServiceCreatedEvent, ServiceUpdatedEvent, CompanyProfileUpdatedEvent, ProjectUpdatedEvent, ProjectStatusChangedEvent implementing IDomainEvent.

#### Scenario: Events carry aggregate identity and relevant data
- **WHEN** ClientCreatedEvent created with ClientId and Name
- **THEN** EventId generated, OccurredOn=UtcNow, properties accessible