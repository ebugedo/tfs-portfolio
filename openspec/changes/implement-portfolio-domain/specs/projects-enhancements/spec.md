# Spec Delta

## Purpose

Enhances the existing Project aggregate with timeline (start date, duration), technologies, client/sector relationships, status workflow, and project-service associations.

## ADDED Requirements

### Requirement: Project has timeline with YearMonth start date and duration in months
The system SHALL extend Project with StartDate (YearMonth value object: month 1-12, year), DurationMonths (integer 1-120), and computed EndDate property.

#### Scenario: Project created with valid timeline
- **WHEN** a Project is created with StartDate (month 6, year 2024) and DurationMonths 6
- **THEN** StartDate, DurationMonths are set, EndDate computes to month 12, year 2024

#### Scenario: Project creation fails with invalid duration
- **WHEN** a Project is created with DurationMonths less than 1 or greater than 120
- **THEN** a DomainException is thrown with message "Duration must be between 1 and 120 months"

#### Scenario: Project creation fails with invalid start date
- **WHEN** a Project is created with StartDate month less than 1 or greater than 12
- **THEN** a DomainException is thrown with message "Invalid start month"

### Requirement: Project has technologies as a collection of Technology value objects
The system SHALL provide a Technologies collection on Project where each Technology has Name, Category (Frontend/Backend/Database/Cloud/Tools), and Proficiency (Beginner/Intermediate/Expert).

#### Scenario: Project technologies can be added
- **WHEN** a Technology is added to a Project's Technologies collection
- **THEN** the technology is included in the collection

#### Scenario: Duplicate technology names are not allowed
- **WHEN** a Technology with the same Name (case-insensitive) is added to a Project
- **THEN** a DomainException is thrown with message "Technology already exists in project"

### Requirement: Project belongs to a Client and Sector
The system SHALL require ClientId (Guid) and SectorId (Guid) on Project creation, with validation that both exist.

#### Scenario: Project created with valid ClientId and SectorId
- **WHEN** a Project is created with existing ClientId and SectorId
- **THEN** the Project is associated with the Client and Sector

#### Scenario: Project creation fails with non-existent ClientId
- **WHEN** a Project is created with a ClientId that does not exist
- **THEN** a DomainException is thrown with message "Client not found"

#### Scenario: Project creation fails with non-existent SectorId
- **WHEN** a Project is created with a SectorId that does not exist
- **THEN** a DomainException is thrown with message "Sector not found"

### Requirement: Project has a status workflow
The system SHALL provide a Status enum on Project with values: Draft, Active, OnHold, Completed, Cancelled. Transitions are restricted: Draft→Active, Active→OnHold/Completed/Cancelled, OnHold→Active/Cancelled, Completed→(none), Cancelled→(none).

#### Scenario: Valid status transition Draft to Active
- **WHEN** a Draft Project's status is changed to Active
- **THEN** status changes to Active and ProjectStatusChangedEvent is raised

#### Scenario: Invalid status transition Active to Draft
- **WHEN** an Active Project's status is changed to Draft
- **THEN** a DomainException is thrown with message "Invalid status transition from Active to Draft"

### Requirement: Project has many-to-many relationship with Services
The system SHALL allow associating multiple Services with a Project through a join collection, with methods to add/remove services.

#### Scenario: Service added to project
- **WHEN** a Service is added to a Project's Services collection
- **THEN** the association is created

#### Scenario: Duplicate service association prevented
- **WHEN** the same Service is added twice to a Project
- **THEN** a DomainException is thrown with message "Service already associated with project"

### Requirement: Extended project repository queries
The system SHALL extend IProjectRepository with GetByClientIdAsync, GetBySectorIdAsync, GetByTechnologyAsync, GetByStatusAsync, and GetActiveAsync methods.

#### Scenario: Repository filters projects by client
- **WHEN** GetByClientIdAsync is called with a valid ClientId
- **THEN** returns only Projects belonging to that Client

#### Scenario: Repository filters projects by technology
- **WHEN** GetByTechnologyAsync is called with a technology name
- **THEN** returns only Projects containing that technology

#### Scenario: Repository filters projects by status
- **WHEN** GetByStatusAsync is called with a ProjectStatus
- **THEN** returns only Projects with that status

### Requirement: Domain events for project enhancements
The system SHALL raise ProjectUpdatedEvent when core properties change, and ProjectStatusChangedEvent when status transitions occur.

#### Scenario: ProjectUpdatedEvent raised on property update
- **WHEN** Project.Update is called with new Name or Description
- **THEN** ProjectUpdatedEvent with ProjectId is added to events

#### Scenario: ProjectStatusChangedEvent raised on status change
- **WHEN** Project.ChangeStatus is called with a valid new status
- **THEN** ProjectStatusChangedEvent with ProjectId, OldStatus, NewStatus is added to events