# Spec Delta

## MODIFIED Requirements

### Requirement: The system SHALL generate EF Core migrations via `dotnet ef migrations add <Name>` in the Persistence project, committed to source control.
**Unchanged:** Migrations are generated and committed as before.

## ADDED Requirements

### Requirement: Application applies migrations automatically on startup
The system SHALL apply pending EF Core migrations automatically when the API container starts in **all environments** (Development, Production, Testing).

#### Scenario: Migrations applied on container start in Production
- **WHEN** `portfolio-api` container starts in Production
- **THEN** `context.Database.Migrate()` executes and applies pending migrations

#### Scenario: Migrations applied on container start in Development
- **WHEN** API starts in Development (via `docker compose` or `dotnet run`)
- **THEN** `context.Database.Migrate()` executes and applies pending migrations

#### Scenario: Migrations applied on container start in Testing
- **WHEN** integration tests run
- **THEN** `context.Database.Migrate()` executes against test database

#### Scenario: Idempotent migration execution
- **WHEN** `context.Database.Migrate()` runs on database already at latest migration
- **THEN** no error occurs; operation completes successfully