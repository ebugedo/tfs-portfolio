# Spec Delta

## MODIFIED Requirements

### Requirement: Scalar/OpenAPI provides API documentation
The system SHALL enable `app.MapOpenApi()` and `app.MapScalarApiReference()` for interactive API docs at `/scalar/v1`, available in all environments including production, not only in development.

#### Scenario: API docs are accessible in production
- **WHEN** navigating to `/scalar/v1` in a browser in a production environment
- **THEN** Scalar UI loads with all endpoints documented

#### Scenario: API docs are accessible in development
- **WHEN** navigating to `/scalar/v1` in a browser in a development environment
- **THEN** Scalar UI loads with all endpoints documented
