# vps-docker-direct Specification

## Purpose
Defines production deployment on VPS using direct Docker CLI commands (no Docker Compose), with PostgreSQL as a separate container on a shared Docker network.

## Requirements

### Requirement: VPS runs PostgreSQL as separate container
The system SHALL run PostgreSQL in a dedicated container named `postgres` on the VPS, connected to a shared Docker network `nginx-net`, with persistent volume for data.

#### Scenario: PostgreSQL container running
- **WHEN** checking `docker ps` on VPS
- **THEN** container named `postgres` is running with image `postgres:16-alpine`

#### Scenario: PostgreSQL data persists across restarts
- **WHEN** `docker stop postgres && docker start postgres`
- **THEN** database data is intact in `postgres_data` volume

#### Scenario: PostgreSQL accessible via Docker network DNS
- **WHEN** API container connects to `Host=postgres;Port=5432`
- **THEN** connection succeeds using Docker network DNS resolution

### Requirement: VPS runs API container on shared network
The system SHALL run the API container named `portfolio-api` on the same `nginx-net` Docker network, binding to localhost only (127.0.0.1) on the configured port.

#### Scenario: API container running on nginx-net
- **WHEN** checking `docker ps` on VPS
- **THEN** container named `portfolio-api` is running and connected to `nginx-net`

#### Scenario: API binds to localhost only
- **WHEN** checking `docker port portfolio-api`
- **THEN** port mapping shows `127.0.0.1:<host-port>:8080/tcp`

#### Scenario: API accessible via Nginx reverse proxy
- **WHEN** Nginx proxies to `http://127.0.0.1:<host-port>`
- **THEN** API responds to public HTTPS requests

### Requirement: API container uses restart policy
The system SHALL configure the API container with `--restart unless-stopped` to ensure automatic recovery after failures or host reboots.

#### Scenario: Container restarts after crash
- **WHEN** API process crashes inside container
- **THEN** Docker restarts container automatically

#### Scenario: Container starts after host reboot
- **WHEN** VPS reboots
- **THEN** `portfolio-api` container starts automatically

### Requirement: No Seq logging in production
The system SHALL disable Seq logging in production by setting `Seq__ServerUrl` and `Seq__ApiKey` to empty strings.

#### Scenario: Seq disabled in production
- **WHEN** checking API container environment variables
- **THEN** `Seq__ServerUrl=""` and `Seq__ApiKey=""` are set

### Requirement: ASPNETCORE_ENVIRONMENT set to Production
The system SHALL set `ASPNETCORE_ENVIRONMENT=Production` in the API container.

#### Scenario: Production environment variable set
- **WHEN** checking API container environment
- **THEN** `ASPNETCORE_ENVIRONMENT=Production` is present
