# Global Architecture Specification

## Architectural Principles & Pattern: Clean Architecture + DDD + CQRS

This repository strictly enforces **Clean Architecture** combined with **Domain-Driven Design (DDD)** and **Command Query Responsibility Segregation (CQRS)**. 

The primary objective is to isolate core business rules from infrastructure, frameworks, and database dependencies.

---

## 1. Solution & Directory Layout
All source code and test projects must strictly adhere to the following directory structure:

- **`src/`**: Contains all production source code projects.
- **`test/`**: Contains all test suites (`.UnitTests`, `.IntegrationTests`).

```text
├── src/
│   ├── Core/
│   │   ├── Domain/                 # Enterprise Domain Logic (Entities, Aggregates, Value Objects)
│   │   └── Application/            # Business Use Cases (CQRS Commands, Queries, DTOs, Mapping)
│   ├── Infrastructure/
│   │   └── Persistence/            # DB Context, Repositories (EF Core + PostgreSQL), Migrations
│   └── Presentation/
│       └── WebAPI/                 # ASP.NET Core API Endpoints, Autofac Modules, Middlewares
└── test/
    ├── UnitTests/                  # Domain & Application Unit Tests (xUnit + Moq + Bogus)
    └── IntegrationTests/           # API & Database Integration Tests