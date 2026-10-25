# Spec Delta

## Modified Requirements

### Requirement: PostgreSQL-specific types are used appropriately
The system SHALL use `Npgsql` types (e.g., `uuid`, `jsonb`, `timestamp with time zone`) via Fluent API for optimal PostgreSQL storage, including correct serialization of nullable value objects to JSONB columns.

#### Scenario: Nullable TfsWebUrl serializes correctly to JSONB (existing - unchanged)
- **WHEN** an entity with a nullable `TfsWebUrl` property (e.g., `Client.LogoUrl`, `CompanyProfile.TfsWebUrl`, `CompanyProfile.LogoUrl`) is persisted to a JSONB column
- **THEN** the value is stored as a valid JSON string when present, or `null` when the property is null
- **THEN** no PostgreSQL error `22P02: invalid input syntax for type json` occurs

#### Scenario: Nullable TfsWebUrl deserializes correctly from JSONB (MODIFIED)
- **WHEN** an entity with a nullable `TfsWebUrl` property is loaded from a JSONB column containing a valid URL string
- **THEN** the property is populated with a valid `TfsWebUrl` instance with `IsValid = true`
- **WHEN** the JSONB column contains a JSON object `{"value": "...", "isValid": true}`
- **THEN** the property is populated with a valid `TfsWebUrl` instance with `Value` set to the stored URL string
- **WHEN** the JSONB column contains a JSON object `{"value": null, "isValid": false}`
- **THEN** the property is `null`

## MODIFIED Requirements

### Requirement: PostgreSQL-specific types are used appropriately
The system SHALL use `Npgsql` types (e.g., `uuid`, `jsonb`, `timestamp with time zone`) via Fluent API for optimal PostgreSQL storage, including correct serialization of nullable value objects to JSONB columns.

#### Scenario: Nullable TfsWebUrl deserializes correctly from JSONB object format (MODIFIED)
- **WHEN** an entity with a nullable `TfsWebUrl` property is loaded from a JSONB column containing a JSON object `{"value": "https://example.com", "isValid": true}`
- **THEN** the property is populated with a valid `TfsWebUrl` instance with `Value` = `"https://example.com"` and `IsValid = true`
- **WHEN** the JSONB column contains a JSON object `{"value": null, "isValid": false}`
- **THEN** the property is `null`
