# Tfs.Portfolio — API Construida con OpenCode, SDD (OpenSpec), Modelos Zen Gratuitos — API Profesional en Producción

## 1. Introducción y Alcance del Sistema

### ¿Qué es Tfs.Portfolio?

Tfs.Portfolio es una API de Gestión de Portafolio construida con .NET 10, ASP.NET Core Web API, Entity Framework Core 9 + Npgsql (PostgreSQL), Autofac (DI), FluentValidation, Serilog, Scalar/OpenAPI, xUnit y Testcontainers. Se despliega en un VPS con Docker y Nginx, con CI/CD a través de GitHub Actions a GHCR.

**Repositorio de código fuente:** https://github.com/ebugedo/tfs-portfolio  
**Documentación de API de producción:** https://api-portfolio.timeforsoftware.com/scalar/v1

### Dominio

El sistema modela un dominio de gestión de portafolio con los siguientes conceptos principales:

- **Clientes** — Individuos u organizaciones que poseen proyectos. Cada cliente tiene un nombre, email y estado activo.
- **Sectores** — Sectores industriales o de negocio utilizados para categorizar proyectos (por ejemplo, Tecnología, Salud, Finanzas).
- **Servicios** — Ofertas que pueden asociarse a proyectos. Cada servicio tiene un nombre, categoría y estado activo.
- **Proyectos** — Elementos de portafolio que vinculan un cliente, un sector y un conjunto de servicios. Los proyectos tienen fechas de inicio, duraciones, tecnologías y estado.
- **Perfil de Empresa** — Un registro único que describe la organización que ejecuta el portafolio (nombre, descripción, detalles de contacto).

### Objetivos

- Proporcionar una API REST para gestionar datos del portafolio (clientes, sectores, servicios, proyectos, perfil de empresa).
- Proteger operaciones de escritura (POST, PUT, DELETE, PATCH) con autenticación mediante API Key, manteniendo las operaciones de lectura (GET) públicamente accesibles.
- Servir documentación interactiva de la API vía Scalar/OpenAPI en `/scalar/v1` (desarrollo) y https://api-portfolio.timeforsoftware.com/scalar/v1 (producción).
- Soportar despliegue en un VPS con Docker y Nginx, con CI/CD vía GitHub Actions.

---

## 2. Pila Tecnológica

| Capa | Tecnología | Versión | Propósito |
|------|------------|---------|-----------|
| Tiempo de ejecución | .NET | 10 | Tiempo de ejecución y SDK de la aplicación |
| Framework Web | ASP.NET Core Web API | .NET 10 | API REST, controladores, middleware |
| Contenedor DI | Autofac | Última | Inyección de dependencias |
| ORM | Entity Framework Core | 9 | Acceso a datos, migraciones, proveedor Npgsql |
| Base de datos | PostgreSQL | 16 | Almacenamiento persistente |
| Validación | FluentValidation | Última | Validación de entrada |
| Registro | Serilog | Última | Sumideros de Consola, Archivo y Seq |
| Documentación de API | Scalar / OpenAPI | Última | Documentación interactiva en /scalar/v1 y producción |
| Serialización | Newtonsoft.Json | Última | Serialización JSON con camelCase |
| Pruebas | xUnit | Última | Pruebas unitarias |
| Pruebas de Integración | Testcontainers | Última | Pruebas de integración con dependencias reales |
| Contenerización | Docker / Docker Compose | Última | Aislamiento de entornos y despliegue |
| Proxy Inverso | Nginx | Última | Terminación TLS y enrutamiento de solicitudes |
| CI/CD | GitHub Actions | — | Canalización de construcción, prueba y despliegue |
| Registro de Contenedores | GHCR | — | Almacena y distribuye imágenes Docker |

---

## 3. Metodología y Herramientas de Desarrollo

### OpenCode con OpenSpec

Este proyecto fue planificado y desarrollado utilizando **OpenCode con OpenSpec** (flujo especificado: proposal → specs → design → tasks → apply → archive) y **modelos Zen gratuitos de OpenCode** para asistencia en el desarrollo y la planificación. Esta información no está presente en ningún archivo de código fuente del proyecto y se documenta aquí explícitamente.

