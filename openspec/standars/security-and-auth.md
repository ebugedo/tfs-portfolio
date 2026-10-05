# Security & Authorization Standards

## Authentication & Authorization
- **Protocol:** JWT (JSON Web Tokens) or OAuth2 / OpenID Connect.
- **Policy Enforcement:** Endpoints requiring auth MUST be decorated with `[Authorize]` or endpoint authorization policies.

## Secrets & Configuration
- **Zero Secrets in Source Control:** Connection strings, API keys, and JWT keys MUST NOT be committed to git.
- **Development:** Use `.NET User Secrets` (`dotnet user-secrets`) or local environment variables.
- **Production:** Use environment variables or secure key vaults (e.g., Azure Key Vault / HashiCorp Vault).