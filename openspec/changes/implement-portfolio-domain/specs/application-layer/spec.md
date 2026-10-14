# Spec Delta

## MODIFIED Requirements

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

## ADDED Requirements

### Requirement: Client commands and queries
The system SHALL provide CreateClientCommand (returns Guid), UpdateClientCommand, DeleteClientCommand, GetClientByIdQuery (returns ClientDto), GetClientsQuery (returns IReadOnlyList<ClientListItemDto>), GetActiveClientsQuery, with corresponding handlers.

#### Scenario: CreateClientCommandHandler creates client
- **WHEN** CreateClientCommand with valid data is handled
- **THEN** Client aggregate created, saved via repository, ClientDto returned with new Id

#### Scenario: GetClientsQuery returns paginated list
- **WHEN** GetClientsQuery with optional filters is handled
- **THEN** returns IReadOnlyList<ClientListItemDto> with Name, Email, IsActive

### Requirement: Sector commands and queries
The system SHALL provide CreateSectorCommand (returns Guid), UpdateSectorCommand, DeleteSectorCommand, GetSectorByIdQuery (returns SectorDto), GetSectorsQuery, GetActiveSectorsQuery, with corresponding handlers.

#### Scenario: CreateSectorCommandHandler creates sector
- **WHEN** CreateSectorCommand with valid data is handled
- **THEN** Sector aggregate created, saved, SectorDto returned

### Requirement: Service commands and queries
The system SHALL provide CreateServiceCommand (returns Guid), UpdateServiceCommand, DeleteServiceCommand, GetServiceByIdQuery (returns ServiceDto), GetServicesQuery, GetActiveServicesQuery, GetServicesByCategoryQuery, with corresponding handlers.

#### Scenario: GetServicesByCategoryQuery filters by category
- **WHEN** GetServicesByCategoryQuery with ServiceCategory.Development is handled
- **THEN** returns only Development services

### Requirement: CompanyProfile commands and queries (singleton)
The system SHALL provide UpdateCompanyProfileCommand, GetCompanyProfileQuery (returns CompanyProfileDto), with handlers. No Create/Delete commands (singleton managed by domain).

#### Scenario: GetCompanyProfileQuery returns singleton
- **WHEN** GetCompanyProfileQuery is handled
- **THEN** returns CompanyProfileDto if exists, null otherwise

#### Scenario: UpdateCompanyProfileCommand updates singleton
- **WHEN** UpdateCompanyProfileCommand with valid data is handled
- **THEN** singleton updated, CompanyProfileDto returned

### Requirement: Enhanced Project commands and queries
The system SHALL extend with CreateProjectCommand (includes StartDate, DurationMonths, ClientId, SectorId, Technologies), UpdateProjectCommand (includes Status), ChangeProjectStatusCommand, AddProjectServiceCommand, RemoveProjectServiceCommand, GetProjectsQuery with filters (clientId, sectorId, technology, status), GetProjectByIdQuery, GetProjectsByClientQuery, GetProjectsBySectorQuery, GetProjectsByTechnologyQuery, GetProjectsByStatusQuery.

#### Scenario: CreateProjectCommandHandler validates relationships
- **WHEN** CreateProjectCommand with non-existent ClientId is handled
- **THEN** throws ClientNotFoundException

#### Scenario: ChangeProjectStatusCommandHandler enforces transitions
- **WHEN** ChangeProjectStatusCommand with invalid transition is handled
- **THEN** throws InvalidProjectStateException

#### Scenario: GetProjectsQuery filters by multiple criteria
- **WHEN** GetProjectsQuery with clientId and status is handled
- **THEN** returns only projects matching both filters

### Requirement: Client, Sector, Service, CompanyProfile DTOs
The system SHALL provide DTOs: ClientDto, ClientListItemDto, CreateClientRequest, UpdateClientRequest, SectorDto, SectorListItemDto, CreateSectorRequest, UpdateSectorRequest, ServiceDto, ServiceListItemDto, CreateServiceRequest, UpdateServiceRequest, CompanyProfileDto, UpdateCompanyProfileRequest, all using camelCase properties.

#### Scenario: DTOs use camelCase for JSON serialization
- **WHEN** inspecting ClientDto properties
- **THEN** uses clientId, clientName, logoUrl, contactEmail, isActive, createdAt

### Requirement: AutoMapper profiles for all new aggregates
The system SHALL provide ClientMappingProfile, SectorMappingProfile, ServiceMappingProfile, CompanyProfileMappingProfile, and extend ProjectMappingProfile for new properties and relationships.

#### Scenario: Client mapping includes value objects
- **WHEN** mapper.Map<ClientDto>(clientEntity) is called
- **THEN** LogoUrl, ContactInfo mapped correctly to DTO

#### Scenario: Project mapping includes technologies and relations
- **WHEN** mapper.Map<ProjectDto>(projectEntity) is called
- **THEN** Technologies, ClientId, SectorId, Status, Services mapped

### Requirement: FluentValidation validators for all new commands/queries
The system SHALL provide validators: CreateClientCommandValidator, UpdateClientCommandValidator, GetClientByIdQueryValidator, CreateSectorCommandValidator, UpdateSectorCommandValidator, GetSectorByIdQueryValidator, CreateServiceCommandValidator, UpdateServiceCommandValidator, GetServiceByIdQueryValidator, UpdateCompanyProfileCommandValidator, CreateProjectCommandValidator (extended), UpdateProjectCommandValidator (extended), ChangeProjectStatusCommandValidator, AddProjectServiceCommandValidator, GetProjectsQueryValidator.

#### Scenario: CreateClientCommandValidator validates required fields
- **WHEN** CreateClientCommand with empty Name is validated
- **THEN** fails with "Client name is required"

#### Scenario: CreateProjectCommandValidator validates relationships
- **WHEN** CreateProjectCommand with DurationMonths > 120 is validated
- **THEN** fails with "Duration must be between 1 and 120 months"

### Requirement: ApplicationModule registers all new handlers, validators, mappers
The system SHALL extend ApplicationModule to register all new command/query handlers, validators, and AutoMapper profiles for Clients, Sectors, Services, CompanyProfile, and enhanced Projects.

#### Scenario: All new handlers resolvable via Autofac
- **WHEN** ApplicationModule.Load is executed
- **THEN** CreateClientCommandHandler, GetClientsQueryHandler, etc. registered as interfaces and concrete types