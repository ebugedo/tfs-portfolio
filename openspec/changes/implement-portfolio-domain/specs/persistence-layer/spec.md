# Spec Delta

## MODIFIED Requirements

### Requirement: Persistence project references Domain, Application, and EF Core only
The system SHALL ensure `Tfs.Portfolio.Infrastructure.Persistence` references `Tfs.Portfolio.Domain`, `Tfs.Portfolio.Application`, `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`—no ASP.NET Core.

#### Scenario: Persistence project references are correct
- **WHEN** inspecting `Tfs.Portfolio.Infrastructure.Persistence.csproj`
- **THEN** references Domain, Application, EF Core, Npgsql—no AspNetCore

### Requirement: DbContext is defined with DbSets for aggregates
The system SHALL provide `ApplicationDbContext : DbContext` with `DbSet<Project>`, `DbSet<Client>`, `DbSet<Sector>`, `DbSet<Service>`, `DbSet<CompanyProfile>` for each aggregate root.

#### Scenario: DbContext exposes aggregate sets
- **WHEN** inspecting `ApplicationDbContext`
- **THEN** it has `public DbSet<Client> Clients { get; set; }`, `public DbSet<Sector> Sectors { get; set; }`, `public DbSet<Service> Services { get; set; }`, `public DbSet<CompanyProfile> CompanyProfiles { get; set; }`, `public DbSet<Project> Projects { get; set; }`

### Requirement: Entity configurations use Fluent API in separate classes
The system SHALL configure each entity using `IEntityTypeConfiguration<T>` in separate classes under `Configurations/`—no Data Annotations on Domain entities.

#### Scenario: Entity configuration is separate from Domain
- **WHEN** inspecting `ClientConfiguration : IEntityTypeConfiguration<Client>`
- **THEN** it configures table name, primary key, indexes, owned types—Domain entity has no attributes

### Requirement: Repositories implement Domain interfaces
The system SHALL implement repository interfaces (e.g., `ClientRepository : IClientRepository`, `SectorRepository : ISectorRepository`, `ServiceRepository : IServiceRepository`, `CompanyProfileRepository : ICompanyProfileRepository`, `ProjectRepository : IProjectRepository`) using EF Core, exposing only Domain types.

#### Scenario: Repository returns Domain entities
- **WHEN** calling `clientRepository.GetByIdAsync(id)`
- **THEN** it returns `Task<Client?>` (Domain entity), not DTO or EF proxy

#### Scenario: ProjectRepository implements extended queries
- **WHEN** calling `projectRepository.GetByClientIdAsync(clientId)`
- **THEN** returns Projects filtered by ClientId using EF Core query

#### Scenario: ICompanyProfileRepository enforces singleton
- **WHEN** calling `companyProfileRepository.GetAsync()`
- **THEN** returns single CompanyProfile or null

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
- **WHEN** querying `context.Clients.ToListAsync()`
- **THEN** only non-deleted clients return; deleted ones require explicit `IgnoreQueryFilters()`

### Requirement: PostgreSQL-specific types are used appropriately
The system SHALL use `Npgsql` types (e.g., `uuid`, `jsonb`, `timestamp with time zone`) via Fluent API for optimal PostgreSQL storage, including correct serialization of nullable value objects to JSONB columns.

#### Scenario: Entity uses PostgreSQL-native types
- **WHEN** inspecting migration for `Client` entity
- **THEN** `Id` is `uuid`, `CreatedAt` is `timestamp with time zone`, `LogoUrl` uses `jsonb`

#### Scenario: Nullable TfsWebUrl serializes correctly to JSONB
- **WHEN** an entity with a nullable `TfsWebUrl` property (e.g., `Client.LogoUrl`, `CompanyProfile.TfsWebUrl`, `CompanyProfile.LogoUrl`) is persisted to a JSONB column
- **THEN** the value is stored as a valid JSON string when present, or `null` when the property is null
- **THEN** no PostgreSQL error `22P02: invalid input syntax for type json` occurs

