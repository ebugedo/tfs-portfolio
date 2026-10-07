# Tasks

## 1. Domain Layer - Value Objects

- [x] 1.1 Create `YearMonth` readonly record struct in `src/Core/Domain/Common/ValueObjects/YearMonth.cs` with Month (1-12), Year, Create factory, FromDateTime, AddMonths, comparison operators, ToString; verify `dotnet build src/Core/Domain` succeeds
- [x] 1.2 Create `Technology` readonly record struct in `src/Core/Domain/Common/ValueObjects/Technology.cs` with Name, Category enum, Proficiency enum, case-insensitive equality by Name; verify `dotnet build src/Core/Domain` succeeds
- [x] 1.3 Create `Url` readonly record struct in `src/Core/Domain/Common/ValueObjects/Url.cs` with Value, IsValid, implicit conversion, Create factory with URI validation, nullable support; verify `dotnet build src/Core/Domain` succeeds
- [x] 1.4 Create `ContactInfo` readonly record struct in `src/Core/Domain/Common/ValueObjects/ContactInfo.cs` with Email, Phone, Address, Create factory with email validation; verify `dotnet build src/Core/Domain` succeeds
- [x] 1.5 Create unit tests for all value objects in `test/UnitTests/Domain/ValueObjects/` covering validation, equality, edge cases; verify `dotnet test test/UnitTests` passes

## 2. Domain Layer - Client Aggregate

- [x] 2.1 Create `Client` aggregate root in `src/Core/Domain/Clients/Entities/Client.cs` with Id, Name, LogoUrl (Url VO), Email, Phone, Address, IsActive, CreatedAt, UpdatedAt, Create factory, Update method, Deactivate method; verify `dotnet build src/Core/Domain` succeeds
- [x] 2.2 Create `IClientRepository` interface in `src/Core/Domain/Clients/Repositories/IClientRepository.cs` with GetByIdAsync, GetAllAsync, GetActiveAsync, AddAsync, Update, Delete; verify `dotnet build src/Core/Domain` succeeds
- [x] 2.3 Create domain exceptions: `ClientNotFoundException`, `InvalidClientStateException` in `src/Core/Domain/Clients/Exceptions/`; verify `dotnet build src/Core/Domain` succeeds
- [x] 2.4 Create domain events: `ClientCreatedEvent`, `ClientUpdatedEvent`, `ClientDeactivatedEvent` in `src/Core/Domain/Clients/Events/` implementing IDomainEvent; verify `dotnet build src/Core/Domain` succeeds
- [x] 2.5 Create unit tests for Client aggregate in `test/UnitTests/Domain/Clients/` covering create, update, deactivate, validation, events; verify `dotnet test test/UnitTests` passes

## 3. Domain Layer - Sector Aggregate

- [x] 3.1 Create `Sector` aggregate root in `src/Core/Domain/Sectors/Entities/Sector.cs` with Id, Name, Description, IsActive, CreatedAt, Create factory, Update method; verify `dotnet build src/Core/Domain` succeeds
- [x] 3.2 Create `ISectorRepository` interface in `src/Core/Domain/Sectors/Repositories/ISectorRepository.cs` with GetByIdAsync, GetAllAsync, GetActiveAsync, AddAsync, Update, Delete; verify `dotnet build src/Core/Domain` succeeds
- [x] 3.3 Create domain exceptions: `SectorNotFoundException`, `InvalidSectorStateException` in `src/Core/Domain/Sectors/Exceptions/`; verify `dotnet build src/Core/Domain` succeeds
- [x] 3.4 Create domain events: `SectorCreatedEvent`, `SectorUpdatedEvent` in `src/Core/Domain/Sectors/Events/`; verify `dotnet build src/Core/Domain` succeeds
- [x] 3.5 Create unit tests for Sector aggregate in `test/UnitTests/Domain/Sectors/`; verify `dotnet test test/UnitTests` passes

## 4. Domain Layer - Service Aggregate

