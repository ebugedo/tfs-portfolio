# Proposal

## Why

The API currently runs locally via Docker Compose for development but lacks a production deployment pipeline. This change establishes a complete CI/CD workflow to build, test, and deploy the API to a VPS using Docker containers, GitHub Container Registry (GHCR) for image storage, and GitHub Actions for automation.

## What Changes

- **New GitHub Actions workflow** (`.github/workflows/ci-cd.yml`): CI job runs unit + integration tests, builds multi-stage Docker image, pushes to GHCR with SHA and `latest` tags. CD job deploys to VPS via SSH using direct Docker CLI commands.
- **Updated deployment standards** (`openspec/standards/deployment-and-ci-cd.md`): Revise Sections 4 (GitHub Secrets) and 6 (VPS Production Deployment) to reflect PostgreSQL as separate container, no Seq logging, no `.env` file on VPS, migrations on startup, GHCR private registry.
- **Program.cs modification**: Remove `IsDevelopment()` guard so `context.Database.Migrate()` runs automatically on every container start (including Production).
- **GitHub Secrets configuration**: Add secrets for VPS access, GHCR authentication, PostgreSQL credentials, and API port.
- **VPS one-time setup**: Create Docker network, run PostgreSQL container with persistent volume (manual step, documented in standards).

## Capabilities

### New Capabilities
- `deployment/ci-cd-pipeline`: GitHub Actions workflow for build, test, push to GHCR, and SSH-based deployment to VPS
- `deployment/vps-docker-direct`: Direct Docker CLI deployment on VPS without Docker Compose, with PostgreSQL as separate container on shared network
- `deployment/github-secrets-management`: All sensitive configuration via GitHub Repository Secrets, no `.env` files on VPS

### Modified Capabilities
- `containerization`: Update standards for production deployment (no Compose, no Seq, migrations on startup, private GHCR)
- `database-and-migrations`: Migrations now run automatically on container startup in all environments

## Impact

- **Code**: `src/Presentation/WebAPI/Program.cs` (remove `IsDevelopment()` guard for migrations)
- **CI/CD**: New `.github/workflows/ci-cd.yml` workflow file
- **Standards**: `openspec/standards/deployment-and-ci-cd.md` (Sections 4 & 6)
- **Infrastructure**: VPS requires Docker, Docker network `app-network`, PostgreSQL container, Nginx reverse proxy (already configured)
- **Secrets**: 10 GitHub Repository Secrets required
- **Registry**: GHCR private images, VPS authenticates with PAT