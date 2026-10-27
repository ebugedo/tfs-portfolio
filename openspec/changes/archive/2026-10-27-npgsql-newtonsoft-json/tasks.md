# Tasks

## 1. Switch Npgsql JSON serializer to Newtonsoft.Json in WebAPI

- [x] 1.1 Add `using Npgsql;` to `src/Presentation/WebAPI/Program.cs`.
- [x] 1.2 Call `NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();` once at startup in `Main` (before `AddDbContext<ApplicationDbContext>`). `UseJsonNet()` does not exist in Npgsql 10; `EnableDynamicJson()` is the supported opt-in per the updated `tech-stack.md:6` (Newtonsoft wherever possible; Npgsql lacks a Newtonsoft switch), matching `CustomWebApplicationFactory.cs:62`.
- [x] 1.3 (If needed) Add direct `<PackageReference Include="Npgsql" .../>` to `src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj`. — Not needed: `Npgsql` types are available transitively via `Npgsql.EntityFrameworkCore.PostgreSQL`; build confirms.
- [x] 1.4 Build: `dotnet build src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj`.

## 2. Verification

- [x] 2.1 Run integration tests exercising `Project` persistence (e.g. `test/IntegrationTests/Persistence/ProjectRepositoryTests.cs`). — 50/50 passed, 0 failed. `NotSupportedException` resolved.
- [x] 2.2 Run `dotnet test test/UnitTests/...` to confirm no regression.