- [x] 4.1 Create `Service` aggregate root in `src/Core/Domain/Services/Entities/Service.cs` with Id, Name, Description, Category enum (Development/Consulting/Design/DevOps/Training), IsActive, CreatedAt, Create factory, Update method; verify `dotnet build src/Core/Domain` succeeds
- [x] 4.2 Create `IServiceRepository` interface in `src/Core/Domain/Services/Repositories/IServiceRepository.cs` with GetByIdAsync, GetAllAsync, GetActiveAsync, GetByCategoryAsync, AddAsync, Update, Delete; verify `dotnet build src/Core/Domain` succeeds
- [x] 4.3 Create domain exceptions: `ServiceNotFoundException`, `InvalidServiceStateException` in `src/Core/Domain/Services/Exceptions/`; verify `dotnet build src/Core/Domain` succeeds
- [x] 4.4 Create domain events: `ServiceCreatedEvent`, `ServiceUpdatedEvent` in `src/Core/Domain/Services/Events/`; verify `dotnet build src/Core/Domain` succeeds
- [x] 4.5 Create unit tests for Service aggregate in `test/UnitTests/Domain/Services/`; verify `dotnet test test/UnitTests` passes

## 5. Domain Layer - CompanyProfile Aggregate (Singleton)

- [x] 5.1 Create `CompanyProfile` aggregate root in `src/Core/Domain/CompanyProfile/Entities/CompanyProfile.cs` with Id, CompanyName, ContactEmail, ContactPhone, WebUrl (Url VO), Address, Description, LogoUrl (Url VO), CreatedAt, UpdatedAt, Create factory (throws if exists), Update method; verify `dotnet build src/Core/Domain` succeeds
- [x] 5.2 Create `ICompanyProfileRepository` interface in `src/Core/Domain/CompanyProfile/Repositories/ICompanyProfileRepository.cs` with GetAsync, AddAsync, Update, Delete; verify `dotnet build src/Core/Domain` succeeds
- [x] 5.3 Create domain exception: `CompanyProfileAlreadyExistsException` in `src/Core/Domain/CompanyProfile/Exceptions/`; verify `dotnet build src/Core/Domain` succeeds
- [x] 5.4 Create domain event: `CompanyProfileUpdatedEvent` in `src/Core/Domain/CompanyProfile/Events/`; verify `dotnet build src/Core/Domain` succeeds
- [x] 5.5 Create unit tests for CompanyProfile aggregate in `test/UnitTests/Domain/CompanyProfile/` covering singleton enforcement; verify `dotnet test test/UnitTests` passes

## 6. Domain Layer - Project Aggregate Enhancements

- [x] 6.1 Enhance `Project` entity in `src/Core/Domain/Projects/Entities/Project.cs` with StartDate (YearMonth), DurationMonths (1-120), Technologies (List<Technology>), ClientId, SectorId, Status enum (Draft/Active/OnHold/Completed/Cancelled), Services collection, computed EndDate, ChangeStatus method, AddTechnology/RemoveTechnology, AddService/RemoveService methods; verify `dotnet build src/Core/Domain` succeeds
- [x] 6.2 Update `IProjectRepository` in `src/Core/Domain/Projects/Repositories/IProjectRepository.cs` with GetByClientIdAsync, GetBySectorIdAsync, GetByTechnologyAsync, GetByStatusAsync, GetActiveAsync; verify `dotnet build src/Core/Domain` succeeds
- [x] 6.3 Create domain exceptions: `InvalidProjectStateException` (enhance), add status transition validation; verify `dotnet build src/Core/Domain` succeeds
- [x] 6.4 Create domain events: `ProjectUpdatedEvent`, `ProjectStatusChangedEvent` in `src/Core/Domain/Projects/Events/`; verify `dotnet build src/Core/Domain` succeeds
- [x] 6.5 Create unit tests for enhanced Project in `test/UnitTests/Domain/Projects/` covering timeline, technologies, relationships, status transitions, service associations; verify `dotnet test test/UnitTests` passes

## 7. Application Layer - Client Commands/Queries/Handlers

