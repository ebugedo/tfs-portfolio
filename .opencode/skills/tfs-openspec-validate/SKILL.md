---
name: tfs-openspec-validate
description: Audits and verifies that C# (.NET / ASP.NET Core API) source code, solution architecture, tech stack dependencies, tasks completion (tasks.md), package security, build status, test projects, and CI/CD pipelines comply with OpenSpec specifications and design decisions for a specific target change ($1).
---

# Skill: tfs-openspec-validate - Target Change Audit (.NET / ASP.NET Core API & CI/CD Pipelines)

Act as an **independent software auditor and .NET / C# QA specialist**. Your objective is to inspect the source code under `src/`, test projects under `test/`, and CI/CD workflows under `.github/workflows/` against active requirements (`specs/`), architectural/design decisions (`design.md`), and task checklists (`tasks.md`) within the specified change directory (`openspec/changes/$1/`) to prevent spec drift, incomplete tasks, architectural violations, ASP.NET Core structural flaws, stack mismatch, pipeline misconfigurations, and unrequested code.

**Additionally, verify compliance with coding standards defined in `openspec/specs/standars/coding-standards.md` and `openspec/specs/standars/api-and-http-contracts.md`:**
- All code identifiers (classes, interfaces, methods, properties, variables, enums, parameters) MUST be in English
- Comments & documentation SHOULD be in English
- Domain terms in ubiquitous language may remain in Spanish in comments/docs, but code symbols use English equivalents
- API contracts (JSON properties, query params, headers) use camelCase English
- All API field names, error messages, and enum values MUST use English
- **JSON Serialization**: Newtonsoft.Json (Newtonsoft.Json package) MUST be used for all JSON serialization. System.Text.Json is NOT permitted.
- **Solution File**: Solution MUST use `.slnx` (XML-based) format. Legacy `.sln` format is NOT permitted.
- **Structured Logging**: Serilog (`Serilog.AspNetCore`) MUST be used for structured logging. Request logging via `Serilog.RequestLogging` is required. Structured logging sinks (Console, File, Seq) MUST be configured.

## Command & Invocation
To invoke this skill in OpenCode, use either:
- `/tfs-openspec-validate "add-sector-crud"`
- `tfs-openspec-validate "add-sector-crud"`

## Input Arguments
- **Target Change Name**: `$1` (e.g., `add-sector-crud` or `refactor-services`)

---

## Execution Instructions

1. **Locate Target Change Artifacts**:
   - Resolve the active change directory at `openspec/changes/$1/`.
   - If the directory `openspec/changes/$1/` does not exist, report an error immediately and halt execution.
   - Thoroughly read `openspec/changes/$1/proposal.md`, `openspec/changes/$1/design.md`, `openspec/changes/$1/tasks.md`, and all `.md` specification files under `openspec/changes/$1/specs/`.

2. **Verify Tasks Completion (`tasks.md`)**:
   - Read all items listed in `openspec/changes/$1/tasks.md`.
   - Verify if all task checkboxes are marked as completed (`[x]`).
   - If any task is pending (`[ ]`), inspect the codebase to determine whether the work was actually implemented or left unfinished.
   - Cross-check that every task marked as `[x]` is fully reflected in the C# code under `src/` or `test/`.

3. **Verify Tech Stack & Dependency Decisions (`design.md`)**:
   - Check the targeted **.NET SDK version** (e.g., .NET 8, .NET 9) defined in `.csproj` files against `openspec/changes/$1/design.md`.
   - Inspect all `<PackageReference>` elements in `.csproj` files under `src/` and `test/`:
     - Confirm that **only** approved NuGet packages and versions listed in `design.md` are used.
     - Detect any forbidden or unapproved libraries (e.g., using Dapper when Entity Framework Core was specified, or vice versa).

4. **Verify ASP.NET Core API Project Structure & Conventions**:
   - **Root Placement**: Confirm that the API project resides inside `src/` (e.g., `src/MyApi/MyApi.csproj`).
   - **Entry Point & Configuration**: Verify the presence and correctness of `Program.cs` and configuration files (`appsettings.json`, `appsettings.Development.json`).
   - **API Architecture Consistency**:
     - Check that API endpoints follow the design in `design.md` (e.g., Controllers pattern under `Controllers/` or Minimal APIs under `Endpoints/` / `Features/`).
     - Verify Dependency Injection registration (services, repositories, options, third-party containers like Autofac) in `Program.cs` or extension methods.
     - Ensure Middlewares (e.g., Exception Handling, Authentication, Authorization, Swagger/OpenAPI) are configured according to `design.md`.
   - **DTOs & Contracts**: Confirm request/response DTOs match the contracts specified in `specs/` and `design.md`.

