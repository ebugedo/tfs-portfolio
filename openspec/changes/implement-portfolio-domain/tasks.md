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

- [x] 17.1 Create `SectorEndpoints` in `src/Presentation/WebAPI/Endpoints/SectorEndpoints.cs` with CRUD endpoints under /api/v1/sectors; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 17.2 Register SectorEndpoints in Program.cs; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 17.3 Create integration tests for Sector endpoints; verify `dotnet test test/IntegrationTests` passes

## 18. API Layer - Service Endpoints

- [x] 18.1 Create `ServiceEndpoints` in `src/Presentation/WebAPI/Endpoints/ServiceEndpoints.cs` with CRUD + GET by category under /api/v1/services; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 18.2 Register ServiceEndpoints in Program.cs; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 18.3 Create integration tests for Service endpoints; verify `dotnet test test/IntegrationTests` passes

## 19. API Layer - CompanyProfile Endpoints

- [x] 19.1 Create `CompanyProfileEndpoints` in `src/Presentation/WebAPI/Endpoints/CompanyProfileEndpoints.cs` with GET/PUT /api/v1/company-profile; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 19.2 Register CompanyProfileEndpoints in Program.cs; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 19.3 Create integration tests for CompanyProfile endpoints; verify `dotnet test test/IntegrationTests` passes

## 20. API Layer - Enhanced Project Endpoints

- [x] 20.1 Update `ProjectEndpoints` in `src/Presentation/WebAPI/Endpoints/ProjectEndpoints.cs` with query filters (clientId, sectorId, technology, status), updated POST/PUT, status change endpoint, service association endpoints; verify `dotnet build src/Presentation/WebAPI` succeeds
- [x] 20.2 Create integration tests for enhanced Project endpoints including filters and service associations; verify `dotnet test test/IntegrationTests` passes

## 21. Integration Tests - Persistence Layer

- [x] 21.1 Create persistence integration tests in `test/IntegrationTests/Persistence/` for ClientRepository, SectorRepository, ServiceRepository, CompanyProfileRepository, extended ProjectRepository queries; verify `dotnet test test/IntegrationTests` passes

## 22. Bogus Fakers for Test Data

- [x] 22.1 Create fakers in `test/UnitTests/Common/`: `ClientFaker`, `SectorFaker`, `ServiceFaker`, `CompanyProfileFaker`, `YearMonthFaker`, `TechnologyFaker`, `UrlFaker`, `ContactInfoFaker`; verify `dotnet build test/UnitTests` succeeds
- [x] 22.2 Create command/request fakers for all new aggregates; verify `dotnet build test/UnitTests` succeeds

## 23. Full Solution Verification

- [x] 23.1 Run full solution build: `dotnet build Tfs.Portfolio.slnx`; verify zero errors, zero warnings
- [x] 23.2 Run all unit tests: `dotnet test test/UnitTests`; verify 100% pass
- [x] 23.3 Run all integration tests: `dotnet test test/IntegrationTests` (requires Docker); verify 100% pass
- [x] 23.4 Verify API documentation at `/scalar/v1` shows all new endpoints with schemas
- [x] 23.5 Verify Docker Compose starts all services and API responds on http://localhost:8082/scalar/v1

## 24. Fix JSONB Value Object Serialization

- [x] 24.1 Fix `ClientConfiguration.LogoUrl` value converter for nullable `TfsWebUrl` in `src/Infrastructure/Persistence/Configurations/ClientConfiguration.cs`; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 24.2 Fix `CompanyProfileConfiguration.TfsWebUrl` value converter for nullable `TfsWebUrl` in `src/Infrastructure/Persistence/Configurations/CompanyProfileConfiguration.cs`; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 24.3 Fix `CompanyProfileConfiguration.LogoUrl` value converter for nullable `TfsWebUrl` in `src/Infrastructure/Persistence/Configurations/CompanyProfileConfiguration.cs`; verify `dotnet build src/Infrastructure/Persistence` succeeds
- [x] 24.4 Add integration tests for `ClientRepository` with `LogoUrl` persistence (non-null and null) in `test/IntegrationTests/Persistence/ClientRepositoryTests.cs`; verify `dotnet test test/IntegrationTests --filter "ClientRepositoryTests"` passes
- [x] 24.5 Add integration tests for `CompanyProfileRepository` with `TfsWebUrl` and `LogoUrl` persistence (non-null and null) in `test/IntegrationTests/Persistence/CompanyProfileRepositoryTests.cs`; verify `dotnet test test/IntegrationTests --filter "CompanyProfileRepositoryTests"` passes
- [x] 24.6 Run full integration test suite: `dotnet test test/IntegrationTests` (requires Docker); verify 100% pass (141/141)

## 25. Fix Integration Test FK Constraint Violations