OpenSpec proporciona un flujo de trabajo de planificación estructurado:

- **proposal.md** — Qué y por qué del cambio
- **specs/<capacidad>/spec.md** — Especificaciones delta (requisitos que el sistema debe cumplir)
- **design.md** — Cómo implementar el cambio
- **tasks.md** — Pasos de implementación con casillas de seguimiento

El flujo de trabajo garantiza una separación clara entre planificación e implementación, con cada fase vinculada a la anterior.

### Desarrollo Basado en Especificaciones (SDD)

Los requisitos se definen como especificaciones antes de la implementación. Cada requisito incluye escenarios que definen el comportamiento esperado, lo que hace que la especificación sea ejecutable y verificable.

### Arquitectura Limpia (Clean Architecture)

El sistema sigue Arquitectura Limpia con separación de preocupaciones entre capas:

- **Capa de Presentación** — ASP.NET Core Web API, controladores, middleware, configuración de OpenAPI/Scalar
- **Capa de Aplicación** — Comandos/consultas CQRS, manejadores, validadores, DTOs (usando MediatR)
- **Capa de Infraestructura** — Persistencia Entity Framework Core, repositorios, migraciones
- **Capa de Dominio** — Lógica de negocio central, entidades, objetos de valor, eventos de dominio

Las capas se comunican mediante inyección de dependencias (Autofac). El proyecto API referencia solo a Aplicación y Persistencia; el Dominio se accede indirectamente a través de Aplicación.

### Diseño Dirigido por el Dominio (DDD)

El dominio es el centro del diseño:

- **Entidades** — Cliente, Sector, Servicio, Proyecto, Perfil de Empresa
- **Objetos de Valor** — Objetos de valor del dominio (por ejemplo, Email, Dinero)
- **Eventos de Dominio** — Tipos de eventos y manejadores para eventos de negocio
- **Agregados** — Cada entidad es una raíz de agregado con límites claros

El lenguaje ubicuo se comparte entre el dominio del negocio y el modelo de código.

### Documentación de Arquitectura C4

La arquitectura del sistema está documentada usando el modelo C4 con diagramas PlantUML ubicados bajo `docs/architecture/diagrams/`:

- **c4-context.puml** — Diagrama de Contexto del Sistema que muestra usuarios externos y dependencias
- **c4-containers.puml** — Diagrama de Contenedores que muestra el contenedor de API, PostgreSQL, Nginx, Docker, GHCR
- **c4-components.puml** — Diagrama de Componentes que muestra las capas de Dominio, Aplicación, Infraestructura y Presentación
- **c4-code.puml** — Diagrama de Código que muestra las clases e interfaces clave

---

## 4. Endpoints de la API

Todos los endpoints están bajo `/api/v1/`. Las respuestas JSON usan nombres de propiedades en camelCase. Las operaciones de escritura requieren una clave API válida en el encabezado `X-API-Key`; las operaciones de lectura (GET) son públicas.

**API de Producción:** https://api-portfolio.timeforsoftware.com/scalar/v1  
**API de Desarrollo:** http://localhost:8082/scalar/v1

### Clientes

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/clients` | Listar todos los clientes | Público |
| GET | `/api/v1/clients/{id}` | Obtener cliente por ID | Público |
| POST | `/api/v1/clients` | Crear un cliente | API Key |
| PUT | `/api/v1/clients/{id}` | Actualizar un cliente | API Key |
| DELETE | `/api/v1/clients/{id}` | Eliminar un cliente | API Key |

### Sectores

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/sectors` | Listar todos los sectores | Público |
| GET | `/api/v1/sectors/{id}` | Obtener sector por ID | Público |
| POST | `/api/v1/sectors` | Crear un sector | API Key |
| PUT | `/api/v1/sectors/{id}` | Actualizar un sector | API Key |
| DELETE | `/api/v1/sectors/{id}` | Eliminar un sector | API Key |

