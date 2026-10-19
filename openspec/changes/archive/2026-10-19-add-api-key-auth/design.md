# Design

## Context

See proposal.md - Why. Current state (from codebase exploration):
- No authentication or authorization exists; `app.UseAuthorization()` is commented out at `Program.cs:145`.
- Middleware pipeline: `MapOpenApi` → `MapScalarApiReference` → `UseErrorHandling` → `UseSerilogRequestLogging` → `UseHttpsRedirection` → `UseRouting` → `[UseAuthorization commented out]` → `MapControllers` → `MapHealthChecks` → `MapGet("/test")`.
- Scalar/OpenAPI at `/scalar/v1` with DeepSpace theme, no security schemes defined.
- Five controllers under `/api/v1/` with GET/POST/PUT/DELETE/PATCH endpoints; none protected by `[Authorize]`.
- `appsettings.json` and `appsettings.Development.json` exist with `ConnectionStrings`, `Seq`, `Serilog`, `AllowedHosts` sections; no auth-related keys.
- CI/CD (`.github/workflows/ci-cd.yml`) injects database and Seq secrets via GitHub Secrets; deploys to VPS with `ASPNETCORE_ENVIRONMENT=Production`.
- Dockerfiles set `ASPNETCORE_URLS` and `ASPNETCORE_ENVIRONMENT` environment variables; no auth-related env vars.

## Goals / Non-Goals

**Goals:**
- Require a valid `X-API-Key` header on all POST, PUT, DELETE, PATCH requests to `/api/v1/` endpoints.
- Allow GET requests without any authentication (fully public).
- Return `401 Unauthorized` when the API Key is missing or invalid.
- Read the API Key from configuration (`ApiKeySettings:MasterKey`): user-secrets or `appsettings.Development.json` (gitignored) in development; environment variable `ApiKeySettings__MasterKey` in production/CI/CD.
- Document the `X-API-Key` requirement in Scalar/OpenAPI for protected endpoints.
- No new NuGet packages beyond what already exists.

**Non-Goals:**
- JWT bearer authentication or token issuance.
- Role-based or policy-based authorization (`[Authorize]` attributes).
- User registration, login, or API Key management UI.
- API Key rotation, expiration, or per-client keys.
- Protecting health checks or the `/test` endpoint.

## Decisions

**Custom middleware over authorization filter/policy:**
- Middleware can inspect the HTTP method before routing and short-circuit non-GET requests without an API Key, returning `401 Unauthorized` early.
- An authorization filter/policy would run after model binding and routing, adding unnecessary overhead for a simple header check.
- Alternative considered: `[Authorize]` attribute with a custom policy on each write controller/action. Rejected because it requires decorating every write endpoint individually and does not naturally exclude GET endpoints.

**Middleware placement: after `UseRouting()`, before `MapControllers()`:**
- Placing after `UseRouting()` ensures route data is available if needed later.
- Placing before `MapControllers()` ensures the check runs before any controller action executes.
- Health checks and `/test` are mapped after `MapControllers()` and are not affected by the middleware (they use endpoint routing directly).

**Configuration key: `ApiKeySettings:MasterKey`:**
- Reads via `IConfiguration["ApiKeySettings:MasterKey"]` from `builder.Configuration`.
- Development: `dotnet user-secrets` or `appsettings.Development.json` (added to `.gitignore`).
- Production: environment variable `ApiKeySettings__MasterKey` injected by CI/CD pipeline and Docker.

**Response: `401 Unauthorized` (not `403 Forbidden`):**
- `401` indicates missing or invalid authentication credentials, which matches the API Key use case.
- `403` would imply authenticated but unauthorized, which is not applicable here.

**Scalar/OpenAPI security scheme:**
- Add `X-API-Key` as an `apiKey` security scheme in the OpenAPI document via `AddOpenApi` document transformer.
- Apply the security requirement globally to POST/PUT/DELETE/PATCH operations; exclude GET.

## Risks / Trade-offs

- [API Key value accidentally logged] → Middleware must not log the header value; only log presence/absence at Debug level.
- [API Key not injected in production → all write endpoints return 401] → Add startup validation: if `ApiKeySettings:MasterKey` is missing in non-Development environments, throw a clear configuration error.
- [GET endpoints with sensitive data exposed] → GET is public by design; document this contract so consumers do not place sensitive data in GET responses.
- [Middleware bypassed if placed incorrectly] → Unit/integration tests must verify 401 on write endpoints without the key and 200 on GET endpoints without the key.

## Migration Plan

1. Add `ApiKeySettings:MasterKey` to `appsettings.Development.json` (gitignored) and configure user-secrets for local development.
2. Create `src/Presentation/WebAPI/Middleware/ApiKeyMiddleware.cs`.
3. Register the middleware in `Program.cs` after `UseRouting()` and before `MapControllers()`.
4. Add `X-API-Key` security scheme to the OpenAPI document in `Program.cs`.
5. Inject `ApiKeySettings__MasterKey` environment variable in CI/CD (`.github/workflows/ci-cd.yml`), `Dockerfile`, and `docker-compose.yml`.
6. Add integration tests for authorized/unauthorized access to write endpoints.
7. Deploy; verify `/scalar/v1` shows `X-API-Key` requirement on protected endpoints.

**Rollback:** Remove the middleware registration from `Program.cs`, delete `ApiKeyMiddleware.cs`, revert `appsettings.Development.json` and CI/CD/Docker changes. All write endpoints become public again.

## Open Questions

None.
