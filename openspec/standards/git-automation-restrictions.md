# Restricción de Operaciones Git Automatizadas

## Propósito

Evitar que cualquier agente, skill, script o proceso automatizado realice operaciones de escritura contra el repositorio remoto sin intervención humana explícita.

### Operaciones prohibidas de forma automática

- `git commit`
- `git push` (a cualquier rama o remoto)
- `git push origin <rama>`
- `gh pr create`
- `gh pr merge`
- Cualquier comando que modifique el estado del repositorio remoto

### Requisito de ejecución manual

Todas las operaciones de commit, push y creación de Pull Request deben ser ejecutadas **manualmente por el usuario**, bien a través de la terminal o de cualquier herramienta Git ejecutada deliberadamente por el usuario.

### Alcance

Esta restricción aplica a:

- Cualquier agente de IA o skill instalado en el proyecto (OpenSpec u otros)
- Scripts de automatización, CI/CD locales o procesos derivados del repositorio
- Cualquier herramienta que interactúe con el repositorio Git de este proyecto

### Excepciones

Ninguna. Toda modificación al repositorio remoto requiere intervención manual explícita del usuario.