### Servicios

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/services` | Listar todos los servicios | Público |
| GET | `/api/v1/services/{id}` | Obtener servicio por ID | Público |
| GET | `/api/v1/services/category/{category}` | Filtrar servicios por categoría | Público |
| POST | `/api/v1/services` | Crear un servicio | API Key |
| PUT | `/api/v1/services/{id}` | Actualizar un servicio | API Key |
| DELETE | `/api/v1/services/{id}` | Eliminar un servicio | API Key |

### Perfil de Empresa (singleton)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/company-profile` | Obtener perfil de empresa | Público |
| PUT | `/api/v1/company-profile` | Actualizar perfil de empresa | API Key |

### Proyectos

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/projects` | Listar proyectos con filtros opcionales | Público |
| POST | `/api/v1/projects` | Crear un proyecto | API Key |
| PUT | `/api/v1/projects/{id}` | Actualizar un proyecto | API Key |
| PATCH | `/api/v1/projects/{id}/status` | Cambiar estado del proyecto | API Key |
| POST | `/api/v1/projects/{id}/services/{serviceId}` | Asociar un servicio | API Key |
| DELETE | `/api/v1/projects/{id}/services/{serviceId}` | Eliminar asociación de servicio | API Key |

### Respuestas de Error

Todos los errores usan el formato RFC 7807 ProblemDetails:

```json
{
  "type": "https://api.tfs.portfolio/problems/validation-error",
  "title": "Uno o más errores de validación ocurrieron",
  "status": 400,
  "detail": "El campo Nombre es requerido.",
  "instance": "/api/v1/clients"
}
```

| Estado | Significado |
|--------|-------------|
| 400 | Solicitud incorrecta — error de validación o entrada inválida |
| 401 | No autorizado — falta o es inválida la clave API |
| 404 | No encontrado — el recurso no existe |
| 500 | Error interno del servidor — falla inesperada |

### Comprobaciones de Salud

| Endpoint | Descripción |
|----------|-------------|
| GET `/health/live` | Prueba de vitalidad |
| GET `/health/ready` | Prueba de preparación (incluye verificación de base de datos) |

### Documentación Interactiva

La UI de Scalar/OpenAPI está disponible en `/scalar/v1` en el host de la API (por ejemplo, `http://localhost:8082/scalar/v1` para desarrollo, `https://api-portfolio.timeforsoftware.com/scalar/v1` para producción).

---

## 5. Despliegue e Infraestructura

### Entorno de Desarrollo

```bash
docker compose up -d
```

La API está disponible en `http://localhost:8082`. La documentación de Scalar en `http://localhost:8082/scalar/v1`.

### Despliegue en Producción

El sistema se despliega en un VPS con Docker y Nginx:

- **Nginx** — Terminación TLS y enrutamiento de solicitudes (puertos 443/80)
- **Contenedor de API** — ASP.NET Core en el puerto 8080 (solo localhost)
- **Contenedor de PostgreSQL** — Almacenamiento persistente en el puerto 5432 (red de Docker)
- **Docker** — Contenerización y aislamiento de entornos

### Canalización CI/CD (GitHub Actions)

- **CI** — Se ejecuta en push/PR a `main`: pruebas unitarias → pruebas de integración → construir imagen Docker → subir a GHCR
- **CD** — Se ejecuta en push a `main` (después de que CI pasa): SSH al VPS → desplegar mediante CLI de Docker

### Secretos Requeridos de GitHub

| Secreto | Descripción |
|---------|-------------|
| `VPS_HOST` | Dirección IP o dominio del VPS |
| `VPS_USERNAME` | Usuario SSH para el despliegue |
| `VPS_SSH_KEY` | Clave privada SSH (formato OpenSSH) |
| `VPS_PORT` | Puerto SSH (predeterminado: 22) |
| `GHCR_PAT` | PAT con lectura/escritura/eliminación de paquetes |
| `POSTGRES_PASSWORD` | Contraseña de superusuario de PostgreSQL |
| `POSTGRES_DB` | Nombre de la base de datos |
| `POSTGRES_USER` | Usuario de PostgreSQL |
| `API_PORT` | Puerto del host para la API |

---

## 6. Virtudes del Desarrollo — Grupo 1: OpenCode + OpenSpec + Modelos Zen Gratuitos

### Planificación Estructurada con OpenSpec

OpenSpec proporciona un flujo de trabajo de planificación estructurado (proposal → specs → design → tasks → apply → archive) que garantiza:

