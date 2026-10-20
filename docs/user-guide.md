# Tfs.Portfolio — User Guide

## Overview

Tfs.Portfolio is a Portfolio Management API built with .NET 10, ASP.NET Core, Entity Framework Core, and PostgreSQL. It manages clients, sectors, services, projects, and company profile information through a RESTful HTTP interface.

## Domain

The system models a portfolio management domain with the following core concepts:

- **Clients** — Individuals or organizations that own projects. Each client has a name, email, and active status.
- **Sectors** — Industry or business sectors used to categorize projects (e.g., Technology, Healthcare, Finance).
- **Services** — Offerings that can be associated with projects. Each service has a name, category, and active status.
- **Projects** — Portfolio items linking a client, sector, and set of services. Projects have start dates, durations, technologies, and status.
- **Company Profile** — A singleton record describing the organization running the portfolio (name, description, contact details).

## Objectives

- Provide a REST API for managing portfolio data (clients, sectors, services, projects, company profile).
- Protect write operations (POST, PUT, DELETE, PATCH) with API Key authentication while keeping read operations (GET) publicly accessible.
- Serve interactive API documentation via Scalar/OpenAPI at `/scalar/v1`.
- Support deployment on a VPS with Docker and Nginx, with CI/CD via GitHub Actions.

## High-Level Architecture

The system follows a clean layered architecture:

1. **Presentation Layer** — ASP.NET Core Web API with controllers, middleware, and OpenAPI/Scalar documentation.
2. **Application Layer** — CQRS commands/queries, handlers, validators, and DTOs (using MediatR).
3. **Infrastructure Layer** — Entity Framework Core persistence, repositories, and database migrations.
4. **Domain Layer** — Core business logic, entities, value objects, and domain events.

Layers communicate through dependency injection (Autofac). The API project references Application and Persistence only; Domain is accessed indirectly through Application.

## Setup Instructions

### Prerequisites

- .NET 10 SDK
- Docker & Docker Compose
- PostgreSQL (provided via Docker Compose)

### Quick Start

```bash
# Clone the repository
git clone <repository-url>
cd tfs-portfolio

# Start development environment (API + PostgreSQL)
docker compose up -d
```

The API is available at `http://localhost:8082`. Scalar interactive documentation is at `http://localhost:8082/scalar/v1`.

### Running Tests

```bash
# Unit tests
dotnet test test/UnitTests/Tfs.Portfolio.UnitTests.csproj

# Integration tests (requires PostgreSQL on localhost:5432)
dotnet test test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj
```

## Technology Stack

| Layer | Technology |
|-------|------------|
| Runtime | .NET 10 |
| Web Framework | ASP.NET Core Web API |
| DI Container | Autofac |
| ORM | Entity Framework Core 9 + Npgsql |
| Validation | FluentValidation |
| Logging | Serilog (Console, File, Seq) |
| API Documentation | Scalar / OpenAPI |
| Serialization | Newtonsoft.Json (camelCase) |
| Testing | xUnit, Testcontainers |
| Containerization | Docker, Docker Compose |
| Reverse Proxy | Nginx (production) |
| CI/CD | GitHub Actions, GHCR |

## System Boundaries

### In Scope

- CRUD operations for Clients, Sectors, Services, Projects, and Company Profile via REST endpoints under `/api/v1/`.
- API Key authentication (`X-API-Key` header) for write endpoints (POST, PUT, DELETE, PATCH).
- Public read access (GET) without authentication.
- Health check endpoints (`/health/live`, `/health/ready`).
- Interactive API documentation via Scalar at `/scalar/v1`.
- ProblemDetails (RFC 7807) error responses.

### Out of Scope

- JWT bearer authentication or token issuance.
- User registration, login, or API Key management UI.
- Role-based or policy-based authorization (`[Authorize]` attributes).
- API Key rotation, expiration, or per-client keys.
- Frontend or client application code.
- Mobile or desktop clients.

## Authentication

Write endpoints require a valid `X-API-Key` header. The key is configured via:

- Development: `dotnet user-secrets` or `appsettings.Development.json` (`ApiKeySettings:MasterKey`)
- Production: environment variable `ApiKeySettings__MasterKey`

Missing or invalid keys on write requests return `401 Unauthorized`. GET endpoints are public and do not require a key.

## Support

For API integration details, see [`docs/api-guide.md`](api-guide.md). For architecture diagrams, see [`docs/architecture/diagrams/`](architecture/diagrams/).