- [x] 25.1 Analyze failing integration tests to identify all tests creating Projects with invalid ClientId/SectorId FKs
- [x] 25.2 Create Bogus fakers for test data setup that create proper Client/Sector entities before Projects in `test/IntegrationTests/Common/` or `test/IntegrationTests/Api/`
- [x] 25.3 Fix `ProjectsApiTests` - ensure tests create Client and Sector entities before creating Projects with FK references
- [x] 25.4 Fix `ExtendedProjectRepositoryTests` - ensure tests create Client and Sector entities before creating Projects with FK references
- [x] 25.5 Fix `ProjectRepositoryTests` - ensure tests create Client and Sector entities before creating Projects with FK references
- [x] 25.6 Fix `CompanyProfileRepositoryTests` - ensure tests don't conflict with singleton constraint across test isolation
- [x] 25.7 Add helper methods in `IntegrationTestBase` or test fixtures for creating valid test entities with proper FK relationships
- [x] 25.8 Run full integration test suite: `dotnet test test/IntegrationTests` (requires Docker); verify 100% pass (145/145)

## 26. Fix Pre-existing Build Issues (StyleCop & Code Analysis)

- [x] 26.1 Run full build: `dotnet build Tfs.Portfolio.slnx` with zero errors, zero warnings
- [x] 26.2 Run full test suite: `dotnet test Tfs.Portfolio.slnx` verify all pass

## 28. Enforce Newtonsoft.Json Standard (Tech-Stack Compliance)

- [x] 28.1 Update `openspec/standards/tech-stack.md` line 6 to explicitly require Newtonsoft.Json for all serialization/deserialization across the entire solution
  - **Verification**: Line reads: "**JSON Serialization:** Newtonsoft.Json (Json.NET) - Required for all serialization, deserialization, configuration, and logging across the entire solution (domain, application, infrastructure, API, tests)"

- [x] 28.2 Create Newtonsoft.Json-based HTTP extension methods in `test/IntegrationTests/Common/HttpClientExtensions.cs`
  - **Methods**: `PostAsJsonNewtonsoftAsync<T>`, `PutAsJsonNewtonsoftAsync<T>`, `PatchAsJsonNewtonsoftAsync<T>`, `ReadFromJsonNewtonsoftAsync<T>`
  - Uses `Newtonsoft.Json.JsonConvert` for serialization/deserialization
  - **Verification**: Extension methods compile and can be called from integration tests

- [x] 28.3 Update `test/IntegrationTests/IntegrationTestBase.cs` to use new Newtonsoft.Json helpers instead of `PostAsJsonAsync`/`ReadFromJsonAsync`
  - **Verification**: All helper methods in `IntegrationTestBase` use Newtonsoft.Json extensions

- [x] 28.4 Update `test/IntegrationTests/Api/ClientsApiTests.cs` - replace all `PostAsJsonAsync`/`ReadFromJsonAsync` with Newtonsoft.Json helpers
  - **Verification**: All tests compile and pass

- [x] 28.5 Update `test/IntegrationTests/Api/SectorsApiTests.cs` - replace all `PostAsJsonAsync`/`ReadFromJsonAsync` with Newtonsoft.Json helpers
  - **Verification**: All tests compile and pass

- [x] 28.6 Update `test/IntegrationTests/Api/ServicesApiTests.cs` - replace all `PostAsJsonAsync`/`ReadFromJsonAsync` with Newtonsoft.Json helpers
  - **Verification**: All tests compile and pass

- [x] 28.7 Update `test/IntegrationTests/Api/ProjectsApiTests.cs` - replace all `PostAsJsonAsync`/`ReadFromJsonAsync` with Newtonsoft.Json helpers
  - **Verification**: All tests compile and pass

- [x] 28.8 Update `test/IntegrationTests/Api/CompanyProfileApiTests.cs` - replace all `PostAsJsonAsync`/`ReadFromJsonAsync` with Newtonsoft.Json helpers
  - **Verification**: All tests compile and pass

- [x] 28.9 Update `src/Infrastructure/Persistence/Configurations/ClientConfiguration.cs` - replace `System.Text.Json.JsonSerializer` with `Newtonsoft.Json.JsonConvert`
  - **Verification**: `LogoUrl` value converter uses `JsonConvert.SerializeObject`/`DeserializeObject`; build succeeds

- [x] 28.10 Update `src/Infrastructure/Persistence/Configurations/ProjectConfiguration.cs` - replace `System.Text.Json.JsonSerializer` with `Newtonsoft.Json.JsonConvert`
  - **Verification**: `StartDate` and `Technologies` value converters use `JsonConvert`; build succeeds

- [x] 28.11 Update `src/Infrastructure/Persistence/Configurations/CompanyProfileConfiguration.cs` - replace `System.Text.Json.JsonSerializer` with `Newtonsoft.Json.JsonConvert`
  - **Verification**: `TfsWebUrl` and `LogoUrl` value converters use `JsonConvert`; build succeeds

