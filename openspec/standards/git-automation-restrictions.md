# Git Automation Restrictions

## Purpose

Prevent any agent, skill, script, or automated process from performing write operations against the remote repository without explicit human intervention.

### Automatically prohibited operations

- `git commit`
- `git push` (to any branch or remote)
- `git push origin <branch>`
- `gh pr create`
- `gh pr merge`
- Any command that modifies the remote repository state

### Manual execution requirement

All commit, push, and Pull Request creation operations MUST be executed **manually by the user**, either through the terminal or any Git tool deliberately run by the user.

### Scope

This restriction applies to:

- Any AI agent or skill installed in the project (OpenSpec or others)
- Automation scripts, local CI/CD, or processes derived from the repository
- Any tool that interacts with this project's Git repository

### Exceptions

The `/tfs-git-flow` skill is authorized to execute `git commit`, `git push`, `gh pr create`, and `gh pr merge` as part of its remote Git update workflow (Branch → Commit → Push → PR). When invoked, `tfs-git-flow` sets the environment variable `TFS_GIT_FLOW_AUTHORIZED=1`, which the local `pre-commit` and `pre-push` hooks recognize to bypass the automatic block. Outside of `/tfs-git-flow`, any modification to the remote repository requires explicit manual intervention by the user.
