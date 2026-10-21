# Tfs.Portfolio — API Built with OpenCode, SDD (OpenSpec), Free Zen Models — Professional API in Production

## 1. Introduction and System Scope

### What is Tfs.Portfolio?

Tfs.Portfolio is a Portfolio Management API built with .NET 10, ASP.NET Core Web API, Entity Framework Core 9 + Npgsql (PostgreSQL), Autofac (DI), FluentValidation, Serilog, Scalar/OpenAPI, xUnit, and Testcontainers. It is deployed on a VPS with Docker and Nginx, with CI/CD via GitHub Actions to GHCR.

**Source code repository:** https://github.com/ebugedo/tfs-portfolio  
**Production API documentation:** https://api-portfolio.timeforsoftware.com/scalar/v1

### Domain

The system models a portfolio management domain with the following core concepts:

- **Clients** — Individuals or organizations that own projects. Each client has a name, email, and active status.
- **Sectors** — Industry or business sectors used to categorize projects (e.g., Technology, Healthcare, Finance).
- **Services** — Offerings that can be associated with projects. Each service has a name, category, and active status.
- **Projects** — Portfolio items linking a client, sector, and set of services. Projects have start dates, durations, technologies, and status.
- **Company Profile** — A singleton record describing the organization running the portfolio (name, description, contact details).

### Objectives

- Provide a REST API for managing portfolio data (clients, sectors, services, projects, company profile).
- Protect write operations (POST, PUT, DELETE, PATCH) with API Key authentication while keeping read operations (GET) publicly accessible.
- Serve interactive API documentation via Scalar/OpenAPI at `/scalar/v1` (development) and https://api-portfolio.timeforsoftware.com/scalar/v1 (production).
- Support deployment on a VPS with Docker and Nginx, with CI/CD via GitHub Actions.

---

## 2. Technology Stack

| Layer | Technology | Version | Purpose |
|-------|------------|---------|---------|
| Runtime | .NET | 10 | Application runtime and SDK |
| Web Framework | ASP.NET Core Web API | .NET 10 | REST API, controllers, middleware |
| DI Container | Autofac | Latest | Dependency injection |
| ORM | Entity Framework Core | 9 | Data access, migrations, Npgsql provider |
| Database | PostgreSQL | 16 | Persistent storage |
| Validation | FluentValidation | Latest | Input validation |
| Logging | Serilog | Latest | Console, File, and Seq sinks |
| API Documentation | Scalar / OpenAPI | Latest | Interactive API docs at /scalar/v1 and production |
| Serialization | Newtonsoft.Json | Latest | JSON serialization with camelCase |
| Testing | xUnit | Latest | Unit tests |
| Integration Testing | Testcontainers | Latest | Integration tests with real dependencies |
| Containerization | Docker / Docker Compose | Latest | Environment isolation and deployment |
| Reverse Proxy | Nginx | Latest | TLS termination and request routing |
| CI/CD | GitHub Actions | — | Build, test, and deploy pipeline |
| Container Registry | GHCR | — | Stores and distributes Docker images |

---

## 3. Methodology and Development Tools

### OpenCode with OpenSpec

This project was planned and developed using **OpenCode with OpenSpec** (spec-driven flow: proposal → specs → design → tasks → apply → archive) and **free Zen models of OpenCode** for assistance in development and planning. This information is not present in any project source file and is documented here explicitly.

OpenSpec provides a structured planning workflow:

- **proposal.md** — What and why of the change
- **specs/<capability>/spec.md** — Delta specifications (requirements the system must do)
- **design.md** — How to implement the change
- **tasks.md** — Implementation steps with trackable checkboxes

The workflow ensures clear separation between planning and implementation, with each phase gated on the previous one.

### Spec-Driven Development (SDD)

Requirements are defined as specifications before implementation. Each requirement includes scenarios that define expected behavior, making the spec executable and verifiable.

### Clean Architecture

The system follows Clean Architecture with separation of concerns across layers:

- **Presentation Layer** — ASP.NET Core Web API controllers, middleware, OpenAPI/Scalar configuration
- **Application Layer** — CQRS commands/queries, handlers, validators, DTOs (using MediatR)
- **Infrastructure Layer** — Entity Framework Core persistence, repositories, migrations
- **Domain Layer** — Core business logic, entities, value objects, domain events

Layers communicate through dependency injection (Autofac). The API project references Application and Persistence only; Domain is accessed indirectly through Application.

### Domain-Driven Design (DDD)

The domain is the center of the design:

- **Entities** — Client, Sector, Service, Project, CompanyProfile
- **Value Objects** — Domain value objects (e.g., Email, Money)
- **Domain Events** — Event types and handlers for business events
- **Aggregates** — Each entity is an aggregate root with clear boundaries

Ubiquitous language is shared between the business domain and the code model.

### C4 Architecture Documentation

