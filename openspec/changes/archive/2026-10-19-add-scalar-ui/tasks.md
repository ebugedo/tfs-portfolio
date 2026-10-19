# Tasks

## 1. Implementación de Scalar en producción

- [x] 1.1 Quitar el condicional `IsDevelopment()` alrededor de `MapOpenApi()` y `MapScalarApiReference()` en `src/Presentation/WebAPI/Program.cs` para que la UI de Scalar esté disponible en todos los entornos, incluida producción. Verificar que el bloque queda fuera de cualquier filtro de entorno.
- [x] 1.2 Compilar el proyecto `Tfs.Portfolio.Api` con `dotnet build src/Presentation/WebAPI/Tfs.Portfolio.Api.csproj` y verificar que la build termina con cero errores.
- [x] 1.3 Verificar que `Scalar.AspNetCore` sigue referenciado en `Tfs.Portfolio.Api.csproj` (versión 1.2.0) y que `using Scalar.AspNetCore` está presente en `Program.cs`.

## 2. Verificación

- [x] 2.1 Confirmar que `app.MapOpenApi()` y `app.MapScalarApiReference()` ya no están protegidos por `if (app.Environment.IsDevelopment())` y que Scalar expone la documentación en `/scalar/v1` en cualquier entorno.