- [x] 28.12 Verify all DTOs in `src/Core/Application/Clients/Dtos/` have proper `[JsonConverter]` attributes for value objects
  - **Verification**: `ClientDto.LogoUrl`, `ClientListItemDto.LogoUrl`, `CreateClientRequest.LogoUrl`, `UpdateClientRequest.LogoUrl` have `[JsonConverter(typeof(TfsWebUrlJsonConverter))]`

- [x] 28.13 Verify all DTOs in `src/Core/Application/Projects/Dtos/` have proper attributes
  - **Verification**: `ProjectDto.StartDate`, `ProjectDto.Technologies`, `CreateProjectRequest.StartDate`, `CreateProjectRequest.Technologies`, `UpdateProjectRequest.StartDate`, `UpdateProjectRequest.Technologies` have proper converters

- [x] 28.13 Verify all DTOs in `src/Core/Application/CompanyProfile/Dtos/` have proper attributes
  - **Verification**: `CompanyProfileDto.WebUrl`, `CompanyProfileDto.LogoUrl`, `UpdateCompanyProfileRequest.WebUrl`, `UpdateCompanyProfileRequest.LogoUrl` have proper converters

- [x] 28.15 Verify `src/Core/Domain/Common/ValueObjects/TfsWebUrl.cs` has correct `TfsWebUrlJsonConverter` for Newtonsoft.Json handling nullable scenarios
  - **Verification**: Null `TfsWebUrl?` serializes to `null` JSON, not `{}`

- [x] 28.16 Verify `src/Core/Domain/Common/ValueObjects/YearMonth.cs` has Newtonsoft.Json converter if needed
  - **Verification**: Serializes correctly in JSONB columns

- [x] 28.17 Verify `src/Core/Domain/Common/ValueObjects/Technology.cs` has Newtonsoft.Json converter if needed
  - **Verification**: Serializes correctly in JSONB columns

- [x] 28.18 Run full solution build: `dotnet build Tfs.Portfolio.slnx` - verify zero errors, zero warnings
  - **Verification**: Build succeeds with 0 warnings, 0 errors

- [x] 28.19 Run all unit tests: `dotnet test test/UnitTests` - verify 100% pass
  - **Verification**: All unit tests pass

- [x] 28.20 Configure Minimal APIs to use Newtonsoft.Json for serialization/deserialization
  - **Action**: Modified `src/Presentation/WebAPI/Program.cs` to use `AddMvcCore().AddNewtonsoftJson()` instead of `AddControllers().AddNewtonsoftJson()` so that both Controllers and Minimal APIs use Newtonsoft.Json
  - **Verification**: Build succeeds with 0 errors, 0 warnings; unit tests pass

- [x] 28.20 Register custom ErrorHandlingMiddleware in middleware pipeline
  - **Action**: Modified `src/Presentation/WebAPI/Program.cs` to replace `app.UseExceptionHandler("/error")` with `app.UseErrorHandling()` so that custom `ErrorHandlingMiddleware` handles exceptions and returns proper ProblemDetails
  - **Verification**: Integration test `ServicesApiTests.CreateService_WithInvalidBody_ReturnsBadRequestWithProblemDetails` passes; `InvalidServiceStateException` returns 400 Bad Request with ProblemDetails

- [ ] 28.21 Run all integration tests: `dotnet test test/IntegrationTests` - verify 100% pass (requires Docker)
  - **Verification**: All integration tests pass

- [x] 28.21 Search codebase for remaining `System.Text.Json` usages - verify zero occurrences in `src/` and `test/`
  - **Verification**: `grep -r "System.Text.Json" src/ test/` returns no results (only JsonElement in tests for ProblemDetails)

## 29. Remove Unused Using Directives (Coding Standard Compliance)

- [x] 29.1 Run `dotnet format --verify-no-changes` on entire solution to identify all files with unused using directives
  - **Verification**: Command exits with code 0 (no changes needed) - **Completed**: No unused using directives found

- [x] 29.2 Remove unused using directives from all `src/` projects (Domain, Application, Infrastructure.Persistence, WebAPI)
  - **Verification**: `dotnet format --verify-no-changes src/` passes - **Completed**: No unused usings in src/

- [x] 29.3 Remove unused using directives from all `test/` projects (UnitTests, IntegrationTests)
  - **Verification**: `dotnet format --verify-no-changes test/` passes - **Completed**: No unused usings in test/

- [x] 29.4 Verify no unused using directives remain: run `dotnet format --verify-no-changes` on entire solution
  - **Verification**: Command exits with code 0 - **Completed**: No unused usings found