The system architecture is documented using the C4 model with PlantUML diagrams located under `docs/architecture/diagrams/`:

- **c4-context.puml** — System Context diagram showing external users and dependencies
- **c4-containers.puml** — Container diagram showing API container, PostgreSQL, Nginx, Docker, GHCR
- **c4-components.puml** — Component diagram showing Domain, Application, Infrastructure, Presentation layers
- **c4-code.puml** — Code diagram showing key classes and interfaces

---

## 4. API Endpoints

All endpoints are under `/api/v1/`. JSON responses use camelCase property names. Write operations require a valid `X-API-Key` header; read operations (GET) are public.

**Production API:** https://api-portfolio.timeforsoftware.com/scalar/v1  
**Development API:** http://localhost:8082/scalar/v1

### Clients

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/clients` | List all clients | Public |
| GET | `/api/v1/clients/{id}` | Get client by ID | Public |
| POST | `/api/v1/clients` | Create a client | API Key |
| PUT | `/api/v1/clients/{id}` | Update a client | API Key |
| DELETE | `/api/v1/clients/{id}` | Delete a client | API Key |

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

### Error Responses

All errors use RFC 7807 ProblemDetails format:

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

### Health Checks

| Endpoint | Description |
|----------|-------------|
| GET `/health/live` | Liveness probe |
| GET `/health/ready` | Readiness probe (includes database check) |

### Interactive Documentation

Scalar/OpenAPI UI is available at `/scalar/v1` on the API host (e.g., `http://localhost:8082/scalar/v1` for development, `https://api-portfolio.timeforsoftware.com/scalar/v1` for production).

---

## 5. Deployment and Infrastructure

### Development Environment

```bash
docker compose up -d
```

The API is available at `http://localhost:8082`. Scalar documentation at `http://localhost:8082/scalar/v1`.

### Production Deployment

The system deploys to a VPS with Docker and Nginx:

- **Nginx** — TLS termination and request routing (ports 443/80)
- **API Container** — ASP.NET Core on port 8080 (localhost only)
- **PostgreSQL Container** — Persistent storage on port 5432 (Docker network)
- **Docker** — Containerization and environment isolation

### CI/CD Pipeline (GitHub Actions)

- **CI** — Runs on push/PR to `main`: unit tests → integration tests → build Docker image → push to GHCR
- **CD** — Runs on push to `main` (after CI passes): SSH to VPS → deploy via Docker CLI

### Required GitHub Secrets

| Secret | Description |
|--------|-------------|
| `VPS_HOST` | VPS IP address or domain |
| `VPS_USERNAME` | SSH user for deployment |
| `VPS_SSH_KEY` | Private SSH key (OpenSSH format) |
| `VPS_PORT` | SSH port (default: 22) |
| `GHCR_PAT` | PAT with write/read/delete packages |
| `POSTGRES_PASSWORD` | PostgreSQL superuser password |
| `POSTGRES_DB` | Database name |
| `POSTGRES_USER` | PostgreSQL user |
| `API_PORT` | Host port for API |

---

## 6. Development Virtues — Group 1: OpenCode + OpenSpec + Free Zen Models

### Structured Planning with OpenSpec

OpenSpec provides a structured planning workflow (proposal → specs → design → tasks → apply → archive) that ensures:

- Clear separation between planning and implementation
- Each phase is gated on the previous one
- Requirements are documented before code is written
- Traceability from specification to tasks

### OpenCode with Free Zen Models

The use of free Zen models of OpenCode provides:

- **Zero-cost access** — No licensing fees for AI assistance
- **Code and documentation generation** — Fast generation of boilerplate and documentation
- **Task automation** — Assistance with repetitive development tasks
- **Lower barrier to entry** — Makes AI-assisted development accessible without paid subscriptions

### Clear Phased Workflow

The guided workflow (propose → apply → archive) ensures:

- Planning artifacts are complete before implementation
- Implementation is traceable to specifications
- Changes are archived with full context after completion

---

## 7. Development Virtues — Group 2: SDD, Clean Architecture and DDD

### Spec-Driven Development (SDD)

SDD treats requirements as executable specifications:

- Requirements are defined before implementation
- Scenarios define expected behavior for each requirement
- Implementation is verifiable against the spec
- Reduces ambiguity between stakeholders and developers

### Clean Architecture

Clean Architecture provides:

- **Separation of layers** — Domain, Application, Infrastructure, Presentation are independent
- **Framework independence** — Business logic is not coupled to ASP.NET Core or EF Core
- **Testability** — Each layer can be tested in isolation
- **Maintainability** — Changes to one layer do not cascade to others

### Domain-Driven Design (DDD)

DDD centers the design on the business domain:

- **Domain as center** — Entities, value objects, and domain events drive the design
- **Ubiquitous language** — Shared vocabulary between business and code
- **Reduced coupling** — Domain layer has no external dependencies
- **Controlled evolution** — Architecture evolves with the domain, not the other way around

