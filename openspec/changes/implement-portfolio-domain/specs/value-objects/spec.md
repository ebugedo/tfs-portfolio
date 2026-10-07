# Spec Delta

## Purpose

Provides shared immutable value objects used across multiple domain aggregates: YearMonth for project timelines, Technology for project tech stacks, Url for validated links, and ContactInfo for contact details.

## ADDED Requirements

### Requirement: YearMonth value object represents month and year only
The system SHALL provide a YearMonth readonly record struct with Month (1-12), Year (e.g., 2024), factory methods Create(month, year) and FromDateTime(DateTime), comparison operators, and ToString returning "MM/yyyy".

#### Scenario: YearMonth created with valid values
- **WHEN** YearMonth.Create(6, 2024) is called
- **THEN** returns YearMonth with Month=6, Year=2024

#### Scenario: YearMonth creation fails with invalid month
- **WHEN** YearMonth.Create(13, 2024) is called
- **THEN** throws ArgumentException with message "Month must be between 1 and 12"

#### Scenario: YearMonth comparison works correctly
- **WHEN** two YearMonth instances with same Month and Year are compared
- **THEN** they are equal; different values are not equal

#### Scenario: YearMonth ordering works chronologically
- **WHEN** YearMonth.Create(1, 2024) is compared to YearMonth.Create(6, 2024)
- **THEN** the first is less than the second

#### Scenario: YearMonth EndDate calculation
- **WHEN** YearMonth.Create(11, 2024).AddMonths(3) is called
- **THEN** returns YearMonth with Month=2, Year=2025

### Requirement: Technology value object represents a technology with category and proficiency
The system SHALL provide a Technology readonly record struct with Name, Category (enum: Frontend/Backend/Database/Cloud/Tools), Proficiency (enum: Beginner/Intermediate/Expert), and equality by Name (case-insensitive).

#### Scenario: Technology created with valid values
- **WHEN** Technology.Create("React", TechnologyCategory.Frontend, Proficiency.Expert) is called
- **THEN** returns Technology with Name="React", Category=Frontend, Proficiency=Expert

#### Scenario: Technology equality is case-insensitive on name
- **WHEN** two Technology instances with names "React" and "react" are compared
- **THEN** they are equal

### Requirement: Url value object validates URI format
The system SHALL provide a Url readonly record struct with Value (string), implicit conversion from string, validation on creation, and IsValid property.

#### Scenario: Url created with valid URI
- **WHEN** Url.Create("https://example.com/logo.png") is called
- **THEN** returns Url with Value="https://example.com/logo.png", IsValid=true

#### Scenario: Url creation fails with invalid URI
- **WHEN** Url.Create("not-a-url") is called
- **THEN** throws ArgumentException with message "Invalid URL format"

#### Scenario: Url handles null/empty gracefully
- **WHEN** Url.Create(null) or Url.Create("") is called
- **THEN** returns Url with Value=null, IsValid=false (nullable Url support)

### Requirement: ContactInfo value object groups contact details
The system SHALL provide a ContactInfo readonly record struct with Email, Phone, Address, and validation on Email format.

#### Scenario: ContactInfo created with valid email
- **WHEN** ContactInfo.Create("contact@company.com", "+1234567890", "123 Street") is called
- **THEN** returns ContactInfo with Email, Phone, Address

#### Scenario: ContactInfo creation fails with invalid email
- **WHEN** ContactInfo.Create("invalid-email", "phone", "address") is called
- **THEN** throws ArgumentException with message "Invalid email format"