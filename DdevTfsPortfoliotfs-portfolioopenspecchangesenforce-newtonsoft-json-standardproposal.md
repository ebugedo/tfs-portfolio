# Proposal

## Why

The project's technology stack standard (`openspec/standards/tech-stack.md`) currently states:
> **JSON Serialization:** Newtonsoft.Json (Json.NET) - Required for all API serialization, configuration, and logging.

However, the codebase violates this standard in multiple places:
- Integration tests use `System.Text.Json` via `PostAsJsonAsync`/`ReadFromJsonAsync` (from `System.Net.Http.Json`)
- EF Core value converters in the Persistence layer use `System.Text.Json.JsonSerializer` directly
- This causes serialization mismatches where `TfsWebUrl?` properties serialize as empty objects `{}` instead of `null`, breaking API contracts and causing test failures

The standard needs to be updated to explicitly require Newtonsoft.Json throughout the **entire system** (not just API serialization, configuration, and logging), and the implementation needs to be brought into compliance.

## What Changes

1. **Update `openspec/standards/tech-stack.md`** - Modify the JSON Serialization line to explicitly require Newtonsoft.Json for all serialization/deserialization across the entire solution (domain, application, infrastructure, API, tests)

2. **Add implementation task to `implement-portfolio-domain` change** - Create a task to:
   - Replace all `System.Text.Json` usage with Newtonsoft.Json
   - Update integration tests to use Newtonsoft.Json for HTTP requests/responses
   - Update EF Core value converters to use Newtonsoft.Json
   - Ensure all DTOs have proper Newtonsoft.Json converter attributes
   - Verify API layer remains compliant (already uses Newtonsoft.Json via `AddNewtonsoftJson`)

## Capabilities

### Modified Capabilities

- `solution-structure`: The technology stack standard is being updated to enforce Newtonsoft.Json universally

## Impact

**Affected Files:**
- `openspec/standards/tech-stack.md` - The standard document itself
- `test/IntegrationTests/IntegrationTestBase.cs` - Uses `PostAsJsonAsync`/`ReadFromJsonAsync` (STJ)
- `test/IntegrationTests/Api/*.cs` - All API tests using STJ methods
- `src/Infrastructure/Persistence/Configurations/ClientConfiguration.cs` - EF Core converters using STJ
- `src/Infrastructure/Persistence/Configurations/ProjectConfiguration.cs` - EF Core converters using STJ
- `src/Infrastructure/Persistence/Configurations/CompanyProfileConfiguration.cs` - EF Core converters using STJ
- `src/Core/Application/*/Dtos/*.cs` - Need verification of Newtonsoft.Json attributes
- `src/Presentation/WebAPI/Program.cs` - Already compliant (uses `AddNewtonsoftJson`)

**Dependencies:**
- Newtonsoft.Json already in Domain, API, and IntegrationTests projects
- System.Text.Json usage needs to be removed/replaced throughout
