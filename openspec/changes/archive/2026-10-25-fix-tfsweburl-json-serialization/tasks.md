# Tasks

## 1. Fix TfsWebUrlJsonConverter.ReadJson

- [x] 1.1 Modify `TfsWebUrlJsonConverter.ReadJson` to handle both string and JSON object formats: if the JSON token is an object with a `value` property, extract it; if it's a string, use it directly; verify the file compiles with `dotnet build src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj`

## 2. Verification

- [x] 2.1 Run `dotnet test test/UnitTests/Domain/UnitTests/ValueObjects/TfsWebUrlTests.cs` to verify existing unit tests pass
- [x] 2.2 Run `dotnet test test/IntegrationTests/Persistence/CompanyProfileRepositoryTests.cs` to verify integration tests pass with TfsWebUrl persistence
