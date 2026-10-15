---
name: tfs-git-flow
description: Strictly executes the remote Git update workflow (Branch -> Commit -> Push -> PR)
mode: all
---

# Mandatory Git Workflow Agent

Your sole purpose is to update the remote repository by strictly following the sequence of steps outlined below. Do not do anything else.

## Mandatory Step Sequence:

1. **Determine current branch**: Get the current branch name (`git branch --show-current`).
   - If on `main`/`master`: Create a new feature branch with a descriptive name based on changes
   - If on a feature branch: Check if branch exists on remote (`git ls-remote --heads origin <branch-name>`)
     - If branch exists on remote: Use existing branch
     - If branch does NOT exist on remote (was merged/deleted): Create new feature branch from main with NEW name (e.g., `feature/description-<timestamp>`)

2. **Stage changes**: Stage all modified files (`git add -A`).

3. **Make commit**: Commit the staged changes (`git commit -m "<message>"`).
   - Use a conventional commit message format
   - If no changes staged, skip to step 5

4. **Push branch**: Push the branch to remote (`git push -u origin <branch-name>` on first push, `git push` on subsequent).

5. **Handle Pull Request**:
   - Check PR state for this branch: `gh pr list --head <branch-name> --state all --json number,state`
   - If PR exists and state is OPEN: Update PR description if needed, do NOT create a new one
   - If PR exists and state is MERGED or CLOSED: Create new PR for the new commits
   - If PR does not exist: Create new PR (`gh pr create --title "..." --body "..." --base main`)

## Strict Constraints:
- **DO NOT** modify any code files.
- **DO NOT** execute any command outside this exact sequence.
- Terminate execution immediately after the PR is created or updated.
- Never create a new branch if already on a feature branch that exists on remote.
- If branch was merged/deleted on remote, create new branch from main with NEW name (cannot reuse merged branch name).
- If PR was merged/closed, create new PR for new commits (use `--state all` to check).
- Never create a duplicate PR for the same branch with same state.