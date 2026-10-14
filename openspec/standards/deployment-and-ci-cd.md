# Infrastructure, Containerization & CI/CD Standards

## 1. Containerization (Docker)
- **Containerization Engine:** Docker.
- **Dockerfile Standards:**
  - Multi-stage builds are mandatory to produce minimal runtime footprint images.
  - Base Image: Official Microsoft .NET Runtime image (e.g., `mcr.microsoft.com/dotnet/aspnet`).
  - Build Image: Official Microsoft .NET SDK image (e.g., `mcr.microsoft.com/dotnet/sdk`).
  - Non-root user: The container runtime MUST execute using a non-privileged user.
  - `.dockerignore`: A strict `.dockerignore` file must be maintained at the repository root to exclude `bin/`, `obj/`, `.git/`, `.env`, and local settings.

## 2. Development Environment (Docker Compose)
- **Orchestration Tool:** Docker Compose (`docker-compose.yml`) for local development only.
- **Compose File Location:** Repository root, version-controlled.
- **Standard Services:**
  - API application container (built from local Dockerfile)
  - PostgreSQL database
  - Seq (structured logging)
- **Local Overrides:** `docker-compose.override.yml` MUST be `.gitignore`d for developer-specific configuration (ports, volumes, env vars).
- **Environment Variables:** `.env.development` (gitignored) for local secrets/config; `.env.example` (versioned) as template.
- **Hot Reload:** Source code mounted as volume with `dotnet watch` for live reloading in development.

## 3. Container Registry (GHCR)
- **Registry Provider:** GitHub Container Registry (`ghcr.io`).
- **Image Naming Convention:** `ghcr.io/<github-owner>/<repository-name>:<tag>`.
- **Image Tagging Strategy:**
  - Every build pushed from `main` MUST tag images with the commit SHA (`:${{ github.sha }}`) and `latest`.
  - Semantic release tags (e.g., `:v1.0.0`) MUST be pushed when a Git tag is created.

## 4. GitHub Secrets Management
All sensitive deployment parameters MUST be stored exclusively as GitHub Repository Secrets (`Settings > Secrets and variables > Actions`) and MUST NEVER be hardcoded in repository files:
- `VPS_HOST`: IP address or domain name of the remote VPS.
- `VPS_USERNAME`: SSH system user for executing deployment commands on the VPS.
- `VPS_SSH_KEY`: Private SSH key authorized for passwordless authentication to the VPS.
- `VPS_PORT`: SSH connection port (defaults to 22 if omitted).
- `GHCR_PAT` / `GITHUB_TOKEN`: Access token used for pushing and pulling images from GitHub Container Registry.

## 5. Continuous Integration & Continuous Deployment (GitHub Actions)
- **CI/CD Platform:** GitHub Actions workflows located in `.github/workflows/`.
- **Deployment Pipeline Workflow Steps:**
  1. **Build & Test:** Run `dotnet test` on the runner before building the image.
  2. **Authenticate & Publish:** Log in to `ghcr.io` and push the built Docker image.
  3. **SSH Remote Execution:** Establish an SSH connection to the VPS using `appleboy/ssh-action` (or equivalent standard action) authenticated via `secrets.VPS_HOST`, `secrets.VPS_USERNAME`, and `secrets.VPS_SSH_KEY`.

## 6. VPS Production Deployment (Direct Docker CLI via SSH)
- **Environment:** Production only. Contrasts with development (Docker Compose) — no Compose on VPS.
- **Deployment Approach:** Direct Docker CLI commands executed over SSH without using Docker Compose.
- **Workflow Executed on the VPS via SSH:**
  1. **Authenticate to GHCR:**
     ```bash
     echo "${{ secrets.GHCR_PAT }}" | docker login ghcr.io -u "${{ github.actor }}" --password-stdin
     ```
  2. **Pull Updated Container Image:**
     ```bash
     docker pull ghcr.io/<owner>/<repo>:latest
     ```
  3. **Stop & Remove Existing Container (if active):**
     ```bash
     docker stop <container-name> || true
     docker rm <container-name> || true
     ```
  4. **Run New Container:**
     ```bash
     docker run -d \
       --name <container-name> \
       --restart unless-stopped \
       -p <host-port>:<container-port> \
       -e ASPNETCORE_ENVIRONMENT=Production \
       --env-file /path/to/vps/.env \
       ghcr.io/<owner>/<repo>:latest
     ```
  5. **Prune Unused Images:**
     ```bash
     docker image prune -f
     ```