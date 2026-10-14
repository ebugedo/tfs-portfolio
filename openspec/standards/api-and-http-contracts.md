# REST API & HTTP Standards

## Endpoint Design
- Use RESTful conventions or Vertical Slice Endpoints.
- Controller/Endpoint URIs MUST be plural and lowercase (e.g., `/api/v1/orders`).

## Standard HTTP Status Codes
- `200 OK`: Successful read or update.
- `201 Created`: Successful resource creation (MUST include `Location` header).
- `204 No Content`: Successful deletion or action without response body.
- `400 Bad Request`: Validation failure.
- `404 Not Found`: Entity does not exist.
- `500 Internal Server Error`: Unhandled infrastructure errors.

## Response Format
- All API errors MUST return standard `ProblemDetails` (RFC 7807) format.

## Language
- All API field names, error messages, and enum values MUST use English.
- Example: `projectId`, `clientName`, `status`, `createdAt`.