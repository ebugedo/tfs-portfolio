# Global Technology Stack Specification

## Core Platform
- **Runtime & Framework:** .NET 10 (ASP.NET Core Web API).
- **Language:** C# (Latest language features enabled).
- **JSON Serialization:** Newtonsoft.Json (Json.NET) - Use Newtonsoft.Json wherever possible for serialization, deserialization, configuration, and logging across the entire solution (domain, application, infrastructure, API, tests). Where a library does not expose a Newtonsoft.Json switch (e.g., Npgsql 10 driver-level JSONB serialization lacks `UseJsonNet`), the best available opt-in (such as Npgsql `EnableDynamicJson`) is used instead.

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
- **Validation:** FluentValidation - Used for request/command validation across all layers (API, Application, Domain) with automatic integration in ASP.NET Core Controllers and Minimal APIs.

## Repository & Solution Layout Conventions
- **Production Code:** All production source code projects MUST be located exclusively under `src/`.
- **Test Code:** All unit, integration, and functional test projects MUST be located exclusively under `test/`.
- **Solution File:** Use `.slnx` (XML-based solution file) format. Do NOT use legacy `.sln` format.
- **Naming Conventions:** Project names, namespaces, solution file, Docker image, and HTTP endpoints follow conventions defined in `coding-standards.md` (Project, Solution & Namespace Conventions; Docker & Container Conventions; HTTP API Endpoint Conventions).
  - Solution: `Tfs.Portfolio.slnx`
  - Root Namespace: `Tfs.Portfolio`
  - Docker Image: `tfs/portfolio-api:<tag>`
  - API Base: `/api/v1/<plural-resource>`