- Separación clara entre planificación e implementación
- Cada fase está vinculada a la anterior
- Los requisitos se documentan antes de escribir código
- Trazabilidad desde la especificación hasta las tareas

### OpenCode con Modelos Zen Gratuitos

El uso de modelos Zen gratuitos de OpenCode proporciona:

- **Acceso sin costo** — Sin tarifas de licencia para asistencia de IA
- **Generación de código y documentación** — Generación rápida de código repetitivo y documentación
- **Automatización de tareas** — Asistencia con tareas repetitivas de desarrollo
- **Barrera de entrada reducida** — Hace el desarrollo asistido por IA accesible sin suscripciones de pago

### Flujo de Trabajo Guiado por Fases

El flujo de trabajo guiado (proponer → aplicar → archivar) garantiza:

- Los artifacts de planificación están completos antes de la implementación
- La implementación es rastreable a las especificaciones
- Los cambios se archivan con todo el contexto después de su finalización

---

## 7. Virtudes del Desarrollo — Grupo 2: SDD, Clean Architecture y DDD

### Desarrollo Basado en Especificaciones (SDD)

SDD trata los requisitos como especificaciones ejecutables:

- Los requisitos se definen antes de la implementación
- Los escenarios definen el comportamiento esperado para cada requisito
- La implementación es verificable contra la especificación
- Reduce la ambigüedad entre las partes interesadas y los desarrolladores

### Arquitectura Limpia (Clean Architecture)

Arquitectura Limpia proporciona:

- **Separación de capas** — Dominio, Aplicación, Infraestructura y Presentación son independientes
- **Independencia de frameworks** — La lógica de negocio no está acoplada a ASP.NET Core o EF Core
- **Probabilidad** — Cada capa puede probarse de forma aislada
- **Mantenibilidad** — Los cambios en una capa no se propagan a otras

### Diseño Dirigido por el Dominio (DDD)

DDD centra el diseño en el dominio del negocio:

- **Dominio como centro** — Entidades, objetos de valor y eventos de dominio impulsan el diseño
- **Lenguaje ubicuo** — Vocabulario compartido entre el negocio y el código
- **Reducción del acoplamiento** — La capa de Dominio no tiene dependencias externas
- **Evolución controlada** — La arquitectura evoluciona con el dominio, no al revés

---

## 8. Virtudes del Desarrollo — Grupo 3: Tecnologías Detalladas

### .NET 10

- Alto rendimiento y productividad
- Ecosistema maduro con soporte extensivo de bibliotecas
- Soporte de largo plazo (LTS)
- Multiplataforma (Windows, Linux, macOS)

### ASP.NET Core

- Framework web rápido y ligero
- Canalización de middleware rica
- Soporte tanto para controladores como para Minimal APIs
- Inyección de dependencias integrada

### Entity Framework Core 9

- Mapeo objeto-relacional (ORM)
- Migraciones de base de datos con enfoque code-first
- Consultas basadas en LINQ
- Proveedor Npgsql para PostgreSQL

### Docker

- Entornos consistentes entre desarrollo y producción
- Aislamiento de aplicaciones
- Despliegue y escalado simplificados

### Scalar/OpenAPI

- Documentación de API interactiva generada automáticamente
- Funcionalidad de prueba para endpoints
- Disponible en `/scalar/v1` en todos los entornos y en la URL de producción

### xUnit + Testcontainers

- Pruebas unitarias con el framework xUnit
- Pruebas de integración con dependencias reales (PostgreSQL) mediante Testcontainers
- Pruebas confiables sin simular infraestructura

---

## 9. Problemas y Desafíos — Grupo 1: OpenCode + OpenSpec + Modelos Zen Gratuitos

### Disciplina de OpenSpec

OpenSpec requiere disciplina para mantener los artifacts sincronizados con el código. Existe el riesgo de desincronización entre la planificación y la implementación si OpenSpec no se usa de manera consistente a lo largo del ciclo de vida del desarrollo.

### Limitaciones de los Modelos Zen Gratuitos

Los modelos Zen gratuitos de OpenCode tienen limitaciones en comparación con los modelos de pago:

- Límites de velocidad en las solicitudes
- Ventana de contexto reducida
- Calidad variable del resultado generado
- Dependencia de conectividad a internet para el acceso a los modelos

