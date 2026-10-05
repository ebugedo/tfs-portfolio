# C# & Code Quality Standards

## Language
- **Code identifiers** (classes, interfaces, methods, properties, variables, enums, parameters) MUST be in **English** (e.g., `Client`, `Project`, `GetByIdAsync`, `IsActive`).
- **Comments & documentation** SHOULD be in English.
- **Domain terms** in ubiquitous language may remain in Spanish in comments/docs, but code symbols use English equivalents.
- **API contracts** (JSON properties, query params, headers) use camelCase English.

## Naming Conventions
- **Classes, Interfaces, Methods & Properties:** PascalCase (e.g., `GetUserByIdQuery`, `IUserRepository`).
- **Interfaces:** Prefix with `I` (e.g., `IUnitOfWork`).
- **Private Fields:** camelCase with leading underscore (e.g., `_dbContext`).
- **Local Variables & Parameters:** camelCase (e.g., `userId`, `command`).

## Error & Exception Handling
- Use custom Domain Exceptions (`DomainException`) for business rule violations.
- Never catch generic `System.Exception` without rethrowing or handling appropriately.
- Validation failures must return standard HTTP 400 (ProblemDetails) in Presentation Layer.

## Async & Concurrency
- All I/O operations (database access, external APIs) MUST be asynchronous using `async`/`await`.
- Append `Async` to all asynchronous method names (e.g., `GetByIdAsync`).