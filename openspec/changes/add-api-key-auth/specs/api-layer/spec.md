# Spec Delta

## MODIFIED Requirements

### Requirement: Scalar/OpenAPI provides API documentation
The system SHALL enable `app.MapOpenApi()` and `app.MapScalarApiReference()` for interactive API docs at `/scalar/v1`, available in all environments including production, not only in development, and SHALL document the `X-API-Key` security requirement on protected endpoints.

#### Scenario: API docs are accessible in production
- **WHEN** navigating to `/scalar/v1` in a browser in a production environment
- **THEN** Scalar UI loads with all endpoints documented

#### Scenario: API docs are accessible in development
- **WHEN** navigating to `/scalar/v1` in a browser in a development environment
- **THEN** Scalar UI loads with all endpoints documented

#### Scenario: Protected endpoints show X-API-Key requirement in Scalar
- **WHEN** viewing a POST/PUT/DELETE/PATCH endpoint in Scalar at `/scalar/v1`
- **THEN** the endpoint indicates an `X-API-Key` header is required

#### Scenario: Public GET endpoints show no X-API-Key requirement in Scalar
- **WHEN** viewing a GET endpoint in Scalar at `/scalar/v1`
- **THEN** no `X-API-Key` header requirement is indicated

### Requirement: Middleware pipeline includes standard concerns
The system SHALL configure middleware in order: Exception handling → Serilog Request Logging → HTTPS Redirection → Routing → API Key Validation → Authorization → Endpoints, where API Key Validation rejects non-GET requests missing a valid `X-API-Key` header with `401 Unauthorized`.

#### Scenario: Exception middleware catches unhandled errors
- **WHEN** an unhandled exception occurs in a controller
- **THEN** it returns 500 ProblemDetails without stack trace in production

#### Scenario: API Key validation rejects POST without X-API-Key header
- **WHEN** a POST request to a protected endpoint is made without an `X-API-Key` header
- **THEN** the response is `401 Unauthorized`

#### Scenario: API Key validation rejects PUT with invalid X-API-Key header
- **WHEN** a PUT request to a protected endpoint is made with an invalid `X-API-Key` header value
- **THEN** the response is `401 Unauthorized`

#### Scenario: API Key validation allows GET without X-API-Key header
- **WHEN** a GET request to any endpoint is made without an `X-API-Key` header
- **THEN** the request proceeds normally (GET endpoints are public)

## ADDED Requirements

### Requirement: API Key authentication protects write endpoints
The system SHALL require a valid `X-API-Key` header on all POST, PUT, DELETE, and PATCH requests to protected endpoints under `/api/v1/`, and SHALL allow GET requests without authentication.

#### Scenario: POST endpoint requires valid X-API-Key header
- **WHEN** a POST request to `/api/v1/projects` is made with a valid `X-API-Key` header
- **THEN** the request proceeds to the controller handler

#### Scenario: POST endpoint rejects missing X-API-Key header
- **WHEN** a POST request to `/api/v1/projects` is made without an `X-API-Key` header
- **THEN** the response is `401 Unauthorized`

#### Scenario: POST endpoint rejects incorrect X-API-Key header
- **WHEN** a POST request to `/api/v1/projects` is made with an incorrect `X-API-Key` header value
- **THEN** the response is `401 Unauthorized`

#### Scenario: DELETE endpoint requires valid X-API-Key header
- **WHEN** a DELETE request to `/api/v1/projects/{id}` is made with a valid `X-API-Key` header
- **THEN** the request proceeds to the controller handler

#### Scenario: PATCH endpoint requires valid X-API-Key header
- **WHEN** a PATCH request to `/api/v1/projects/{id}/status` is made with a valid `X-API-Key` header
- **THEN** the request proceeds to the controller handler

#### Scenario: GET endpoint is public without X-API-Key header
- **WHEN** a GET request to `/api/v1/projects` is made without an `X-API-Key` header
- **THEN** the response is `200 OK` with the list of projects
