# Spec Delta

## Purpose

Defines the foundational solution structure, project layout, naming conventions, and build configuration that all other capabilities depend on.

## ADDED Requirements

### Requirement: Solution file exists and uses XML format
The system SHALL provide a `Tfs.Portfolio.slnx` solution file in the repository root using the XML-based format (not legacy `.sln`).

#### Scenario: Solution file is valid XML format
- **WHEN** opening `Tfs.Portfolio.slnx` in Visual Studio or `dotnet sln list`
- **THEN** it loads successfully and lists all 6 projects

### Requirement: Production projects live under src/
The system SHALL organize all production code projects exclusively under `src/` following Clean Architecture layer directories.

#### Scenario: Directory structure matches Clean Architecture
- **WHEN** inspecting the repository structure
- **THEN** `src/Core/Domain`, `src/Core/Application`, `src/Infrastructure/Persistence`, `src/Presentation/WebAPI` exist

### Requirement: Test projects live under test/
The system SHALL organize all test projects exclusively under `test/` with `UnitTests` and `IntegrationTests` directories.

#### Scenario: Test directory structure exists
- **WHEN** inspecting the repository structure
- **THEN** `test/UnitTests` and `test/IntegrationTests` exist

### Requirement: Project names follow naming conventions
The system SHALL name projects and assemblies as `Tfs.Portfolio.<Layer>` matching the folder structure.

#### Scenario: Project names are correct
- **WHEN** listing projects in the solution
- **THEN** projects are named: `Tfs.Portfolio.Domain`, `Tfs.Portfolio.Application`, `Tfs.Portfolio.Infrastructure.Persistence`, `Tfs.Portfolio.Api`, `Tfs.Portfolio.UnitTests`, `Tfs.Portfolio.IntegrationTests`

### Requirement: Root namespace is Tfs.Portfolio
The system SHALL use `Tfs.Portfolio` as the root namespace for all projects.

#### Scenario: Namespaces follow convention
- **WHEN** inspecting any C# file in the solution
- **THEN** namespace starts with `Tfs.Portfolio` and mirrors folder structure

### Requirement: Shared build configuration exists
The system SHALL provide `Directory.Build.props` and `Directory.Build.targets` for common build settings (TargetFramework net10.0, Nullable enable, ImplicitUsings, TreatWarningsAsErrors, LangVersion latest).

#### Scenario: Build props apply to all projects
- **WHEN** building any project
- **THEN** it targets net10.0, has nullable enabled, treats warnings as errors

### Requirement: Global.json pins SDK version
The system SHALL include `global.json` to pin the .NET SDK version.

#### Scenario: SDK version is pinned
- **WHEN** running `dotnet --version` in the repo
- **THEN** it matches the version in `global.json`

### Requirement: EditorConfig enforces code style
The system SHALL include `.editorconfig` for consistent formatting across editors.

#### Scenario: EditorConfig is respected
- **WHEN** formatting code in a supported editor
- **THEN** it follows the defined style rules

### Requirement: Gitignore excludes build artifacts and secrets
The system SHALL include `.gitignore` excluding `bin/`, `obj/`, `.env*`, `docker-compose.override.yml`, user secrets.

#### Scenario: Git ignores build output and secrets
- **WHEN** running `git status` after build
- **THEN** no `bin/`, `obj/`, or secret files appear as untracked