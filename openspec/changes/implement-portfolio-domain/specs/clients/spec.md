# Spec Delta

## Purpose

Manages clients of the software consulting company including their identity, contact information, logo, and active status.

## ADDED Requirements

### Requirement: Client aggregate root exists with identity and contact details
The system SHALL provide a Client aggregate root with Id (Guid), Name (required, max 200), LogoUrl (Url value object), Email, Phone, Address, IsActive, CreatedAt, and UpdatedAt.

#### Scenario: Client created with valid data
- **WHEN** a Client is created with valid Name, Email, and optional LogoUrl
- **THEN** the Client aggregate is instantiated with a new Guid Id, IsActive true, CreatedAt set to current UTC time

#### Scenario: Client creation fails with empty name
- **WHEN** a Client is created with empty or whitespace Name
- **THEN** a DomainException is thrown with message "Client name cannot be empty"

#### Scenario: Client creation fails with invalid email format
- **WHEN** a Client is created with invalid Email format
- **THEN** a DomainException is thrown with message "Invalid email format"

#### Scenario: Client creation fails with invalid logo URL
- **WHEN** a Client is created with invalid LogoUrl format
- **THEN** a DomainException is thrown with message "Invalid logo URL format"

### Requirement: Client can be updated
The system SHALL allow updating a Client's Name, LogoUrl, Email, Phone, Address, and IsActive status, setting UpdatedAt to current UTC time.

#### Scenario: Client updated successfully
- **WHEN** a Client's Name, Email, and Phone are updated
- **THEN** the properties are changed and UpdatedAt is set to current UTC time

#### Scenario: Client update fails with empty name
- **WHEN** a Client's Name is updated to empty or whitespace
- **THEN** a DomainException is thrown with message "Client name cannot be empty"

### Requirement: Client can be deactivated
The system SHALL allow deactivating a Client by setting IsActive to false.

#### Scenario: Client deactivated
- **WHEN** a Client's IsActive is set to false
- **THEN** the Client is marked inactive and UpdatedAt is set to current UTC time

### Requirement: Client repository interface exists
The system SHALL provide an IClientRepository interface with methods: GetByIdAsync, GetAllAsync, GetActiveAsync, AddAsync, Update, and Delete, returning only Domain types.

#### Scenario: Repository returns Client aggregate
- **WHEN** GetByIdAsync is called with an existing Id
- **THEN** returns Task<Client?> with the aggregate

#### Scenario: Repository filters active clients
- **WHEN** GetActiveAsync is called
- **THEN** returns only Clients with IsActive true

### Requirement: Domain events are raised for client lifecycle
The system SHALL raise ClientCreatedEvent when a Client is created, ClientUpdatedEvent when updated, and ClientDeactivatedEvent when deactivated.

#### Scenario: ClientCreatedEvent raised on creation
- **WHEN** Client.Create is called successfully
- **THEN** ClientCreatedEvent with ClientId and Name is added to aggregate's events

#### Scenario: ClientUpdatedEvent raised on update
- **WHEN** Client.Update is called successfully
- **THEN** ClientUpdatedEvent with ClientId is added to aggregate's events

#### Scenario: ClientDeactivatedEvent raised on deactivation
- **WHEN** Client.IsActive is set to false
- **THEN** ClientDeactivatedEvent with ClientId is added to aggregate's events