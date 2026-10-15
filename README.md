# Tfs.Portfolio

Portfolio Management API built with .NET 10, ASP.NET Core, Entity Framework Core, and PostgreSQL.

## Architecture

- **Domain Layer**: Core business logic, entities, value objects, domain events
- **Application Layer**: CQRS commands/queries, handlers, validators, DTOs
- **Infrastructure Layer**: EF Core persistence, repositories, migrations
- **Presentation Layer**: ASP.NET Core Web API, controllers, middleware

## Development

### Prerequisites
- .NET 10 SDK
- Docker & Docker Compose
- PostgreSQL (via Docker Compose)

### Quick Start
```bash
# Start development environment (API + PostgreSQL)
docker compose up -d

# API available at http://localhost:8082
# Swagger UI at http://localhost:8082/scalar/v1
```

### Run Tests
```bash
# Unit tests
dotnet test test/UnitTests/Tfs.Portfolio.UnitTests.csproj

# Integration tests (requires PostgreSQL on localhost:5432)
dotnet test test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj
```

## Production Deployment

### Architecture
```
Internet → Nginx (TLS termination) → API Container (port 8080, localhost only)
                              ↓
                        PostgreSQL Container (port 5432, Docker network)
```

### CI/CD Pipeline (GitHub Actions)
- **CI**: Runs on push/PR to `main` → unit tests → integration tests → build Docker image → push to GHCR
- **CD**: Runs on push to `main` (after CI passes) → SSH to VPS → deploy via Docker CLI

### Required GitHub Secrets
| Secret | Description |
|--------|-------------|
| `VPS_HOST` | VPS IP address or domain |
| `VPS_USERNAME` | SSH user for deployment |
| `VPS_SSH_KEY` | Private SSH key (OpenSSH format) |
| `VPS_PORT` | SSH port (default: 22) |
| `GHCR_PAT` | PAT with `write:packages`, `read:packages`, `delete:packages` |
| `POSTGRES_PASSWORD` | PostgreSQL superuser password |
| `POSTGRES_DB` | Database name (e.g., `portfolio`) |
| `POSTGRES_USER` | PostgreSQL user (e.g., `postgres`) |
| `API_PORT` | Host port for API (e.g., `8080`) |

### VPS One-Time Setup
```bash
# Create Docker network (shared with Nginx)
docker network create nginx-net

# Run PostgreSQL with persistent volume
docker run -d \
  --name postgres \
  --network nginx-net \
  --restart unless-stopped \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=<same-as-secret> \
  -e POSTGRES_DB=portfolio \
  -v postgres_data:/var/lib/postgresql/data \
  postgres:16-alpine

# Configure Nginx to proxy HTTPS → http://localhost:8080 (ensure Nginx is on nginx-net)
```

### Deployment Process
1. Push to `main` branch
2. GitHub Actions runs CI (tests + build + push to GHCR)
3. GitHub Actions runs CD (SSH to VPS, pull image, replace container)
4. API available at configured domain

### Rollback
```bash
# On VPS, deploy previous version by SHA tag
docker stop portfolio-api && docker rm portfolio-api
docker run -d --name portfolio-api --network nginx-net --restart unless-stopped \
  -p 127.0.0.1:8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=portfolio;Username=postgres;Password=<pwd>" \
  -e Seq__ServerUrl="" -e Seq__ApiKey="" \
  ghcr.io/<owner>/<repo>:sha-<previous-commit-sha>
```

## Technology Stack
- .NET 10 / ASP.NET Core
- Entity Framework Core 9 + Npgsql
- Autofac for DI
- FluentValidation
- Serilog (Console + File)
- Scalar for API documentation
- xUnit for testing
- Testcontainers for integration tests