---

## 10. Problemas y Desafíos — Grupo 2: SDD, Clean Architecture y DDD

### Posible Sobreingeniería

Para proyectos pequeños, la arquitectura de múltiples capas (Dominio → Aplicación → Infraestructura → Presentación) puede ser sobreingeniería, añadiendo complejidad sin beneficio inmediato.

### Curva de Aprendizaje

DDD y Arquitectura Limpia tienen una curva de aprendizaje más pronunciada. Los desarrolladores necesitan entender conceptos como agregados, objetos de valor, eventos de dominio y CQRS antes de ser productivos.

### Código Repetitivo

Más capas significan más código repetitivo y abstracciones intermedias. El tiempo de configuración inicial es mayor en comparación con arquitecturas más simples.

---

## 11. Problemas y Desafíos — Grupo 3: Tecnologías Detalladas

### Configuración de Autofac

La configuración de inyección de dependencias de Autofac en `Program.cs` puede volverse verbosa a medida que la aplicación crece, requiriendo una organización cuidadosa de los módulos.

### Gestión de Secretos

La gestión de secretos entre entornos es desafiante:

- Desarrollo: `dotnet user-secrets` o `appsettings.Development.json`
- Producción: Variables de entorno inyectadas mediante CI/CD y Docker

### Docker + Nginx + PostgreSQL en VPS

El despliegue en producción requiere configuración manual de:

- Red de Docker compartida entre Nginx y API
- Volúmenes persistentes para datos de PostgreSQL
- Configuración de terminación TLS y proxy de Nginx

### Requisitos de Testcontainers

Las pruebas de integración con Testcontainers requieren que Docker esté disponible en el entorno de pruebas, lo cual puede no estar disponible en todos los contextos de CI.

### Renderizado de PlantUML

Los diagramas PlantUML requieren un renderizador externo (servidor de PlantUML, complemento o herramienta en línea) para visualizarse; no se renderizan de forma nativa en todos los visores de Markdown.

---

## 12. Conclusiones y Recomendaciones

### Balance entre Arquitectura y Complejidad

Arquitectura Limpia y DDD proporcionan mantenibilidad y probabilidad a largo plazo, pero añaden complejidad inicial. Para este proyecto, la arquitectura en capas está justificada por la necesidad de una lógica de gestión de portafolio mantenible y probabile.

### Mantener la Documentación de OpenSpec Sincronizada

Para mantener los artifacts de OpenSpec sincronizados con el código:

- Actualice `proposal.md`, `specs/`, `design.md` y `tasks.md` con cada PR de característica
- Incluya cambios de documentación en la definición de listo
- Revise los artifacts de planificación durante la revisión de código

### Pasos Sugeridos

1. Integre las actualizaciones de documentación en la canalización CI/CD
2. Considere la validación automatizada de los artifacts de OpenSpec contra la base de código
3. Agregue registros de decisiones de arquitectura (ADRs) para decisiones técnicas significativas

---

## 13. Apéndice: Glosario de Términos

| Término | Definición |
|---------|------------|
| API Key | Token de autenticación pasado en el encabezado `X-API-Key` para operaciones de escritura |
| CQRS | Command Query Responsibility Segregation — separa operaciones de lectura y escritura |
| DDD | Domain-Driven Design — diseño de software centrado en el dominio del negocio |
| GHCR | GitHub Container Registry — almacena y distribuye imágenes Docker |
| OpenCode | Asistente de codificación asistido por IA (OpenCode) |
| OpenSpec | Open Specification — flujo de trabajo de planificación estructurado (proposal, specs, design, tasks) |
| ProblemDetails | Formato estándar RFC 7807 para respuestas de error HTTP |
| SDD | Spec-Driven Development — requisitos definidos como especificaciones ejecutables |
| Scalar | UI de documentación interactiva para OpenAPI |
| Testcontainers | Biblioteca para pruebas de integración con instancias de contenedor desechables |
| VPS | Virtual Private Server — servidor virtual privado para despliegue en producción |
| Modelos Zen | Modelos de IA gratuitos proporcionados por OpenCode |

---

*Generado desde `docs/Tfs.Portfolio-Documentation.es.md`*
