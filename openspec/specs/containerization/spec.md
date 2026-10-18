# containerization Specification

## Purpose
Defines containerization for development and production: multi-stage Dockerfile, Docker Compose for local development (API + PostgreSQL + Seq), and image publishing to GHCR.

## Requirements

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
The system SHALL provide `docker-compose.yml` at repo root for **local development only** with services: `api` (build from Dockerfile.dev), `postgres` (PostgreSQL 16). Seq is optional for local development. **Production deployment uses direct Docker CLI without Docker Compose.**

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
The system SHALL use `.env.development` for local secrets (connection strings) and `.env.example` as versioned template. **Seq key is optional for local development. No `.env` files on VPS - all production config via GitHub Secrets and `docker run -e`.**

#### Scenario: Secrets not committed
- **WHEN** running `git status` with local .env.development
- **THEN** it does not appear; `.env.example` does appear

#### Scenario: No .env file on VPS
- **WHEN** checking VPS deployment
- **THEN** no `.env` file exists; all config passed via `docker run -e` from GitHub Secrets

### Requirement: PostgreSQL service persists data in volume
The system SHALL define a named volume for PostgreSQL data in `docker-compose.yml` **for local development**. **On VPS, PostgreSQL uses a separate persistent volume managed independently.**

#### Scenario: Data survives container restart
- **WHEN** `docker compose down && docker compose up -d`
- **THEN** database data is intact

#### Scenario: VPS PostgreSQL data persists
- **WHEN** `docker stop postgres && docker start postgres` on VPS
- **THEN** database data is intact in `postgres_data` volume

### Requirement: Image name follows convention
The system SHALL build image as `ghcr.io/<owner>/<repo>` (lowercase, kebab-case) and tag with commit SHA (`sha-<sha>`) and `latest`. Semantic version tags pushed when Git tag created.

#### Scenario: Image tags are correct
- **WHEN** running `docker images tfs/portfolio-api`
- **THEN** tags include `10.0.0`, `abc123f`, `latest`

#### Scenario: GHCR image tags
- **WHEN** CI pushes to GHCR
- **THEN** tags include `sha-<commit-sha>` and `latest`

### Requirement: GHCR publish works from CI
The system SHALL ensure Dockerfile and build args support `docker buildx build --push -t ghcr.io/<owner>/<repo>:<tag> .` in GitHub Actions.

#### Scenario: CI can publish to GHCR
- **WHEN** GitHub Actions workflow runs docker buildx
- **THEN** image appears in GHCR with correct tags

### Requirement: Production deployment uses direct Docker CLI via SSH
The system SHALL deploy to production VPS using direct Docker CLI commands executed over SSH, without Docker Compose.

#### Scenario: VPS deployment via SSH
- **WHEN** CD workflow runs
- **THEN** SSH connects to VPS and executes `docker pull`, `docker stop`, `docker rm`, `docker run`, `docker image prune`

### Requirement: GHCR is private registry requiring authentication
The system SHALL authenticate to GHCR using PAT both in CI (push) and on VPS (pull).

#### Scenario: CI authenticates to GHCR
- **WHEN** CI runs `docker login ghcr.io` with `GITHUB_TOKEN`
- **THEN** push succeeds

#### Scenario: VPS authenticates to GHCR
- **WHEN** CD runs `docker login ghcr.io` with `GHCR_PAT` on VPS
- **THEN** pull succeeds

### Requirement: Seq logging omitted in production
The system SHALL NOT include Seq in production deployment. Seq is optional for local development only.

#### Scenario: No Seq in production
- **WHEN** checking VPS containers
- **THEN** no Seq container running

#### Scenario: API disables Seq in production
- **WHEN** checking API container environment
- **THEN** `Seq__ServerUrl=""` and `Seq__ApiKey=""`
