# Spec Delta

## Purpose

Manages industry sectors for categorizing projects in the portfolio system.

## ADDED Requirements

### Requirement: Sector aggregate root exists with identity and description
The system SHALL provide a Sector aggregate root with Id (Guid), Name (required, max 100), Description (max 500), IsActive, and CreatedAt.

#### Scenario: Sector created with valid data
- **WHEN** a Sector is created with valid Name and optional Description
- **THEN** the Sector aggregate is instantiated with a new Guid Id, IsActive true, CreatedAt set to current UTC time

#### Scenario: Sector creation fails with empty name
- **WHEN** a Sector is created with empty or whitespace Name
- **THEN** a DomainException is thrown with message "Sector name cannot be empty"

#### Scenario: Sector creation fails with name exceeding max length
- **WHEN** a Sector is created with Name longer than 100 characters
- **THEN** a DomainException is thrown with message "Sector name must not exceed 100 characters"

### Requirement: Sector can be updated
The system SHALL allow updating a Sector's Name, Description, and IsActive status.

#### Scenario: Sector updated successfully
- **WHEN** a Sector's Name and Description are updated
- **THEN** the properties are changed

#### Scenario: Sector update fails with empty name
- **WHEN** a Sector's Name is updated to empty or whitespace
- **THEN** a DomainException is thrown with message "Sector name cannot be empty"

### Requirement: Sector repository interface exists
The system SHALL provide an ISectorRepository interface with methods: GetByIdAsync, GetAllAsync, GetActiveAsync, AddAsync, Update, and Delete, returning only Domain types.

#### Scenario: Repository returns Sector aggregate
- **WHEN** GetByIdAsync is called with an existing Id
- **THEN** returns Task<Sector?> with the aggregate

#### Scenario: Repository filters active sectors
- **WHEN** GetActiveAsync is called
- **THEN** returns only Sectors with IsActive true

### Requirement: Domain events are raised for sector lifecycle
The system SHALL raise SectorCreatedEvent when a Sector is created and SectorUpdatedEvent when updated.

#### Scenario: SectorCreatedEvent raised on creation
- **WHEN** Sector.Create is called successfully
- **THEN** SectorCreatedEvent with SectorId and Name is added to aggregate's events

#### Scenario: SectorUpdatedEvent raised on update
- **WHEN** Sector.Update is called successfully
- **THEN** SectorUpdatedEvent with SectorId is added to aggregate's events