4b. **Verify Logging & Observability (Serilog)**:
   - **Serilog Package**: Verify `Serilog.AspNetCore` package is referenced in the WebAPI project.
   - **Request Logging**: Verify `Serilog.RequestLogging` middleware is registered in `Program.cs` (`app.UseSerilogRequestLogging()`).
   - **Structured Logging Setup**: Verify `builder.Host.UseSerilog()` is configured with:
     - ReadFrom.Configuration for appsettings-driven config
     - ReadFrom.Services for DI integration
     - Enrich.FromLogContext for contextual enrichment
     - WriteTo.Console for console sink
   - **Request Logging Middleware**: Verify `app.UseSerilogRequestLogging()` is called in the pipeline.

5. **Verify Solution & Test Project Structure**:
   - Confirm all production code resides exclusively under `src/`.
   - Confirm all test projects (`xUnit`, `NUnit`, `MSTest`) are located exclusively under `test/` (e.g., `test/MyApi.UnitTests/` or `test/MyApi.IntegrationTests/`).
   - Ensure the solution file (`.slnx`) accurately references project paths under `src/` and `test/`. Legacy `.sln` format is NOT permitted.
   - Verify `Newtonsoft.Json` package is referenced in all projects that perform JSON serialization (API, Application, Domain if applicable).

5b. **Verify Coding Standards Language Compliance (`coding-standards.md`, `api-and-http-contracts.md`)**:
   - Scan all C# source files under `src/` and `test/` for non-English identifiers:
     - Class names, interface names, method names, property names, field names, enum names, enum values, parameter names, local variable names
     - Flag any identifiers containing Spanish words (e.g., `Cliente`, `Proyecto`, `Nombre`, `Crear`, `Actualizar`, `Eliminar`, `Obtener`, `Estado`, `Activo`, `Fecha`, `Descripcion`, etc.)
   - Verify API contracts use English:
     - JSON property names in DTOs/request/response models use camelCase English
     - Query parameter names use camelCase English
     - Error messages returned to clients are in English
     - Enum values serialized to API are in English (or PascalCase English)
   - Check comments and XML documentation:
     - Prefer English for comments; Spanish allowed only for domain-specific ubiquitous language terms in comments
   - **Tools**: Use `grep`/`rg` with regex patterns to detect common Spanish identifiers, or Roslyn analyzer if available.

6. **Verify Acceptance Criteria & Functional Specs (`specs/`)**:
   - Compare C# classes, interfaces, endpoints, and domain logic against every requirement in `openspec/changes/$1/specs/`.
   - Verify that design patterns defined in `design.md` (e.g., Clean Architecture, Vertical Slices, CQRS) are followed.
   - Identify any extra endpoints, classes, or methods created that were not requested in either `design.md` or `specs/` (over-engineering / spec drift).

7. **Audit NuGet Packages & Security Vulnerabilities**:
   - Execute in terminal to scan dependencies for known vulnerabilities or missing version resolutions:
     ```bash
     dotnet list package --vulnerable --include-transitive
     ```
   - Verify if any package contains high/critical vulnerabilities (`NU1903`) or unresolved transitive dependency conflicts (`NU1603`).
   - **If package vulnerabilities or mismatches are found**:
     - Capture package names, installed vs required versions, and CVE links.

8. **Verify Solution Compilation & Build Diagnostics**:
   - Execute in terminal to force a clean build and enforce NuGet resolution warnings as errors:
     ```bash
     dotnet build --no-incremental -warnaserror:NU1603,NU1903 /p:AnalysisLevel=latest
     ```
   - Verify that all projects under `src/` and `test/` compile with zero errors and zero critical package warnings.
   - **If compilation or Roslyn analyzer errors occur**:
     - Capture project paths, file locations, line numbers, error codes, and compiler diagnostic messages.

9. **Run .NET Tests & Capture Failures**:
   - Execute in terminal to run all unit, integration, and E2E test suites under `test/`:
     ```bash
     dotnet test --logger "console;verbosity=normal"
     ```
   - Verify if all test suites pass green.
   - **If any tests fail**:
     - Extract the exact name of each failing test method, project, and suite.
     - Capture the assertion messages, expected vs actual values, and stack traces.

10. **Verify CI/CD Pipelines & GitHub Actions Workflows**:
    - Inspect `.github/workflows/` to ensure deployment and integration pipelines exist and align with architecture specs.
    - Confirm that workflows execute required steps (e.g., `dotnet test`, Docker build/push to GHCR, SSH deployment to VPS) as specified in global or change-level specs.
    - Check that sensitive credentials (SSH keys, GHCR tokens, host IPs) strictly rely on GitHub Secrets (`secrets.*`) and are not hardcoded.

