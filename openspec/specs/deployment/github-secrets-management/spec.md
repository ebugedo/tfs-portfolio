# github-secrets-management Specification

## Purpose
Defines management of all sensitive configuration exclusively through GitHub Repository Secrets, with no `.env` files on the VPS.

## Requirements

### Requirement: All secrets stored in GitHub Repository Secrets
The system SHALL store all sensitive deployment parameters exclusively as GitHub Repository Secrets, never hardcoded in repository files or `.env` files on the VPS.

#### Scenario: No secrets in repository files
- **WHEN** searching repository for connection strings, passwords, SSH keys
- **THEN** no sensitive values found in any committed file

#### Scenario: No .env file on VPS
- **WHEN** checking VPS deployment directory
- **THEN** no `.env` file exists; all config passed via `docker run -e`

### Requirement: VPS access secrets configured
The system SHALL require the following GitHub Secrets for VPS access:
- `VPS_HOST`: IP address or domain name of the remote VPS
- `VPS_USERNAME`: SSH system user for deployment commands
- `VPS_SSH_KEY`: Private SSH key for passwordless authentication
- `VPS_PORT`: SSH connection port (default 22)

#### Scenario: SSH connection succeeds with secrets
- **WHEN** GitHub Actions uses `appleboy/ssh-action` with VPS secrets
- **THEN** SSH connection establishes successfully

### Requirement: GHCR authentication secrets configured
The system SHALL require `GHCR_PAT` secret with `write:packages`, `read:packages`, `delete:packages` scopes for pushing/pulling images.

#### Scenario: GHCR push succeeds with PAT
- **WHEN** CI runs `docker login ghcr.io` with `GHCR_PAT`
- **THEN** authentication succeeds and image pushes

#### Scenario: GHCR pull succeeds on VPS
- **WHEN** CD runs `docker login ghcr.io` with `GHCR_PAT` on VPS
- **THEN** authentication succeeds and image pulls

### Requirement: PostgreSQL credentials in secrets
The system SHALL require the following GitHub Secrets for PostgreSQL:
- `POSTGRES_PASSWORD`: PostgreSQL superuser password
- `POSTGRES_DB`: Database name (e.g., `portfolio`)
- `POSTGRES_USER`: PostgreSQL user (e.g., `postgres`)

#### Scenario: Connection string constructed from secrets
- **WHEN** CD constructs connection string from PostgreSQL secrets
- **THEN** format is `Host=postgres;Port=5432;Database=<POSTGRES_DB>;Username=<POSTGRES_USER>;Password=<POSTGRES_PASSWORD>`

### Requirement: API port configurable via secret
The system SHALL require `API_PORT` secret for the host port binding.

#### Scenario: API port from secret
- **WHEN** CD runs `docker run -p 127.0.0.1:${{ secrets.API_PORT }}:8080`
- **THEN** API binds to the configured host port
