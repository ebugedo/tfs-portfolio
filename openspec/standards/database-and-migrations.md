# Database & EF Core Standards

## Database Technology
- **Engine:** PostgreSQL using `Npgsql.EntityFrameworkCore.PostgreSQL`.

## Entity Framework Core Rules
- **Configurations:** Entity configurations MUST be isolated using Fluent API in `IEntityTypeConfiguration<T>` classes within Infrastructure. Do NOT use Data Annotations in Domain entities.
- **Primary Keys:** Standardize on `Guid` or `long` (identity) for Primary Keys.
- **Soft Delete:** Implement soft-deleting via query filters where required by business rules.

## Migrations Standard
**Purpose:** Establish rules for generating and maintaining Entity Framework Core migrations in the project, ensuring that `context.Database.Migrate()` works correctly in any environment (development, CI, production VPS).

### 1. One initial migration per empty database
When the target database is empty, there MUST be exactly **one** initial migration containing all the required tables (`CreateTable`). Multiple partial migrations are not allowed, nor migrations named `XXXXX` or placeholder names.

### 2. Do not leave orphaned migrations
Before creating a new initial migration, delete:
- All `.cs` and `.Designer.cs` files in `src/Infrastructure/Persistence/Migrations/`
- The `ApplicationDbContextModelSnapshot.cs` file

### 3. Authorized commands to generate migrations
Use exclusively:

```bash
dotnet ef migrations add <Name> \
  --project "src\Infrastructure\Persistence" \
  --startup-project "src\Presentation\WebAPI"
```

Valid names: `InitialCreate`, or descriptive names (e.g., `AddClientsTable`, `AddProjectsTable`). **Prohibited** to use `XXXXX` or meaningless names.

### 4. Mandatory verification after generating
After running `dotnet ef migrations add`, review the generated `.cs` file and correct:

| Common issue | Fix |
|--------------|-----|
| Broken `using ;` line | Remove the entire line |
| Anonymous type `List<<>f__AnonymousType0<...>>>` | Replace with the real type (e.g., `string` with JSONB converter) |
| Incomplete `using ` | Complete or remove |

### 5. Build verification
After correcting the migration, run:

```bash
dotnet build Tfs.Portfolio.slnx --configuration Release
```

It MUST finish with **0 errors**.

### 6. Model snapshot
The `ApplicationDbContextModelSnapshot.cs` file is generated automatically with `dotnet ef migrations add`. Do NOT edit it manually. If it is missing, regenerate the initial migration.

### 7. Prohibited
- Create migrations named `XXXXX` or placeholder names
- Have multiple migration files that create the same tables
- Leave the snapshot out of date with respect to the entity model
- Commit migrations with build errors to the remote repository

- Migrations MUST be generated using `dotnet ef migrations add <MigrationName>`.
- NEVER modify existing migration files that have already been merged to the main branch.
