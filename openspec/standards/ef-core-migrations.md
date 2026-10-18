# EF Core Migrations Standard

## Propósito

Establecer las reglas para generar y mantener migraciones de Entity Framework Core en el proyecto, asegurando que `context.Database.Migrate()` funcione correctamente en cualquier entorno (desarrollo, CI, VPS de producción).

## Reglas obligatorias

### 1. Una sola migración inicial por base de datos vacía
Cuando la base de datos de destino está vacía, debe existir exactamente **una** migración inicial que contenga todas las tablas (`CreateTable`) necesarias. No se permiten múltiples migraciones parciales ni migraciones con nombre `XXXXX` o nombres de placeholder.

### 2. No dejar migraciones huérfanas
Antes de crear una nueva migración inicial, eliminar:
- Todos los archivos `.cs` y `.Designer.cs` en `src/Infrastructure/Persistence/Migrations/`
- El archivo `ApplicationDbContextModelSnapshot.cs`

### 3. Comandos autorizados para generar migraciones
Usar exclusivamente:

```bash
dotnet ef migrations add <Nombre> \
  --project "src\Infrastructure\Persistence" \
  --startup-project "src\Presentation\WebAPI"
```

Nombres válidos: `InitialCreate`, o nombres descriptivos (ej. `AddClientsTable`, `AddProjectsTable`). **Prohibido** usar `XXXXX` o nombres sin significado.

### 4. Verificación obligatoria después de generar
Después de ejecutar `dotnet ef migrations add`, revisar el archivo `.cs` generado y corregir:

| Problema común | Corrección |
|----------------|------------|
| Línea `using ;` rota | Eliminar la línea completa |
| Tipo anónimo `List<<>f__AnonymousType0<...>>>` | Reemplazar por el tipo real (ej. `string` con conversor JSONB) |
| `using ` incompleto | Completar o eliminar |

### 5. Verificación de compilación
Después de corregir la migración, ejecutar:

```bash
dotnet build Tfs.Portfolio.slnx --configuration Release
```

Debe finalizar con **0 errores**.

### 6. Modelo snapshot
El archivo `ApplicationDbContextModelSnapshot.cs` se genera automáticamente con `dotnet ef migrations add`. No editar manualmente. Si falta, regenerar la migración inicial.

### 7. Prohibido
- Crear migraciones con nombre `XXXXX` o nombres de placeholder
- Tener múltiples archivos de migración que creen las mismas tablas
- Dejar el snapshot desactualizado respecto al modelo de entidades
- Subir migraciones con errores de compilación al repositorio remoto