---

## 8. Development Virtues — Group 3: Detailed Technologies

### .NET 10

- High performance and productivity
- Mature ecosystem with extensive library support
- Long-term support (LTS)
- Cross-platform (Windows, Linux, macOS)

### ASP.NET Core

- Fast and lightweight web framework
- Rich middleware pipeline
- Support for both controllers and Minimal APIs
- Built-in dependency injection

### Entity Framework Core 9

- Object-relational mapping (ORM)
- Database migrations with code-first approach
- LINQ-based querying
- Npgsql provider for PostgreSQL

### Docker

- Consistent environments across development and production
- Application isolation
- Simplified deployment and scaling

### Scalar/OpenAPI

- Interactive API documentation generated automatically
- Try-it-out functionality for endpoint testing
- Available at `/scalar/v1` in all environments and at production URL

### xUnit + Testcontainers

- Unit tests with xUnit framework
- Integration tests with real dependencies (PostgreSQL) via Testcontainers
- Reliable testing without mocking infrastructure

---

## 9. Problems and Challenges — Group 1: OpenCode + OpenSpec + Free Zen Models

### OpenSpec Discipline

OpenSpec requires discipline to maintain artifacts synchronized with the code. There is a risk of desynchronization between planning and implementation if OpenSpec is not used consistently throughout the development lifecycle.

### Free Zen Model Limitations

Free Zen models of OpenCode have limitations compared to paid models:

- Rate limits on requests
- Reduced context window
- Variable quality of generated output
- Dependence on internet connectivity for model access

---

## 10. Problems and Challenges — Group 2: SDD, Clean Architecture and DDD

### Potential Over-Engineering

For small projects, the multi-layered architecture (Domain → Application → Infrastructure → Presentation) may be over-engineering, adding complexity without immediate benefit.

### Learning Curve

DDD and Clean Architecture have a steeper learning curve. Developers need to understand concepts like aggregates, value objects, domain events, and CQRS before being productive.

### Boilerplate Code

More layers mean more boilerplate code and intermediate abstractions. Initial setup time is higher compared to simpler architectures.

---

## 11. Problems and Challenges — Group 3: Detailed Technologies

### Autofac Configuration

Autofac dependency injection configuration in `Program.cs` can become verbose as the application grows, requiring careful module organization.

### Secret Management

Managing secrets across environments is challenging:

- Development: `dotnet user-secrets` or `appsettings.Development.json`
- Production: Environment variables injected via CI/CD and Docker

### Docker + Nginx + PostgreSQL on VPS

Production deployment requires manual configuration of:

- Docker network sharing between Nginx and API
- Persistent volumes for PostgreSQL data
- Nginx TLS termination and proxy configuration

### Testcontainers Requirements

Integration tests with Testcontainers require Docker to be available in the test environment, which may not be available in all CI contexts.

### PlantUML Rendering

PlantUML diagrams require an external renderer (PlantUML server, plugin, or online tool) to visualize; they are not rendered natively by all Markdown viewers.

---

## 12. Conclusions and Recommendations

### Balance Between Architecture and Complexity

Clean Architecture and DDD provide long-term maintainability and testability but add initial complexity. For this project, the layered architecture is justified by the need for maintainable, testable portfolio management logic.

### Maintaining OpenSpec Documentation

To keep OpenSpec artifacts synchronized with the code:

- Update `proposal.md`, `specs/`, `design.md`, and `tasks.md` with each feature PR
- Include documentation changes in the definition of done
- Review planning artifacts during code review

### Suggested Next Steps

1. Integrate documentation updates into the CI/CD pipeline
2. Consider automated validation of OpenSpec artifacts against the codebase
3. Add architectural decision records (ADRs) for significant technical decisions

---

## 13. Appendix: Glossary of Terms

| Term | Definition |
|------|------------|
| API Key | Authentication token passed in the `X-API-Key` header for write operations |
| CQRS | Command Query Responsibility Segregation — separates read and write operations |
| DDD | Domain-Driven Design — software design centered on the business domain |
| GHCR | GitHub Container Registry — stores and distributes Docker images |
| OpenCode | AI-assisted coding assistant (OpenCode) |
| OpenSpec | Open Specification — structured planning workflow (proposal, specs, design, tasks) |
| ProblemDetails | RFC 7807 standard format for HTTP error responses |
| SDD | Spec-Driven Development — requirements defined as executable specifications |
| Scalar | Interactive API documentation UI for OpenAPI |
| Testcontainers | Library for integration tests with disposable container instances |
| VPS | Virtual Private Server — cloud-hosted server for production deployment |
| Zen Models | Free AI models provided by OpenCode |

---

*Generated from `docs/Tfs.Portfolio-Documentation.md`*
