# Design

## Context

See proposal.md - Why. The API currently runs locally via Docker Compose for development but lacks production deployment automation. The existing `containerization` spec defines Docker and Docker Compose for local development; this change extends it with CI/CD pipeline and VPS deployment via direct Docker CLI.

Current state:
- Multi-stage Dockerfile exists (build + runtime stages, non-root user)
- `docker-compose.yml` for local dev (API + PostgreSQL + Seq)
- `docker-compose.test.yml` for integration tests
- GitHub Actions not configured
- VPS has Docker installed, Nginx configured as reverse proxy
- PostgreSQL will run as separate container on VPS

## Goals / Non-Goals

**Goals:**
- Automated CI/CD pipeline via GitHub Actions
- Unit + integration tests run on every push/PR
- Docker image built and pushed to GHCR (private) with SHA and latest tags
- VPS deployment via SSH using direct Docker CLI (no Compose)
- PostgreSQL as separate container on shared Docker network
- All secrets in GitHub Repository Secrets, no .env files on VPS
- Automatic EF Core migrations on container startup in all environments
- Seq logging disabled in production

**Non-Goals:**
- Kubernetes or orchestration tools
- Docker Compose on VPS
- Seq logging in production
- Database backup/restore automation (manual for now)
- Blue-green or canary deployments (simple stop/start/replace)
- TLS termination in API container (handled by Nginx)

## Decisions

### 1. CI/CD Platform: GitHub Actions
**Choice:** GitHub Actions workflows in `.github/workflows/ci-cd.yml`
**Rationale:** Native GitHub integration, free for public repos, secrets management built-in, marketplace actions for SSH deployment.
**Alternative considered:** GitLab CI, Azure DevOps, Jenkins - rejected due to additional setup/maintenance.

### 2. Integration Tests in CI: GitHub Actions Service Container
**Choice:** Use `services.postgres` in workflow YAML instead of `docker-compose.test.yml`
**Rationale:** Simpler, no docker-compose dependency in CI, native GitHub Actions feature, faster startup.
**Alternative considered:** Run docker-compose in CI - rejected due to complexity and slower execution.

### 3. VPS Deployment: Direct Docker CLI via SSH
**Choice:** `appleboy/ssh-action` to run Docker commands on VPS
**Rationale:** Matches deployment-and-ci-cd.md standard (Section 6), no agent/installation on VPS, simple and auditable.
**Alternative considered:** Self-hosted runner on VPS - rejected due to security surface and maintenance.

### 4. Container Registry: GHCR Private
**Choice:** `ghcr.io/<owner>/<repo>` with PAT authentication
**Rationale:** Free private hosting for GitHub users, integrated with GitHub Actions `GITHUB_TOKEN`, PAT for VPS pull.
**Alternative considered:** Docker Hub, Azure Container Registry - rejected due to cost/complexity.

### 5. Image Tagging: SHA + Latest
**Choice:** Push both `sha-${{ github.sha }}` and `latest` tags from main branch
**Rationale:** SHA for immutable traceability, latest for simple deployment. Semantic tags on Git tags (future).
**Alternative considered:** Only semantic versions - rejected for main branch continuous deployment.

### 6. PostgreSQL on VPS: Separate Container on Shared Network
**Choice:** `docker network create nginx-net`, both containers attach to it
**Rationale:** Simple, no external dependency, DNS resolution via container name (`postgres`), persistent volume. Network shared with Nginx reverse proxy.
**Alternative considered:** Managed PostgreSQL (RDS, Cloud SQL) - rejected per user requirement.

### 7. Secrets: GitHub Repository Secrets Only
**Choice:** All config via `docker run -e` from secrets, no .env file on VPS
**Rationale:** Security best practice, audit trail in GitHub, no file management on VPS.
**Alternative considered:** .env file on VPS - rejected per user requirement.

### 8. Migrations: Automatic on Startup (All Environments)
**Choice:** Remove `IsDevelopment()` guard from `context.Database.Migrate()` in Program.cs
**Rationale:** Idempotent, safe, ensures schema matches code on every deploy. EF Core Migrate() is designed for this.
**Alternative considered:** Manual migration step before deploy - rejected due to operational complexity.

### 9. Seq Logging: Disabled in Production
**Choice:** Set `Seq__ServerUrl=""` and `Seq__ApiKey=""` in production container
**Rationale:** Simplifies production, reduces dependencies, console logging sufficient for Docker stdout collection.
**Alternative considered:** Keep Seq in production - rejected per user requirement.

### 10. API Port Binding: Localhost Only
**Choice:** `-p 127.0.0.1:${API_PORT}:8080` so only Nginx can reach API
**Rationale:** Security - API not exposed directly to internet, Nginx handles TLS/rate limiting/WAF.
**Alternative considered:** Bind to 0.0.0.0 - rejected for security.

### 11. Nginx: Pre-configured Reverse Proxy
**Choice:** User confirms Nginx already configured to proxy HTTPS → `http://localhost:8080`
**Rationale:** Separation of concerns, Nginx handles TLS, API focuses on application logic.
**Decision:** No changes to Nginx config in this change.

## Risks / Trade-offs

| Risk | Mitigation |
|------|------------|
| SSH key compromised | Use dedicated deploy user with minimal permissions, rotate keys periodically |
| GHCR PAT leaked | Store only in GitHub Secrets, use fine-grained PAT with minimal scopes |
| Migration failure on startup | EF Core Migrate() is idempotent; test migrations in CI; monitor deployment logs |
| VPS disk fills up | `docker image prune -f` runs after each deploy; monitor disk usage |
| PostgreSQL data loss | Persistent volume `postgres_data`; manual backup strategy documented separately |
| Network partition between API and PostgreSQL | Both on same host Docker network; low latency, high reliability |
| Nginx misconfiguration | Out of scope for this change; user manages Nginx |
| No rollback mechanism | `sha-<commit>` tags allow manual `docker run` of previous version if needed |

## Migration Plan

### Pre-deployment (One-time, Manual on VPS)
1. Create Docker network: `docker network create nginx-net`
2. Run PostgreSQL container with persistent volume
3. Verify Nginx proxies to `http://localhost:8080`

### Deployment (Automated via GitHub Actions)
1. Push to `main` triggers CI
2. CI: Unit tests → Integration tests → Build image → Push to GHCR
3. CD: SSH to VPS → Login GHCR → Pull latest → Stop/remove old → Run new → Prune

### Rollback (Manual)
```bash
# On VPS, to rollback to previous version:
docker stop portfolio-api
docker rm portfolio-api
docker run -d --name portfolio-api --network nginx-net --restart unless-stopped \
  -p 127.0.0.1:8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=portfolio;Username=postgres;Password=<pwd>" \
  -e Seq__ServerUrl="" -e Seq__ApiKey="" \
  ghcr.io/<owner>/<repo>:sha-<previous-commit-sha>
```

## Open Questions

None - all technical decisions resolved in this design.