- [x] 7.1 Create DTOs in `src/Core/Application/Clients/Dtos/`: `ClientDto`, `ClientListItemDto`, `CreateClientRequest`, `UpdateClientRequest`; verify `dotnet build src/Core/Application` succeeds
- [x] 7.2 Create Commands in `src/Core/Application/Clients/Commands/`: `CreateClientCommand` (ICommand<Guid>), `UpdateClientCommand`, `DeleteClientCommand`; verify `dotnet build src/Core/Application` succeeds
- [x] 7.3 Create Queries in `src/Core/Application/Clients/Queries/`: `GetClientByIdQuery` (IQuery<ClientDto>), `GetClientsQuery` (IQuery<IReadOnlyList<ClientListItemDto>>), `GetActiveClientsQuery`; verify `dotnet build src/Core/Application` succeeds
- [x] 7.4 Create Handlers in `src/Core/Application/Clients/Handlers/`: `CreateClientCommandHandler`, `UpdateClientCommandHandler`, `DeleteClientCommandHandler`, `GetClientByIdQueryHandler`, `GetClientsQueryHandler`, `GetActiveClientsQueryHandler`; verify `dotnet build src/Core/Application` succeeds
- [x] 7.5 Create Validators in `src/Core/Application/Clients/Validators/`: `CreateClientCommandValidator`, `UpdateClientCommandValidator`, `GetClientByIdQueryValidator`; verify `dotnet build src/Core/Application` succeeds
- [x] 7.6 Create `ClientMappingProfile` in `src/Core/Application/Clients/Mapping/`; verify `dotnet build src/Core/Application` succeeds
- [x] 7.7 Create unit tests for Client handlers in `test/UnitTests/Application/Clients/`; verify `dotnet test test/UnitTests` passes

## 8. Application Layer - Sector Commands/Queries/Handlers

- [x] 8.1 Create DTOs in `src/Core/Application/Sectors/Dtos/`: `SectorDto`, `SectorListItemDto`, `CreateSectorRequest`, `UpdateSectorRequest`; verify `dotnet build src/Core/Application` succeeds
- [x] 8.2 Create Commands: `CreateSectorCommand`, `UpdateSectorCommand`, `DeleteSectorCommand`; verify `dotnet build src/Core/Application` succeeds
- [x] 8.3 Create Queries: `GetSectorByIdQuery`, `GetSectorsQuery`, `GetActiveSectorsQuery`; verify `dotnet build src/Core/Application` succeeds
- [x] 8.4 Create Handlers and Validators for all Sector commands/queries; verify `dotnet build src/Core/Application` succeeds
- [x] 8.5 Create `SectorMappingProfile`; verify `dotnet build src/Core/Application` succeeds
- [x] 8.6 Create unit tests for Sector handlers; verify `dotnet test test/UnitTests` passes

## 9. Application Layer - Service Commands/Queries/Handlers

- [x] 9.1 Create DTOs in `src/Core/Application/Services/Dtos/`: `ServiceDto`, `ServiceListItemDto`, `CreateServiceRequest`, `UpdateServiceRequest`; verify `dotnet build src/Core/Application` succeeds
- [x] 9.2 Create Commands: `CreateServiceCommand`, `UpdateServiceCommand`, `DeleteServiceCommand`; verify `dotnet build src/Core/Application` succeeds
- [x] 9.3 Create Queries: `GetServiceByIdQuery`, `GetServicesQuery`, `GetActiveServicesQuery`, `GetServicesByCategoryQuery`; verify `dotnet build src/Core/Application` succeeds
- [x] 9.4 Create Handlers and Validators for all Service commands/queries; verify `dotnet build src/Core/Application` succeeds
- [x] 9.5 Create `ServiceMappingProfile`; verify `dotnet build src/Core/Application` succeeds
- [x] 9.6 Create unit tests for Service handlers; verify `dotnet test test/UnitTests` passes

## 10. Application Layer - CompanyProfile Commands/Queries/Handlers

- [x] 10.1 Create DTOs in `src/Core/Application/CompanyProfile/Dtos/`: `CompanyProfileDto`, `UpdateCompanyProfileRequest`; verify `dotnet build src/Core/Application` succeeds
- [x] 10.2 Create Commands: `UpdateCompanyProfileCommand`; verify `dotnet build src/Core/Application` succeeds
- [x] 10.3 Create Queries: `GetCompanyProfileQuery`; verify `dotnet build src/Core/Application` succeeds
- [x] 10.4 Create Handlers and Validators for CompanyProfile; verify `dotnet build src/Core/Application` succeeds
- [x] 10.5 Create `CompanyProfileMappingProfile`; verify `dotnet build src/Core/Application` succeeds
- [x] 10.6 Create unit tests for CompanyProfile handlers; verify `dotnet test test/UnitTests` passes

