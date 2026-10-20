# Tfs.Portfolio — API Consumer Guide

## Getting Started

### Base URL

- Development: `http://localhost:8082`
- Production: `https://<your-domain>` (configured at deployment)

### Interactive Documentation

Scalar/OpenAPI UI is available at `/scalar/v1` on the API host. It provides:
- Full endpoint catalog with request/response schemas
- Try-it-out functionality
- Security scheme documentation for protected endpoints

Example: `http://localhost:8082/scalar/v1`

## Authentication

### API Key

Write operations (POST, PUT, DELETE, PATCH) require a valid `X-API-Key` header. Read operations (GET) are public.

**Header:**
```
X-API-Key: <your-api-key>
```

**Configuration:**
- Development: set via `dotnet user-secrets` (`ApiKeySettings:MasterKey`) or `appsettings.Development.json`
- Production: injected as environment variable `ApiKeySettings__MasterKey`

**Example request with API Key:**
```http
POST /api/v1/clients
Content-Type: application/json
X-API-Key: <your-api-key>

{
  "name": "Acme Corporation",
  "email": "contact@acme.com",
  "isActive": true
}
```

**Missing or invalid key:** `401 Unauthorized`

## Endpoints

All endpoints are under `/api/v1/`. JSON responses use camelCase property names.

### Clients

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/clients` | List all clients | Public |
| GET | `/api/v1/clients/{id}` | Get client by ID | Public |
| POST | `/api/v1/clients` | Create a client | API Key |
| PUT | `/api/v1/clients/{id}` | Update a client | API Key |
| DELETE | `/api/v1/clients/{id}` | Delete a client | API Key |

**Create client request:**
```http
POST /api/v1/clients
Content-Type: application/json
X-API-Key: <your-api-key>

{
  "name": "Acme Corporation",
  "email": "contact@acme.com",
  "isActive": true
}
```

**Create client response (201):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Acme Corporation",
  "email": "contact@acme.com",
  "isActive": true,
  "createdAt": "2026-01-15T10:30:00Z"
}
```

### Sectors

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/sectors` | List all sectors | Public |
| GET | `/api/v1/sectors/{id}` | Get sector by ID | Public |
| POST | `/api/v1/sectors` | Create a sector | API Key |
| PUT | `/api/v1/sectors/{id}` | Update a sector | API Key |
| DELETE | `/api/v1/sectors/{id}` | Delete a sector | API Key |

### Services

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/services` | List all services | Public |
| GET | `/api/v1/services/{id}` | Get service by ID | Public |
| GET | `/api/v1/services/category/{category}` | Filter services by category | Public |
| POST | `/api/v1/services` | Create a service | API Key |
| PUT | `/api/v1/services/{id}` | Update a service | API Key |
| DELETE | `/api/v1/services/{id}` | Delete a service | API Key |

### Company Profile (singleton)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/company-profile` | Get company profile | Public |
| PUT | `/api/v1/company-profile` | Update company profile | API Key |

### Projects

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/projects` | List projects with optional filters | Public |
| POST | `/api/v1/projects` | Create a project | API Key |
| PUT | `/api/v1/projects/{id}` | Update a project | API Key |
| PATCH | `/api/v1/projects/{id}/status` | Change project status | API Key |
| POST | `/api/v1/projects/{id}/services/{serviceId}` | Associate a service | API Key |
| DELETE | `/api/v1/projects/{id}/services/{serviceId}` | Remove a service association | API Key |

**Project filters (query parameters):**
- `clientId` — filter by client GUID
- `sectorId` — filter by sector GUID
- `technology` — filter by technology string
- `status` — filter by project status enum

**Create project request:**
```http
POST /api/v1/projects
Content-Type: application/json
X-API-Key: <your-api-key>

{
  "startDate": "2026-02-01",
  "durationMonths": 12,
  "clientId": "550e8400-e29b-41d4-a716-446655440000",
  "sectorId": "660e8400-e29b-41d4-a716-446655440000",
  "technologies": [".NET", "PostgreSQL"]
}
```

## Error Responses

All errors use [RFC 7807 ProblemDetails](https://datatracker.ietf.org/doc/html/rfc7807) format:

```json
{
  "type": "https://api.tfs.portfolio/problems/validation-error",
  "title": "One or more validation errors occurred",
  "status": 400,
  "detail": "The Name field is required.",
  "instance": "/api/v1/clients"
}
```

| Status | Meaning |
|--------|---------|
| 400 | Bad Request — validation error or invalid input |
| 401 | Unauthorized — missing or invalid API Key |
| 404 | Not Found — resource does not exist |
| 500 | Internal Server Error — unexpected failure |

## Health Checks

| Endpoint | Description |
|----------|-------------|
| GET `/health/live` | Liveness probe |
| GET `/health/ready` | Readiness probe (includes database check) |

## Versioning

API versioning is route-based using `MapGroup("api/v1")`. Future versions would use `MapGroup("api/v2")`.

## JSON Serialization

- Contract resolver: camelCase
- Example: `projectId`, `projectName`, `createdAt`
