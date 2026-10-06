# Spec Delta

## Purpose

Defines containerization for development and production: multi-stage Dockerfile, Docker Compose for local development (API + PostgreSQL + Seq), and image publishing to GHCR.

## ADDED Requirements

### Requirement: Multi-stage Dockerfile produces minimal runtime image
The system SHALL provide a `Dockerfile` with build stage (SDK image) and runtime stage (ASP.NET Runtime image), copying only published output to runtime.

#### Scenario: Runtime image is minimal
- **WHEN** inspecting final image size
- **THEN** it is under 200MB (ASP.NET runtime + app only, no SDK)

### Requirement: Dockerfile uses non-root user
The system SHALL create a non-root user in the runtime stage and `USER` switch to it.

#### Scenario: Container runs as non-root
- **WHEN** running `docker run --rm tfs/portfolio-api:latest whoami`
- **THEN** output is not `root`

### Requirement: .dockerignore excludes build artifacts and secrets
The system SHALL maintain `.dockerignore` at repo root excluding `bin/`, `obj/`, `.git/`, `.env*`, `docker-compose.override.yml`, `**/*.slnx`, `**/*.csproj.user`.

#### Scenario: Build context is clean
- **WHEN** running `docker build .`
- **THEN** no bin/obj/git/env files sent to daemon

### Requirement: Docker Compose orchestrates local development
The system SHALL provide `docker-compose.yml` at repo root with services: `api` (build from Dockerfile), `postgres` (PostgreSQL 16), `seq` (Seq latest).

#### Scenario: Dev environment starts with one command
- **WHEN** running `docker compose up -d`
- **THEN** all three services start; API accessible at `http://localhost:8080`

### Requirement: API service supports hot reload in development
The system SHALL mount source code as volume in `docker-compose.yml` and run `dotnet watch` for live reload.

#### Scenario: Code changes reflect without rebuild
- **WHEN** editing a controller and saving
- **THEN** API restarts automatically within seconds; new code executes

### Requirement: Docker Compose override is gitignored
The system SHALL document that `docker-compose.override.yml` is for local overrides (ports, env vars) and must be in `.gitignore`.

#### Scenario: Override file is not tracked
- **WHEN** running `git status` with local override
- **THEN** `docker-compose.override.yml` does not appear

### Requirement: Environment variables use .env.development (gitignored)
The system SHALL use `.env.development` for local secrets (connection strings, Seq key) and `.env.example` as versioned template.

#### Scenario: Secrets not committed
- **WHEN** running `git status` with local .env.development
- **THEN** it does not appear; `.env.example` does appear

### Requirement: PostgreSQL service persists data in volume
The system SHALL define a named volume for PostgreSQL data in `docker-compose.yml`.

#### Scenario: Data survives container restart
- **WHEN** `docker compose down && docker compose up -d`
- **THEN** database data is intact

### Requirement: Image name follows convention
The system SHALL build image as `tfs/portfolio-api` (lowercase, kebab-case) and tag with semantic version, commit SHA, and latest.

#### Scenario: Image tags are correct
- **WHEN** running `docker images tfs/portfolio-api`
- **THEN** tags include `10.0.0`, `abc123f`, `latest`

### Requirement: GHCR publish works from CI
The system SHALL ensure Dockerfile and build args support `docker buildx build --push -t ghcr.io/<owner>/<repo>:<tag> .` in GitHub Actions.

#### Scenario: CI can publish to GHCR
- **WHEN** GitHub Actions workflow runs docker buildx
- **THEN** image appears in GHCR with correct tags