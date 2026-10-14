# Database & EF Core Standards

## Database Technology
- **Engine:** PostgreSQL using `Npgsql.EntityFrameworkCore.PostgreSQL`.

## Entity Framework Core Rules
- **Configurations:** Entity configurations MUST be isolated using Fluent API in `IEntityTypeConfiguration<T>` classes within Infrastructure. Do NOT use Data Annotations in Domain entities.
- **Primary Keys:** Standardize on `Guid` or `long` (identity) for Primary Keys.
- **Soft Delete:** Implement soft-deleting via query filters where required by business rules.
- **Migrations:**
  - Migrations MUST be generated using `dotnet ef migrations add <MigrationName>`.
  - NEVER modify existing migration files that have already been merged to the main branch.