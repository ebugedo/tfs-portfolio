# Design

## Context

`Tfs.Portfolio.Api` ya referencia `Scalar.AspNetCore` v1.2.0 (`Tfs.Portfolio.Api.csproj:19`). En `Program.cs:132-140`, `MapOpenApi()` y `MapScalarApiReference()` están envueltos en `if (app.Environment.IsDevelopment())`, por lo que la UI de Scalar solo está disponible en desarrollo. La propuesta requiere que Scalar esté accesible también en producción.

## Goals / Non-Goals

**Goals:**
- Hacer disponible la UI de Scalar en todos los entornos, incluida producción, sin nuevas dependencias.
- Cambio de código mínimo y reversible.

**Non-Goals:**
- Agregar autenticación o autorización sobre el endpoint de Scalar (out of scope; se menciona como riesgo).
- Cambiar la versión del paquete NuGet.

## Decisions

**Quitar el condicional `IsDevelopment()`** alrededor de `MapOpenApi()` y `MapScalarApiReference()` en `Program.cs`, dejando ambos llamados fuera de cualquier filtro de entorno.
- Alternativa considerada: envolver en `if (app.Environment.IsDevelopment() || app.Environment.IsProduction())`. Descartada porque añade complejidad sin beneficio; la documentación debe estar disponible en staging y otros entornos intermedios también.
- Alternativa considerada: mantener solo en desarrollo y crear un cambio separado para producción. Descartada porque el requisito del usuario es explícito: producción incluida.

**Opciones de Scalar:** mantener `Title = "Tfs.Portfolio API"` y `Theme = ScalarTheme.DeepSpace` sin cambios.

## Risks / Trade-offs

- [Exposición de documentación de endpoints en producción] → Aumenta la superficie de información disponible a atacantes; mitigación opcional posterior: restringir acceso por IP o autenticación (no en este cambio).
- [Sin diferencia de comportamiento por entorno] → La UI estará disponible también en Testing/Staging; aceptable dado el requisito.

## Migration Plan

- Desplegar el cambio de código en `Program.cs` (quitar el bloque `if`).
- Verificar acceso a `/scalar/v1` en el entorno de destino.
- Rollback: restaurar el condicional `IsDevelopment()` si se detecta exposición no deseada.

## Open Questions

Ninguna.
