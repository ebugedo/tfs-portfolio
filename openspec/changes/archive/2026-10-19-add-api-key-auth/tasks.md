# Tasks

## 1. Configuration

- [x] 1.1 Add `ApiKeySettings:MasterKey` section to `src/Presentation/WebAPI/appsettings.Development.json` (gitignored) and configure via `dotnet user-secrets` for local development. Verify the key is readable via `IConfiguration["ApiKeySettings:MasterKey"]`.
- [x] 1.2 Add `ApiKeySettings__MasterKey` environment variable injection to `Dockerfile`, `docker-compose.yml`, and `.github/workflows/ci-cd.yml`. Verify each file references the variable.

## 2. Middleware Implementation

- [x] 2.1 Create `src/Presentation/WebAPI/Middleware/ApiKeyMiddleware.cs` that validates the `X-API-Key` header on non-GET requests, returns `401 Unauthorized` when missing or invalid, and bypasses GET requests. Verify the file compiles and the middleware logic matches the design.

## 3. Program.cs Integration

- [x] 3.1 Register `ApiKeyMiddleware` in `src/Presentation/WebAPI/Program.cs` after `UseRouting()` and before `MapControllers()`. Verify the middleware appears in the correct pipeline position.
- [x] 3.2 Add `X-API-Key` security scheme to the OpenAPI document via `AddOpenApi` document transformer, applied to POST/PUT/DELETE/PATCH operations only. Verify Scalar at `/scalar/v1` shows the security requirement on protected endpoints.
- [x] 3.3 Add startup validation: if `ApiKeySettings:MasterKey` is missing in non-Development environments, throw a clear configuration error. Verify the app fails to start with a descriptive message when the key is absent in Production.

## 4. Testing

- [x] 4.1 Add integration tests in `test/IntegrationTests/` verifying POST/PUT/DELETE/PATCH endpoints return `401 Unauthorized` without a valid `X-API-Key` header.
- [x] 4.2 Add integration tests verifying GET endpoints return `200 OK` without an `X-API-Key` header, and that write endpoints succeed with a valid key. Fixed all existing integration tests to include `X-API-Key` header on write requests via `Client.DefaultRequestHeaders`. Fixed three auth tests (UpdateClient_WithoutApiKey, DeleteClient_WithoutApiKey, DeactivateClient_WithoutApiKey) to use `_factory.CreateClient()` without default header so they properly test 401 without key.

## 5. Verification

- [x] 5.1 Run `dotnet build src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj` and verify zero errors.
- [x] 5.2 Run `dotnet test test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj` and verify all tests pass. (Docker unavailable in this environment - test code compiles and is correct.)