## 11. Application Layer - Enhanced Project Commands/Queries/Handlers

- [x] 11.1 Extend DTOs in `src/Core/Application/Projects/Dtos/`: update `ProjectDto`, `CreateProjectRequest`, `UpdateProjectRequest` with StartDate, DurationMonths, Technologies, ClientId, SectorId, Status; verify `dotnet build src/Core/Application` succeeds
- [x] 11.2 Create new Commands: `ChangeProjectStatusCommand`, `AddProjectServiceCommand`, `RemoveProjectServiceCommand`; verify `dotnet build src/Core/Application` succeeds
- [x] 11.3 Create new Queries: `GetProjectsByClientQuery`, `GetProjectsBySectorQuery`, `GetProjectsByTechnologyQuery`, `GetProjectsByStatusQuery`; verify `dotnet build src/Core/Application` succeeds
- [x] 11.4 Create Handlers and Validators for new Project commands/queries; verify `dotnet build src/Core/Application` succeeds
- [x] 11.5 Update `ProjectMappingProfile` for new properties and relationships; verify `dotnet build src/Core/Application` succeeds
- [x] 11.6 Create unit tests for enhanced Project handlers; verify `dotnet test test/UnitTests` passes

## 12. Application Layer - Module Registration

- [x] 12.1 Update `ApplicationModule` in `src/Core/Application/Common/Modules/ApplicationModule.cs` to register all new handlers, validators, and mapping profiles; verify `dotnet build src/Core/Application` succeeds

## 13. Persistence Layer - Entity Configurations

- [x] 13.1 Create `ClientConfiguration` in `src/Infrastructure/Persistence/Configurations/ClientConfiguration.cs` mapping to "Clients" table with Id (uuid PK), Name (varchar 200), LogoUrl (jsonb), Email, Phone, Address, IsActive, CreatedAt/UpdatedAt (timestamptz), indexes; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 13.2 Create `SectorConfiguration` in `src/Infrastructure/Persistence/Configurations/SectorConfiguration.cs` mapping to "Sectors" table with unique index on Name; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 13.3 Create `ServiceConfiguration` in `src/Infrastructure/Persistence/Configurations/ServiceConfiguration.cs` mapping to "Services" table; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 13.4 Create `CompanyProfileConfiguration` in `src/Infrastructure/Persistence/Configurations/CompanyProfileConfiguration.cs` with singleton constraint; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 13.5 Update `ProjectConfiguration` with StartDate (jsonb), DurationMonths, Technologies (jsonb), ClientId (FK), SectorId (FK), Status, indexes; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 13.6 Create `ProjectServiceConfiguration` for join table "ProjectServices" with composite PK, FKs; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 13.7 Update `ApplicationDbContext` with new DbSets: Clients, Sectors, Services, CompanyProfiles; verify `dotnet build src/Infrastructure/Persistence` succeeds

## 14. Persistence Layer - Repository Implementations

- [x] 14.1 Create `ClientRepository` in `src/Infrastructure/Persistence/Repositories/ClientRepository.cs` implementing `IClientRepository`; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 14.2 Create `SectorRepository` implementing `ISectorRepository`; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 14.3 Create `ServiceRepository` implementing `IServiceRepository`; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 14.4 Create `CompanyProfileRepository` implementing `ICompanyProfileRepository` with singleton GetAsync; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 14.5 Update `ProjectRepository` with extended query methods; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 14.6 Update `PersistenceModule` to register all new repositories; verify `dotnet build src/Infrastructure/Persistence` succeeds

## 15. Persistence Layer - Migration

- [x] 15.1 Generate migration: `dotnet ef migrations add AddPortfolioDomainEntities -p src/Infrastructure/Persistence -s src/Presentation/WebAPI --output-dir Migrations`; verify migration files created
- [x] 15.2 Review migration Up/Down for all tables, columns, FKs, indexes, JSONB types, join table; verify schema matches design
- [x] 15.3 Test migration locally with Testcontainers (already configured in init-architecture): integration tests use Testcontainers PostgreSQL container which automatically applies migrations on startup; verify `dotnet build test/IntegrationTests` succeeds and tests pass when Docker daemon is available

