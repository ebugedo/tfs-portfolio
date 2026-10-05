# Global Technology Stack Specification

## Core Platform
- **Runtime & Framework:** .NET 10 (ASP.NET Core Web API).
- **Language:** C# (Latest language features enabled).
- **JSON Serialization:** Newtonsoft.Json (Json.NET) - Required for all API serialization, configuration, and logging.

## Domain & Application Layer Libraries
- **Dependency Injection:** Autofac (Used as the IoC/DI container for modular component registration).
- **Object Mapping:** AutoMapper (For entity-to-DTO and DTO-to-entity mapping across layers).

## Persistence & Database
- **Database:** PostgreSQL.
- **ORM Provider:** Entity Framework Core using `Npgsql.EntityFrameworkCore.PostgreSQL`.

## Logging & Observability
- **Structured Logging:** Serilog (`Serilog.AspNetCore`) - Structured, structured logging with sinks for Console, File, Seq, etc.
- **Request Logging:** `Serilog.RequestLogging` - Automatic HTTP request/response logging.
- **Enrichment:** Serilog enrichers (FromLogContext, WithMachineName, WithThreadId, WithProcessId).

## API Documentation & Testing
- **OpenAPI Specification:** Built-in ASP.NET Core OpenAPI support (`Microsoft.AspNetCore.OpenApi`).
- **API Documentation UI:** Scalar (`Scalar.AspNetCore`) - Modern, fast, and customizable API reference.
- **API Versioning:** Built-in ASP.NET Core route-based versioning via `MapGroup`.

## Testing & Quality Assurance
- **Test Framework:** xUnit.
- **Mocking Library:** Moq.
- **TestData Generation:** Bogus (For fake data creation in unit tests, integration tests, and database seeding).

## Repository & Solution Layout Conventions
- **Production Code:** All production source code projects MUST be located exclusively under `src/`.
- **Test Code:** All unit, integration, and functional test projects MUST be located exclusively under `test/`.
- **Solution File:** Use `.slnx` (XML-based solution file) format. Do NOT use legacy `.sln` format.
- **Naming Conventions:** Project names, namespaces, solution file, Docker image, and HTTP endpoints follow conventions defined in `coding-standards.md` (Project, Solution & Namespace Conventions; Docker & Container Conventions; HTTP API Endpoint Conventions).
  - Solution: `Tfs.Portfolio.slnx`
  - Root Namespace: `Tfs.Portfolio`
  - Docker Image: `tfs/portfolio-api:<tag>`
  - API Base: `/api/v1/<plural-resource>`