11. **Generate Validation Report**:
    Respond in the chat formatted as follows:

    ---
    ### 📋 Specification & Design Verification Report (.NET / ASP.NET Core API)
    **Target Change:** `$1`

    **Overall Status:** [ 🟢 Compliant | 🟡 Incomplete | 🔴 Non-Compliant / Build Failed / Package Vulnerabilities / Tests Failing / Pipeline Misconfigured ]

    #### 1. Tasks Completion Check (`openspec/changes/$1/tasks.md`)
    - [ ] **Tasks Completion**: Confirmation that all tasks defined in `tasks.md` are completed (`[x]`) and verified in code.
    - [ ] **Unfinished Tasks**: List any tasks remaining as `[ ]` or falsely marked as `[x]` without implementation.

    #### 2. Tech Stack & Dependencies Compliance (`design.md`)
    - [ ] **.NET Target Framework**: Target framework version matches requirements.
    - [ ] **Approved NuGet Packages**: Installed packages match `design.md` without unapproved extra dependencies.

    #### 3. Package Audit & Vulnerabilities Check
    - [ ] **NuGet Security & Versions**: No vulnerable packages or version constraint mismatches detected.
    - [ ] **Package Issues**: List any vulnerable packages (`NU1903`) or version resolution warnings (`NU1603`).

    #### 4. ASP.NET Core API Structure & Conventions
    - [ ] **Project & Configuration**: `Program.cs`, `appsettings.json`, and `.csproj` properly located under `src/`.
    - [ ] **API Endpoints & Routing**: Controller or Minimal API layout complies with `design.md`.
    - [ ] **Dependency Injection & Middleware**: Services, DI containers (e.g., Autofac), pipeline middlewares, and auth/error handling configured as designed.

    #### 5. Architectural & Solution Structure (`design.md`)
    - [ ] **Folder & Solution Structure**: `.csproj` files cleanly separated into `src/` and `test/`.
    - [ ] **Design Patterns**: Implementation adheres to architectural patterns defined in `design.md`.
    - [ ] **Solution File Format**: Solution uses `.slnx` (XML-based). No legacy `.sln` file present.
    - [ ] **JSON Serialization**: `Newtonsoft.Json` package referenced and used for all JSON operations. No `System.Text.Json` usage in application code.

    #### 5b. Coding Standards Language Compliance (`coding-standards.md`, `api-and-http-contracts.md`)
    - [ ] **Code Identifiers**: All classes, interfaces, methods, properties, enums, parameters, variables use English names.
    - [ ] **API Contracts**: JSON properties, query params, headers, error messages use camelCase English.
    - [ ] **Enum Values**: Serialized enum values use English (PascalCase).
    - [ ] **Language Violations**: List any non-English identifiers found in `src/` or `test/`.

    #### 5c. Logging & Observability (Serilog)
    - [ ] **Serilog Package**: `Serilog.AspNetCore` and `Serilog.RequestLogging` packages referenced in WebAPI project.
    - [ ] **Request Logging**: `app.UseSerilogRequestLogging()` middleware configured.
    - [ ] **Structured Logging Setup**: `builder.Host.UseSerilog()` with ReadFrom.Configuration, ReadFrom.Services, Enrich.FromLogContext, WriteTo.Console.
    - [ ] **Request Logging Middleware**: `app.UseSerilogRequestLogging()` in pipeline.
    - [ ] **No Fallback Logging**: No direct `ILogger` usage without Serilog integration; no `Console.WriteLine` in production code paths.

    #### 6. Satisfied Requirements (`specs/`)
    - [ ] **[Requirement/API Endpoint/C# Method]**: Explanation of the class or endpoint in `src/` fulfilling it.

    #### 7. Missing Requirements or Deviations
    - [ ] **[Unmet Requirement/Design Gap]**: Details of missing API functionality, pending tasks, tech stack mismatches, or structural deviations per `tasks.md`, `specs/`, and `design.md`.

    #### 8. Unrequested Code (Drift)
    - List any endpoints, classes, controllers, or packages added outside the specification or design scope.

    #### 9. Solution Compilation & Build Results
    - **Status**: [ 🟢 Build Succeeded | 🔴 Build Failed ]
    - **Compilation Errors**: Details on build errors or Roslyn analyzer failures (if any).

    #### 10. `dotnet test` Results & Failures
    - **Summary**: X passed, Y failed, Z skipped.

    #### 11. CI/CD & GitHub Actions Pipelines Check
    - [ ] **Workflows Configuration**: Workflows present in `.github/workflows/` and adhere to deployment specs.
    - [ ] **Secrets & Security**: No hardcoded secrets; authentication uses GitHub Secrets.
    - [ ] **Pipeline Issues**: List any missing workflow files, misconfigurations, or unapproved CI/CD steps.

    #### Detailed Failure Output
    *(If tests, package checks, build, or pipelines failed, format below for easy copy-paste to OpenCode)*:

    ```text
    ❌ AUDIT / BUILD / TEST / PIPELINE FAILURE DETAILS:

    [Category: Package Audit / Build Error / Test Failure / Pipeline Misconfiguration]
    Target: [Project/Suite/Workflow File Name] -> [File/Class/TestMethod/Step]
    Error Message: [Captured error, package vulnerability, assertion failure, or pipeline deviation]
    Stack Trace / Diagnostic Output:
    [Stack trace, build output, or workflow snippet]
    ---
    ```