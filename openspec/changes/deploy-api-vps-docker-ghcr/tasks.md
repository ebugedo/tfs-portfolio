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

## 7. Documentation & Cleanup

- [x] 7.1 Update README.md with deployment overview and secret requirements — verify README renders correctly
- [x] 7.2 Verify `.dockerignore` excludes `.env*`, `docker-compose.override.yml` — verify with `docker build` context size

## 8. PostgreSQL Host and Port Secrets

- [x] 8.1 Update CI/CD workflow to use `POSTGRES_HOST` and `POSTGRES_PORT` secrets instead of hardcoded `postgres` and `5432` — verify workflow runs successfully
- [x] 8.2 Remove `docker network create` and `docker network connect` from CI/CD workflow — the `nginx-net` network is created manually on the VPS and not managed by the pipeline
- [x] 8.3 Add `needs: ci` and `if: success()` to CD job so CD runs only after CI succeeds on push to main
- [x] 9.1 Delete all EF Core migrations and model snapshot (including VS-created XXXXX migrations); fix .csproj to include Migrations\*.cs in compilation (was excluded, causing Migrate() to not find migrations)
- [x] 9.2 Create new initial EF Core migration `20261018102041_InitialCreate` + `ApplicationDbContextModelSnapshot.cs` with all tables (Clients, CompanyProfiles, Sectors, Services, Projects, ProjectServices) — fixed broken `using ;` and anonymous type column `Technologies`