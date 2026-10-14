# Spec Delta

## Purpose

Manages the catalog of services offered by the software consulting company.

## ADDED Requirements

### Requirement: Service aggregate root exists with identity, description, and category
The system SHALL provide a Service aggregate root with Id (Guid), Name (required, max 150), Description (max 1000), Category (enum: Development/Consulting/Design/DevOps/Training), IsActive, and CreatedAt.

#### Scenario: Service created with valid data
- **WHEN** a Service is created with valid Name, Description, and Category
- **THEN** the Service aggregate is instantiated with a new Guid Id, IsActive true, CreatedAt set to current UTC time

#### Scenario: Service creation fails with empty name
- **WHEN** a Service is created with empty or whitespace Name
- **THEN** a DomainException is thrown with message "Service name cannot be empty"

#### Scenario: Service creation fails with invalid category
- **WHEN** a Service is created with an undefined Category value
- **THEN** a DomainException is thrown with message "Invalid service category"

### Requirement: Service can be updated
The system SHALL allow updating a Service's Name, Description, Category, and IsActive status.

#### Scenario: Service updated successfully
- **WHEN** a Service's Name and Category are updated
- **THEN** the properties are changed

#### Scenario: Service update fails with empty name
- **WHEN** a Service's Name is updated to empty or whitespace
- **THEN** a DomainException is thrown with message "Service name cannot be empty"

### Requirement: Service repository interface exists
The system SHALL provide an IServiceRepository interface with methods: GetByIdAsync, GetAllAsync, GetActiveAsync, GetByCategoryAsync, AddAsync, Update, and Delete, returning only Domain types.

#### Scenario: Repository returns Service aggregate
- **WHEN** GetByIdAsync is called with an existing Id
- **THEN** returns Task<Service?> with the aggregate

#### Scenario: Repository filters services by category
- **WHEN** GetByCategoryAsync is called with a ServiceCategory
- **THEN** returns only Services with that category

#### Scenario: Repository filters active services
- **WHEN** GetActiveAsync is called
- **THEN** returns only Services with IsActive true

### Requirement: Domain events are raised for service lifecycle
The system SHALL raise ServiceCreatedEvent when a Service is created and ServiceUpdatedEvent when updated.

#### Scenario: ServiceCreatedEvent raised on creation
- **WHEN** Service.Create is called successfully
- **THEN** ServiceCreatedEvent with ServiceId and Name is added to aggregate's events

#### Scenario: ServiceUpdatedEvent raised on update
- **WHEN** Service.Update is called successfully
- **THEN** ServiceUpdatedEvent with ServiceId is added to aggregate's events