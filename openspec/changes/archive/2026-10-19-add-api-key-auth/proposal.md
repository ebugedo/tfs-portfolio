# Proposal

## Why

The Tfs.Portfolio API currently has no authentication or authorization (`app.UseAuthorization()` is commented out in `Program.cs:145`). All endpoints are publicly accessible, including write operations that modify data. An API Key mechanism is needed to protect mutation endpoints (POST, PUT, DELETE, PATCH) while keeping read endpoints (GET) publicly available, without hardcoding secrets in source control.

## What Changes

- Add a custom middleware or authorization filter that validates the `X-API-Key` header on non-GET requests
- Configure the API Key from `dotnet user-secrets` (development) or environment variable `ApiKeySettings:MasterKey` (production)
- Return `401 Unauthorized` when the API Key is missing or invalid
- Exclude GET requests from API Key validation (fully public)
- Add `X-API-Key` security scheme to OpenAPI/Scalar documentation so consumers know protected endpoints require the header
- Add integration tests covering authorized and unauthorized access to protected endpoints

## Capabilities

### Modified Capabilities
- `api-layer`: Authentication behavior changes — POST/PUT/DELETE/PATCH endpoints now require a valid `X-API-Key` header; GET endpoints remain public. This changes the security requirements of the API layer.

## Impact

- Code: `src/Presentation/WebAPI/Program.cs` (middleware registration, `AddAuthentication`/`AddAuthorization` services)
- New file: `src/Presentation/WebAPI/Middleware/ApiKeyMiddleware.cs` (custom API Key validation middleware)
- Configuration: `appsettings.json`, `appsettings.Development.json` (add `ApiKeySettings` section), user-secrets for local dev
- CI/CD: `.github/workflows/ci-cd.yml` (inject `ApiKeySettings__MasterKey` via GitHub Secrets)
- Docker: `Dockerfile`, `docker-compose.yml` (inject API Key via environment variable)
- Tests: `test/IntegrationTests/` (add API Key auth test scenarios)
- Scalar/OpenAPI: Add `X-API-Key` security definition and requirement to OpenAPI document
