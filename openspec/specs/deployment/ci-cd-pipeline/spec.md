# ci-cd-pipeline Specification

## Purpose
Defines the GitHub Actions CI/CD pipeline that builds, tests, and deploys the API to a VPS via Docker containers and GHCR.

## Requirements

### Requirement: CI runs on push and pull requests to main
The system SHALL execute the CI pipeline on every push to `main` branch and every pull request targeting `main`.

#### Scenario: CI triggers on push to main
- **WHEN** a commit is pushed to `main` branch
- **THEN** the CI workflow runs automatically

#### Scenario: CI triggers on pull request to main
- **WHEN** a pull request is opened targeting `main`
- **THEN** the CI workflow runs automatically

### Requirement: CI runs unit tests
The system SHALL execute all unit tests (`test/UnitTests/Tfs.Portfolio.UnitTests.csproj`) in the CI pipeline and fail if any test fails.

#### Scenario: Unit tests pass
- **WHEN** CI runs `dotnet test` on unit test project
- **THEN** all unit tests pass and pipeline continues

#### Scenario: Unit tests fail
- **WHEN** CI runs `dotnet test` on unit test project and a test fails
- **THEN** pipeline fails and stops

### Requirement: CI runs integration tests with PostgreSQL
The system SHALL execute all integration tests (`test/IntegrationTests/Tfs.Portfolio.IntegrationTests.csproj`) against a real PostgreSQL instance in the CI pipeline and fail if any test fails.

#### Scenario: Integration tests pass
- **WHEN** CI runs `dotnet test` on integration test project with PostgreSQL service container
- **THEN** all integration tests pass and pipeline continues

#### Scenario: Integration tests fail
- **WHEN** CI runs `dotnet test` on integration test project and a test fails
- **THEN** pipeline fails and stops

### Requirement: CI builds and pushes Docker image to GHCR
The system SHALL build a multi-stage Docker image from the repository Dockerfile and push it to GitHub Container Registry with two tags: commit SHA and `latest`.

#### Scenario: Image pushed with SHA tag
- **WHEN** CI builds and pushes Docker image
- **THEN** image appears in GHCR as `ghcr.io/<owner>/<repo>:sha-<commit-sha>`

#### Scenario: Image pushed with latest tag
- **WHEN** CI builds and pushes Docker image
- **THEN** image appears in GHCR as `ghcr.io/<owner>/<repo>:latest`

#### Scenario: GHCR authentication succeeds
- **WHEN** CI authenticates to GHCR using `GITHUB_TOKEN`
- **THEN** push succeeds without authentication errors

### Requirement: CD deploys to VPS via SSH on successful CI
The system SHALL execute the CD pipeline only after CI passes, and only on pushes to `main` (not pull requests).

#### Scenario: CD runs after successful CI on main
- **WHEN** CI completes successfully on `main` branch
- **THEN** CD workflow starts automatically

#### Scenario: CD does not run on pull request
- **WHEN** CI completes successfully on a pull request
- **THEN** CD workflow does not run

### Requirement: CD authenticates to GHCR on VPS
The system SHALL authenticate to GHCR on the VPS using a PAT stored in GitHub Secrets before pulling the image.

#### Scenario: VPS GHCR authentication succeeds
- **WHEN** CD runs `docker login ghcr.io` with PAT from secrets
- **THEN** authentication succeeds and pull proceeds

### Requirement: CD pulls latest image and replaces container
The system SHALL pull the `latest` image, stop and remove the existing `portfolio-api` container, and start a new container with the updated image.

#### Scenario: Container replaced with new image
- **WHEN** CD runs `docker pull`, `docker stop`, `docker rm`, `docker run`
- **THEN** new `portfolio-api` container runs with `latest` image

#### Scenario: Old container stopped and removed
- **WHEN** CD runs stop and remove commands
- **THEN** no container named `portfolio-api` exists from previous deployment

### Requirement: CD prunes unused Docker images
The system SHALL run `docker image prune -f` after deployment to clean up old image layers.

#### Scenario: Unused images removed
- **WHEN** CD runs `docker image prune -f`
- **THEN** dangling and unused images are removed from VPS
