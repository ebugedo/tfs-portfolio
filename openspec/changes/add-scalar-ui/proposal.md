# Proposal

## Why

La interfaz de Scalar está configurada únicamente para el entorno de desarrollo (`Program.cs` guarda `MapScalarApiReference` dentro de `if (app.Environment.IsDevelopment())`). Equipos de producto, soporte y consumidores de la API no pueden explorar la documentación interactiva en entornos de pre-producción o producción sin desplegar una instancia de desarrollo. Se expone Scalar en producción para permitir la consulta y prueba de endpoints documentados directamente desde el entorno real.

## What Changes

- Quitar el condicional `IsDevelopment()` alrededor de `MapOpenApi()` y `MapScalarApiReference()` en `Program.cs` para que la documentación interactiva esté disponible en todos los entornos (incluido producción).
- Mantener `Scalar.AspNetCore` ya referenciado en `Tfs.Portfolio.Api.csproj` (v1.2.0).
- Ajustar opciones de Scalar (título y tema) si es necesario para uso en producción.
- Actualizar la especificación `openspec/specs/api-layer/spec.md` si el requisito de disponibilidad de documentación cambia de "solo desarrollo" a "todos los entornos".

## Capabilities

### Modified Capabilities
- `api-layer`: el requisito "Scalar/OpenAPI provides API documentation" pasa de estar disponible solo en desarrollo a estar disponible en todos los entornos (incluido producción).

## Impact

- Código: `src/Presentation/WebAPI/Program.cs` (middleware pipeline, bloque de desarrollo).
- Especificación: `openspec/specs/api-layer/spec.md` (requisito de documentación Scalar).
- Dependencias: ninguna nueva; `Scalar.AspNetCore` ya está referenciado.
- Seguridad: exponer la UI de Swagger/Scalar en producción amplia la superficie de información de endpoints; considerar restringir acceso si la API no es pública.
