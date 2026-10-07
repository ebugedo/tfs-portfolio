# Proposal

## Why

The portfolio system needs a complete domain model to represent a software consulting company's business: Clients, Sectors, Projects (with timeline, technologies, and relationships), Services, and a Company Profile. Currently only a basic Project aggregate exists as a reference implementation. This change establishes the full domain foundation required for all future portfolio management features.

## What Changes

- Create **Client** aggregate root with logo URL, contact info, and active status
- Create **Sector** aggregate root for categorizing projects
- Enhance **Project** aggregate: add start date (YearMonth), duration, technologies, client/sector relationships, status enum, and project-service many-to-many
- Create **Service** aggregate root with category enum
- Create **CompanyProfile** singleton aggregate for company-wide settings
- Add Value Objects: **YearMonth**, **Technology**, **Url**, **ContactInfo**
- Add Domain Events for all aggregates
- Implement full CRUD API endpoints under `/api/v1/` for all aggregates
- Configure EF Core persistence with Fluent API, JSONB for technologies, join table for project-service
- Add unit and integration tests for all new domain logic and API endpoints

## Capabilities

### New Capabilities
- `clients`: Client aggregate, repository, commands/queries, API endpoints, persistence
- `sectors`: Sector aggregate, repository, commands/queries, API endpoints, persistence
- `projects/enhancements`: Project aggregate enhancements (start date, duration, technologies, relationships, status), extended repository queries, API filters
- `services`: Service aggregate, repository, commands/queries, API endpoints, persistence
- `company-profile`: CompanyProfile singleton aggregate, repository, commands/queries, API endpoint, persistence
- `value-objects`: Shared value objects (YearMonth, Technology, Url, ContactInfo) used across aggregates
- `domain-events`: Additional domain events for new aggregates and project status changes

### Modified Capabilities
- `domain-layer`: Extends existing domain-layer spec with new aggregates, value objects, and events
- `application-layer`: Extends with new commands, queries, handlers, DTOs, validators, and mappers for all new capabilities
- `persistence-layer`: Extends with new entity configurations, repositories, and migration for new tables/columns
- `api-layer`: Extends with new Minimal API endpoint groups for clients, sectors, services, company-profile
- `testing-foundation`: Extends with unit and integration tests for all new capabilities

## Impact

- **Code**: New domain entities (~8 files), value objects (~4 files), repositories (~4), application commands/queries/handlers (~30), DTOs, validators, mappers, API endpoints (~5 endpoint groups), EF configurations, migration
- **APIs**: New RESTful endpoints at `/api/v1/clients`, `/api/v1/sectors`, `/api/v1/projects` (extended), `/api/v1/services`, `/api/v1/company-profile`
- **Dependencies**: No new NuGet packages - uses existing stack (AutoMapper, FluentValidation, EF Core, Npgsql, Testcontainers 4.13.0)
- **Database**: New tables `Clients`, `Sectors`, `Services`, `CompanyProfiles`, `ProjectServices` (join), extended `Projects` table with new columns and FKs
- **Infrastructure**: No changes to Docker, CI/CD, or deployment configuration