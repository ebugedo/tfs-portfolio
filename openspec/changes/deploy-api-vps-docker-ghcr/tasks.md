# Tasks

## 1. Update Deployment Standards

- [x] 1.1 Update `openspec/standards/deployment-and-ci-cd.md` Section 4 (GitHub Secrets) with all required secrets: VPS_HOST, VPS_USERNAME, VPS_SSH_KEY, VPS_PORT, GHCR_PAT, POSTGRES_PASSWORD, POSTGRES_DB, POSTGRES_USER, API_PORT — verify file saves correctly
- [x] 1.2 Update `openspec/standards/deployment-and-ci-cd.md` Section 6 (VPS Production Deployment) with: PostgreSQL as separate container, no Seq, no .env file, migrations on startup, GHCR private, docker run command with -e from secrets — verify file saves correctly

## 2. Create GitHub Actions Workflow

- [x] 2.1 Create `.github/workflows/ci-cd.yml` with CI job: checkout, setup .NET 10, run unit tests (`dotnet test test/UnitTests`), run integration tests with PostgreSQL service container (`dotnet test test/IntegrationTests`), build/push Docker image to GHCR with SHA and latest tags — verify workflow syntax with `gh workflow lint` or GitHub Actions UI
- [x] 2.2 Add CD job to workflow: needs CI, runs on main branch only, uses `appleboy/ssh-action@v1` with VPS secrets, executes deployment script (login GHCR, pull, stop/rm old container, run new container with -e from secrets, prune images) — verify workflow syntax

## 3. Modify Program.cs for Automatic Migrations

- [x] 3.1 Edit `src/Presentation/WebAPI/Program.cs`: remove `if (app.Environment.IsDevelopment())` wrapper around `context.Database.Migrate()` so migrations run in all environments — verify change compiles with `dotnet build`
- [x] 3.2 Verify migration logic: run integration tests locally to confirm `Migrate()` executes on startup — verify tests pass

## 4. Configure GitHub Repository Secrets

- [ ] 4.1 Add VPS secrets in GitHub repo settings: VPS_HOST, VPS_USERNAME, VPS_SSH_KEY, VPS_PORT — verify secrets appear in Settings > Secrets and variables > Actions
- [ ] 4.2 Add GHCR_PAT with write:packages, read:packages, delete:packages scopes — verify PAT works with `docker login ghcr.io`
- [ ] 4.3 Add PostgreSQL secrets: POSTGRES_PASSWORD, POSTGRES_DB (portfolio), POSTGRES_USER (postgres) — verify secrets appear
- [ ] 4.4 Add API_PORT secret (8080) — verify secret appears

## 5. VPS One-Time Setup (Manual)

- [ ] 5.1 SSH to VPS and create Docker network: `docker network create nginx-net` — verify with `docker network ls`
- [ ] 5.2 Run PostgreSQL container on VPS: `docker run -d --name postgres --network nginx-net --restart unless-stopped -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=<secret> -e POSTGRES_DB=portfolio -v postgres_data:/var/lib/postgresql/data postgres:16-alpine` — verify with `docker ps` and `docker exec postgres pg_isready`
- [ ] 5.3 Verify Nginx proxies HTTPS to `http://localhost:8080` — verify with `curl https://<domain>/health/live`

## 6. First Deployment & Verification

- [ ] 6.1 Push to main branch to trigger CI/CD pipeline — verify workflow runs in GitHub Actions
- [ ] 6.2 Verify CI passes: unit tests, integration tests, Docker build, GHCR push — verify green checkmarks in Actions
- [ ] 6.3 Verify CD passes: SSH connects, GHCR pull, container replacement, prune — verify green checkmarks in Actions
- [ ] 6.4 Verify API responds on VPS: `curl https://<domain>/health/live` returns 200, `curl https://<domain>/health/ready` returns 200 — verify both endpoints healthy
- [ ] 6.5 Verify migrations applied: check PostgreSQL migration history table — verify `__EFMigrationsHistory` contains latest migration

## 7. Documentation & Cleanup

- [x] 7.1 Update README.md with deployment overview and secret requirements — verify README renders correctly
- [x] 7.2 Verify `.dockerignore` excludes `.env*`, `docker-compose.override.yml` — verify with `docker build` context size