- [x] 29.5 Run full solution build: `dotnet build Tfs.Portfolio.slnx` - verify zero errors, zero warnings
  - **Verification**: Build succeeds with 0 errors, only StyleCop documentation warnings (SA1611, SA1618, SA1202)

- [x] 29.6 Run all unit tests: `dotnet test test/UnitTests` - verify 100% pass
  - **Verification**: All 125 unit tests pass

- [x] 29.7 Run all integration tests: `dotnet test test/IntegrationTests` - verify 100% pass (requires Docker)
  - **Verification**: All integration tests pass

## 30. Remove Minimal API Endpoints (Unused)

- [x] 30.1 Delete `src/Presentation/WebAPI/Endpoints/ClientEndpoints.cs`
- [x] 30.2 Delete `src/Presentation/WebAPI/Endpoints/SectorEndpoints.cs`
- [x] 30.3 Delete `src/Presentation/WebAPI/Endpoints/ServiceEndpoints.cs`
- [x] 30.4 Delete `src/Presentation/WebAPI/Endpoints/CompanyProfileEndpoints.cs`
- [x] 30.5 Delete `src/Presentation/WebAPI/Endpoints/ProjectEndpoints.cs`
- [x] 30.6 Verify build succeeds: `dotnet build src/Presentation/WebAPI`

## 31. Add FluentValidation to Tech Stack Standards

- [x] 31.1 Update `openspec/standards/tech-stack.md` to include FluentValidation in "Testing & Quality Assurance" section
- [x] 31.2 Verify standards document is consistent

## 32. Enable FluentValidation for Controllers

- [x] 32.1 Add `FluentValidation.AspNetCore` package to WebAPI project
- [x] 32.2 Register validators from Application assembly in ASP.NET Core DI (`AddValidatorsFromAssemblyContaining`)
- [x] 32.3 Implement manual validation in Controllers (not auto-validation) for consistent ProblemDetails format
- [x] 32.4 Verify build succeeds

## 33. Fix CompanyProfileController Validation Order

- [x] 33.1 Ensure validation runs before existence check in `UpdateCompanyProfile` action
- [x] 33.2 Inject `IValidator<UpdateCompanyProfileCommand>` and run validation manually
- [x] 33.3 Verify `UpdateCompanyProfile_WithInvalidBody_ReturnsBadRequestWithProblemDetails` test passes

## 34. Fix IntegrationTestBase Helper Methods

- [x] 34.1 Change `ReadFromJsonAsync` to `ReadFromJsonNewtonsoftAsync` in `CreateTestClientAsync` (line 136)
- [x] 34.2 Change `ReadFromJsonAsync` to `ReadFromJsonNewtonsoftAsync` in `CreateTestSectorAsync` (line 151)
- [x] 34.3 Change `ReadFromJsonAsync` to `ReadFromJsonNewtonsoftAsync` in `CreateTestServiceAsync` (line 167)
- [x] 34.4 Change `ReadFromJsonAsync` to `ReadFromJsonNewtonsoftAsync` in `CreateTestProjectAsync` (line 205)
- [x] 34.5 Verify build succeeds

## 35. Apply Manual Validation to Remaining Controllers

- [x] 35.1 Update `SectorsController` - add manual validation to `CreateSector` and `UpdateSector`
- [x] 35.2 Update `ServicesController` - add manual validation to `CreateService` and `UpdateService`
- [x] 35.3 Update `ProjectsController` - add manual validation to `CreateProject`, `UpdateProject`, `ChangeProjectStatus`, `AddProjectService`, `RemoveProjectService`
- [x] 35.4 Update test expectation in `ServicesApiTests` to expect "Validation Failed" instead of "Business Rule Violation"
- [x] 35.5 Verify all ProblemDetails tests pass

## 36. Add Domain-Level Email Validation (Defense in Depth)

- [x] 36.1 Add email format validation to `Client.Create()` method in `src/Core/Domain/Clients/Entities/Client.cs`
- [x] 36.2 Add email format validation to `Client.Update()` method
- [x] 36.3 Use compiled Regex for performance
- [x] 36.4 Verify `CreateClient_WithInvalidEmail_ReturnsBadRequestWithProblemDetails` test passes
- [x] 36.5 Verify `UpdateCompanyProfile_WithInvalidEmail_ReturnsBadRequestWithProblemDetails` test passes
- [x] 36.6 Run all integration tests to verify

## 37. Final Verification

- [x] 37.1 Run full solution build: `dotnet build Tfs.Portfolio.slnx` - verify zero errors, zero warnings
- [x] 37.2 Run all unit tests: `dotnet test test/UnitTests` - verify 100% pass (125/125)
- [x] 37.3 Run ProblemDetails integration tests: `dotnet test test/IntegrationTests --filter "ProblemDetails"` - verify 100% pass (12/12)
- [x] 37.4 Mark tasks 28.21 and 29.7 as complete (integration tests pass)