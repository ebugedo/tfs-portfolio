# Spec Delta

## Purpose

Manages the singleton company profile containing company-wide settings: name, contact information, web presence, and branding.

## ADDED Requirements

### Requirement: CompanyProfile singleton aggregate root exists
The system SHALL provide a CompanyProfile aggregate root with Id (Guid), CompanyName, ContactEmail, ContactPhone, WebUrl (Url value object), Address, Description, LogoUrl (Url value object), CreatedAt, and UpdatedAt. Only ONE instance SHALL exist in the system.

#### Scenario: CompanyProfile created with valid data
- **WHEN** a CompanyProfile is created with valid CompanyName, ContactEmail, and optional WebUrl
- **THEN** the aggregate is instantiated with a new Guid Id, CreatedAt set to current UTC time

#### Scenario: CompanyProfile creation fails when instance already exists
- **WHEN** a second CompanyProfile creation is attempted
- **THEN** a DomainException is thrown with message "Company profile already exists"

#### Scenario: CompanyProfile creation fails with empty company name
- **WHEN** a CompanyProfile is created with empty or whitespace CompanyName
- **THEN** a DomainException is thrown with message "Company name cannot be empty"

#### Scenario: CompanyProfile creation fails with invalid web URL
- **WHEN** a CompanyProfile is created with invalid WebUrl format
- **THEN** a DomainException is thrown with message "Invalid web URL format"

#### Scenario: CompanyProfile creation fails with invalid contact email
- **WHEN** a CompanyProfile is created with invalid ContactEmail format
- **THEN** a DomainException is thrown with message "Invalid contact email format"

### Requirement: CompanyProfile can be updated
The system SHALL allow updating all properties except Id, setting UpdatedAt to current UTC time.

#### Scenario: CompanyProfile updated successfully
- **WHEN** CompanyProfile.Update is called with new CompanyName and WebUrl
- **THEN** properties are changed and UpdatedAt is set to current UTC time

#### Scenario: CompanyProfile update fails with empty company name
- **WHEN** CompanyProfile.Update is called with empty CompanyName
- **THEN** a DomainException is thrown with message "Company name cannot be empty"

### Requirement: CompanyProfile repository interface enforces singleton
The system SHALL provide an ICompanyProfileRepository interface with methods: GetAsync (returns the single instance or null), AddAsync (throws if instance exists), Update, and Delete.

#### Scenario: Repository returns singleton instance
- **WHEN** GetAsync is called after CompanyProfile is created
- **THEN** returns the single CompanyProfile instance

#### Scenario: Repository prevents duplicate creation
- **WHEN** AddAsync is called when instance already exists
- **THEN** throws DomainException with message "Company profile already exists"

### Requirement: Domain event raised for company profile updates
The system SHALL raise CompanyProfileUpdatedEvent when the profile is updated.

#### Scenario: CompanyProfileUpdatedEvent raised on update
- **WHEN** CompanyProfile.Update is called successfully
- **THEN** CompanyProfileUpdatedEvent with ProfileId is added to aggregate's events