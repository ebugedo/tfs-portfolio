# Spec Delta

## MODIFIED Requirements

### Requirement: The system SHALL provide `docker-compose.yml` at repo root with services: `api` (build from Dockerfile), `postgres` (PostgreSQL 16), `seq` (Seq latest).
**Updated:** The system SHALL provide `docker-compose.yml` at repo root for **local development only** with services: `api` (build from Dockerfile.dev), `postgres` (PostgreSQL 16). Seq is optional for local development. **Production deployment uses direct Docker CLI without Docker Compose.**

#### Scenario: Docker Compose for development only
- **WHEN** running `docker compose up -d` locally
- **THEN** API and PostgreSQL start; API accessible at `http://localhost:8082`

#### Scenario: No Docker Compose on VPS
- **WHEN** deploying to VPS
- **THEN** no `docker-compose.yml` is used; direct `docker run` commands execute via SSH

### Requirement: The system SHALL document that `docker-compose.override.yml` is for local overrides (ports, env vars) and must be in `.gitignore`.
**Unchanged:** Still required for local development overrides.

### Requirement: The system SHALL use `.env.development` for local secrets (connection strings, Seq key) and `.env.example` as versioned template.
**Updated:** The system SHALL use `.env.development` for local secrets (connection strings) and `.env.example` as versioned template. **Seq key is optional for local development. No `.env` files on VPS - all production config via GitHub Secrets and `docker run -e`.**

#### Scenario: Local development uses .env.development
- **WHEN** running `docker compose up -d` locally
- **THEN** `.env.development` provides local connection strings

#### Scenario: No .env file on VPS
- **WHEN** checking VPS deployment
- **THEN** no `.env` file exists; all config passed via `docker run -e` from GitHub Secrets

### Requirement: The system SHALL define a named volume for PostgreSQL data in `docker-compose.yml`.
**Updated:** The system SHALL define a named volume for PostgreSQL data in `docker-compose.yml` **for local development**. **On VPS, PostgreSQL uses a separate persistent volume managed independently.**

#### Scenario: Local PostgreSQL data persists
- **WHEN** `docker compose down && docker compose up -d` locally
- **THEN** database data is intact in local named volume

#### Scenario: VPS PostgreSQL data persists
- **WHEN** `docker stop postgres && docker start postgres` on VPS
- **THEN** database data is intact in `postgres_data` volume

### Requirement: The system SHALL build image as `tfs/portfolio-api` (lowercase, kebab-case) and tag with semantic version, commit SHA, and latest.
**Updated:** The system SHALL build image as `ghcr.io/<owner>/<repo>` (lowercase, kebab-case) and tag with commit SHA (`sha-<sha>`) and `latest`. Semantic version tags pushed when Git tag created.

#### Scenario: GHCR image tags
- **WHEN** CI pushes to GHCR
- **THEN** tags include `sha-<commit-sha>` and `latest`

### Requirement: The system SHALL ensure Dockerfile and build args support `docker buildx build --push -t ghcr.io/<owner>/<repo>:<tag> .` in GitHub Actions.
**Unchanged:** Multi-stage Dockerfile supports buildx push to GHCR.

## ADDED Requirements

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