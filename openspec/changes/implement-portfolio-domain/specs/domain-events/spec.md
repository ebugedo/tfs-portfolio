# Spec Delta

## Purpose

Defines additional domain events for the new aggregates (Client, Sector, Service, CompanyProfile) and enhanced Project events (status changes, updates).

## ADDED Requirements

### Requirement: Client domain events
The system SHALL define ClientCreatedEvent, ClientUpdatedEvent, and ClientDeactivatedEvent as record types implementing IDomainEvent.

#### Scenario: ClientCreatedEvent contains client identity
- **WHEN** ClientCreatedEvent is instantiated with ClientId and Name
- **THEN** EventId is generated, OccurredOn is set to UTC now, ClientId and Name are accessible

#### Scenario: ClientUpdatedEvent contains client identity
- **WHEN** ClientUpdatedEvent is instantiated with ClientId
- **THEN** EventId is generated, OccurredOn is set to UTC now, ClientId is accessible

#### Scenario: ClientDeactivatedEvent contains client identity
- **WHEN** ClientDeactivatedEvent is instantiated with ClientId
- **THEN** EventId is generated, OccurredOn is set to UTC now, ClientId is accessible

### Requirement: Sector domain events
The system SHALL define SectorCreatedEvent and SectorUpdatedEvent as record types implementing IDomainEvent.

#### Scenario: SectorCreatedEvent contains sector identity
- **WHEN** SectorCreatedEvent is instantiated with SectorId and Name
- **THEN** EventId is generated, OccurredOn is set to UTC now, SectorId and Name are accessible

#### Scenario: SectorUpdatedEvent contains sector identity
- **WHEN** SectorUpdatedEvent is instantiated with SectorId
- **THEN** EventId is generated, OccurredOn is set to UTC now, SectorId is accessible

### Requirement: Service domain events
The system SHALL define ServiceCreatedEvent and ServiceUpdatedEvent as record types implementing IDomainEvent.

#### Scenario: ServiceCreatedEvent contains service identity
- **WHEN** ServiceCreatedEvent is instantiated with ServiceId and Name
- **THEN** EventId is generated, OccurredOn is set to UTC now, ServiceId and Name are accessible

#### Scenario: ServiceUpdatedEvent contains service identity
- **WHEN** ServiceUpdatedEvent is instantiated with ServiceId
- **THEN** EventId is generated, OccurredOn is set to UTC now, ServiceId is accessible

### Requirement: CompanyProfile domain event
The system SHALL define CompanyProfileUpdatedEvent as a record type implementing IDomainEvent.

#### Scenario: CompanyProfileUpdatedEvent contains profile identity
- **WHEN** CompanyProfileUpdatedEvent is instantiated with ProfileId
- **THEN** EventId is generated, OccurredOn is set to UTC now, ProfileId is accessible

### Requirement: Project status changed event
The system SHALL define ProjectStatusChangedEvent as a record type implementing IDomainEvent with ProjectId, OldStatus, NewStatus.

#### Scenario: ProjectStatusChangedEvent contains transition details
- **WHEN** ProjectStatusChangedEvent is instantiated with ProjectId, OldStatus, NewStatus
- **THEN** EventId is generated, OccurredOn is set to UTC now, all properties are accessible

### Requirement: Project updated event
The system SHALL define ProjectUpdatedEvent as a record type implementing IDomainEvent with ProjectId.

#### Scenario: ProjectUpdatedEvent contains project identity
- **WHEN** ProjectUpdatedEvent is instantiated with ProjectId
- **THEN** EventId is generated, OccurredOn is set to UTC now, ProjectId is accessible