#### Scenario: Nullable TfsWebUrl deserializes correctly from JSONB
- **WHEN** an entity with a nullable `TfsWebUrl` property is loaded from a JSONB column containing a valid URL string
- **THEN** the property is populated with a valid `TfsWebUrl` instance with `IsValid = true`
- **WHEN** the JSONB column contains `null`
- **THEN** the property is `null` (default for nullable struct)

### Requirement: Persistence layer compiles independently
The system SHALL ensure `dotnet build` on `Tfs.Portfolio.Infrastructure.Persistence` succeeds with Domain and Application references.

#### Scenario: Persistence builds with Domain and Application
- **WHEN** running `dotnet build src/Infrastructure/Persistence/Tfs.Portfolio.Infrastructure.Persistence.csproj`
- **THEN** build succeeds with zero errors

## ADDED Requirements

### Requirement: Client entity configuration
The system SHALL provide ClientConfiguration mapping to "Clients" table with Id (uuid PK), Name (varchar 200 required), LogoUrl (jsonb nullable), Email (varchar 255), Phone (varchar 50), Address (text), IsActive (boolean), CreatedAt/UpdatedAt (timestamp with time zone), index on Name and Email.

#### Scenario: ClientConfiguration defines schema correctly
- **WHEN** inspecting migration for Clients table
- **THEN** columns match specification with correct types and constraints

### Requirement: Sector entity configuration
The system SHALL provide SectorConfiguration mapping to "Sectors" table with Id (uuid PK), Name (varchar 100 required unique), Description (varchar 500), IsActive (boolean), CreatedAt (timestamp with time zone), index on Name.

#### Scenario: SectorConfiguration enforces unique name
- **WHEN** inspecting migration for Sectors table
- **THEN** unique index on Name column exists

### Requirement: Service entity configuration
The system SHALL provide ServiceConfiguration mapping to "Services" table with Id (uuid PK), Name (varchar 150 required), Description (text), Category (integer enum), IsActive (boolean), CreatedAt (timestamp with time zone), index on Category.

### Requirement: CompanyProfile entity configuration (singleton)
The system SHALL provide CompanyProfileConfiguration mapping to "CompanyProfiles" table with Id (uuid PK), CompanyName (varchar 200 required), ContactEmail (varchar 255), ContactPhone (varchar 50), WebUrl (jsonb), Address (text), Description (text), LogoUrl (jsonb), CreatedAt/UpdatedAt (timestamp with time zone), unique constraint ensuring single row.

#### Scenario: CompanyProfileConfiguration enforces singleton
- **WHEN** inspecting migration for CompanyProfiles table
- **THEN** unique constraint or check constraint limits to 1 row

### Requirement: Project entity configuration extended
The system SHALL extend ProjectConfiguration with StartDate (jsonb YearMonth), DurationMonths (integer), Technologies (jsonb array), ClientId (uuid FK to Clients), SectorId (uuid FK to Sectors), Status (integer enum), indexes on ClientId, SectorId, Status.

#### Scenario: ProjectConfiguration defines foreign keys
- **WHEN** inspecting migration for Projects table
- **THEN** FK to Clients.ClientId, FK to Sectors.SectorId with cascade delete restrict

### Requirement: Project-Service many-to-many configuration
The system SHALL provide ProjectServiceConfiguration for join table "ProjectServices" with ProjectId (uuid FK), ServiceId (uuid FK), composite PK, indexes on both FKs.

#### Scenario: ProjectServiceConfiguration defines many-to-many
- **WHEN** inspecting migration for ProjectServices table
- **THEN** composite PK (ProjectId, ServiceId), FKs with cascade delete

### Requirement: PersistenceModule registers all new repositories
The system SHALL extend PersistenceModule to register ClientRepository, SectorRepository, ServiceRepository, CompanyProfileRepository, and updated ProjectRepository.

#### Scenario: All repositories resolvable via Autofac
- **WHEN** PersistenceModule.Load is executed
- **THEN** IClientRepository, ISectorRepository, IServiceRepository, ICompanyProfileRepository registered