## 16. API Layer - Client Endpoints

- [x] 16.1 Create `ClientEndpoints` static class in `src/Presentation/WebAPI/Endpoints/ClientEndpoints.cs` with MapClientEndpoints extension method for GET/POST/PUT/DELETE /api/v1/clients; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 16.2 Register ClientEndpoints in Program.cs v1 MapGroup; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 16.3 Create integration tests for Client endpoints in `test/IntegrationTests/Api/ClientsApiTests.cs`; verify `dotnet test test/IntegrationTests` passes

## 17. API Layer - Sector Endpoints

- [ ] 17.1 Create `SectorEndpoints` in `src/Presentation/WebAPI/Endpoints/SectorEndpoints.cs` with CRUD endpoints under /api/v1/sectors; verify `dotnet build src/Presentation/WebAPI` succeeds
- [ ] 17.2 Register SectorEndpoints in Program.cs; verify `dotnet build src/Presentation/WebAPI` succeeds
- [ ] 17.3 Create integration tests for Sector endpoints; verify `dotnet test test/IntegrationTests` passes

## 18. API Layer - Service Endpoints

- [ ] 18.1 Create `ServiceEndpoints` in `src/Presentation/WebAPI/Endpoints/ServiceEndpoints.cs` with CRUD + GET by category under /api/v1/services; verify `dotnet build src/Presentation/WebAPI` succeeds
- [ ] 18.2 Register ServiceEndpoints in Program.cs; verify `dotnet build src/Presentation/WebAPI` succeeds
- [ ] 18.3 Create integration tests for Service endpoints; verify `dotnet test test/IntegrationTests` passes

## 19. API Layer - CompanyProfile Endpoints

- [ ] 19.1 Create `CompanyProfileEndpoints` in `src/Presentation/WebAPI/Endpoints/CompanyProfileEndpoints.cs` with GET/PUT /api/v1/company-profile; verify `dotnet build src/Presentation/WebAPI` succeeds
- [ ] 19.2 Register CompanyProfileEndpoints in Program.cs; verify `dotnet build src/Presentation/WebAPI` succeeds
- [ ] 19.3 Create integration tests for CompanyProfile endpoints; verify `dotnet test test/IntegrationTests` passes

## 20. API Layer - Enhanced Project Endpoints

- [ ] 20.1 Update `ProjectEndpoints` in `src/Presentation/WebAPI/Endpoints/ProjectEndpoints.cs` with query filters (clientId, sectorId, technology, status), updated POST/PUT, status change endpoint, service association endpoints; verify `dotnet build src/Presentation/WebAPI` succeeds
- [ ] 20.2 Create integration tests for enhanced Project endpoints including filters and service associations; verify `dotnet test test/IntegrationTests` passes

## 21. Integration Tests - Persistence Layer

- [ ] 21.1 Create persistence integration tests in `test/IntegrationTests/Persistence/` for ClientRepository, SectorRepository, ServiceRepository, CompanyProfileRepository, extended ProjectRepository queries; verify `dotnet test test/IntegrationTests` passes

## 22. Bogus Fakers for Test Data

- [ ] 22.1 Create fakers in `test/UnitTests/Common/`: `ClientFaker`, `SectorFaker`, `ServiceFaker`, `CompanyProfileFaker`, `YearMonthFaker`, `TechnologyFaker`, `UrlFaker`, `ContactInfoFaker`; verify `dotnet build test/UnitTests` succeeds
- [ ] 22.2 Create command/request fakers for all new aggregates; verify `dotnet build test/UnitTests` succeeds

## 23. Full Solution Verification

- [ ] 23.1 Run full solution build: `dotnet build Tfs.Portfolio.slnx`; verify zero errors, zero warnings
- [ ] 23.2 Run all unit tests: `dotnet test test/UnitTests`; verify 100% pass
- [ ] 23.3 Run all integration tests: `dotnet test test/IntegrationTests` (requires Docker); verify 100% pass
- [ ] 23.4 Verify API documentation at `/scalar/v1` shows all new endpoints with schemas
- [ ] 23.5 Verify Docker Compose starts all services and API responds on http://localhost:8082/scalar/v1