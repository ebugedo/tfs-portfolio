// <copyright file="ProjectsApiTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Api;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Application.Sectors.Dtos;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.IntegrationTests;
using Xunit;
using FluentAssertions;

/// <summary>
    /// Integration tests for the Projects API endpoints.
    /// </summary>
    public sealed class ProjectsApiTests : IntegrationTestBase
    {
        private const string BaseUrl = "/api/v1/projects";

        /// <summary>
        /// Tests that GET /api/v1/projects returns 200 with empty array initially.
        /// </summary>
        [Fact]
        public async Task GetProjects_WhenEmpty_ReturnsEmptyArray()
        {
            // Act
            var response = await Client.GetAsync(new Uri(BaseUrl, UriKind.Relative));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<ProjectListItemDto>>();
            projects.Should().NotBeNull();
            projects.Should().BeEmpty();
        }

        /// <summary>
        /// Tests that POST /api/v1/projects with valid body returns 201 with Location header and ProjectDto.
        /// </summary>
        [Fact]
        public async Task CreateProject_WithValidBody_ReturnsCreatedWithLocationAndDto()
        {
            // Arrange - create client and sector first
            var clientId = await CreateTestClientAsync();
            var sectorId = await CreateTestSectorAsync();

            var request = new CreateProjectCommand(
                "Test Project",
                "Test Description",
                YearMonth.Create(6, 2024),
                6,
                clientId,
                sectorId,
                new List<string> { "C#", ".NET" });

            // Act
            var response = await Client.PostAsJsonAsync(BaseUrl, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.ToString().Should().StartWith($"{BaseUrl}/");

            var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
            project.Should().NotBeNull();
            project!.Name.Should().Be("Test Project");
            project.Description.Should().Be("Test Description");
            project.Id.Should().NotBeEmpty();
            project.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }

    /// <summary>
    /// Tests that GET /api/v1/projects/{id} returns 200 with ProjectDto.
    /// </summary>
    [Fact]
    public async Task GetProjectById_WhenExists_ReturnsProjectDto()
    {
        // Arrange - create a project first
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var createRequest = new CreateProjectCommand(
            "Get By ID Test",
            "Description",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId,
            new List<string> { "C#", ".NET" });
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        // Act
        var response = await Client.GetAsync(
            new Uri($"{BaseUrl}/{createdProject!.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        project.Should().NotBeNull();
        project!.Id.Should().Be(createdProject.Id);
        project.Name.Should().Be("Get By ID Test");
    }

    /// <summary>
    /// Tests that PUT /api/v1/projects/{id} returns 204.
    /// </summary>
    [Fact]
    public async Task UpdateProject_WhenExists_ReturnsNoContent()
    {
        // Arrange - create a project first
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var createRequest = new CreateProjectCommand(
            "Update Test",
            "Original Description",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId,
            new List<string> { "C#", ".NET" });
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        // Act - update the project
        var updateRequest = new UpdateProjectCommand(
            createdProject!.Id,
            "Updated Name",
            "Updated Description",
            YearMonth.Create(7, 2024),
            8,
            clientId,
            sectorId,
            new List<string> { "C#", ".NET" },
            ProjectStatus.Draft);
        var response = await Client.PutAsJsonAsync(
            new Uri($"{BaseUrl}/{createdProject.Id}", UriKind.Relative),
            updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await Client.GetAsync(
            new Uri($"{BaseUrl}/{createdProject.Id}", UriKind.Relative));
        var updatedProject = await getResponse.Content.ReadFromJsonAsync<ProjectDto>();
        updatedProject!.Name.Should().Be("Updated Name");
        updatedProject.Description.Should().Be("Updated Description");
    }

    /// <summary>
    /// Tests that DELETE /api/v1/projects/{id} returns 204.
    /// </summary>
    [Fact]
    public async Task DeleteProject_WhenExists_ReturnsNoContent()
    {
        // Arrange - create a project first
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var createRequest = new CreateProjectCommand(
            "Delete Test",
            "To be deleted",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId,
            new List<string> { "C#" });
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        // Act
        var response = await Client.DeleteAsync(
            new Uri($"{BaseUrl}/{createdProject!.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await Client.GetAsync(
            new Uri($"{BaseUrl}/{createdProject.Id}", UriKind.Relative));
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that POST /api/v1/projects with invalid body returns 400 ProblemDetails.
    /// </summary>
    [Fact]
    public async Task CreateProject_WithInvalidBody_ReturnsBadRequestWithProblemDetails()
    {
        // Arrange - create client and sector first
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        // Arrange - empty name should fail validation
        var request = new CreateProjectCommand(
            string.Empty,
            "Test Description",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId,
            new List<string> { "C#" });

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>();
        problemDetails.GetProperty("status").GetInt32().Should().Be(400);
        problemDetails.GetProperty("title").GetString().Should().Be("Validation Failed");
        problemDetails.GetProperty("detail").GetString().Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that GET /api/v1/projects/{nonexistent} returns 404 ProblemDetails.
    /// </summary>
    [Fact]
    public async Task GetProjectById_WhenNotFound_ReturnsNotFoundWithProblemDetails()
    {
        // Act
        var response = await Client.GetAsync(
            new Uri($"{BaseUrl}/{Guid.NewGuid()}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>();
        problemDetails.GetProperty("status").GetInt32().Should().Be(404);
        problemDetails.GetProperty("title").GetString().Should().Be("Not Found");
    }

    /// <summary>
    /// Tests that PUT /api/v1/projects/{id} with mismatched ID returns 400.
    /// </summary>
    [Fact]
    public async Task UpdateProject_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange - create client, sector, and project first
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var createRequest = new CreateProjectCommand(
            "Mismatch Test",
            "Test",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId,
            new List<string> { "C#" });
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        // Act - update with different ID
        var updateRequest = new UpdateProjectCommand(
            Guid.NewGuid(),
            "Updated",
            "Test",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId,
            new List<string> { "C#" },
            ProjectStatus.Draft);
        var response = await Client.PutAsJsonAsync(
            $"{BaseUrl}/{createdProject!.Id}",
            updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Tests that GET /api/v1/projects returns all projects.
    /// </summary>
    [Fact]
    public async Task GetProjects_WhenMultipleExist_ReturnsAll()
    {
        // Arrange - create client and sector first
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        // Arrange - create multiple projects
        var project1 = await Client.PostAsJsonAsync(
            BaseUrl,
            new CreateProjectCommand(
                "Project 1",
                "Desc 1",
                YearMonth.Create(6, 2024),
                6,
                clientId,
                sectorId,
                new List<string> { "C#" }));
        var project2 = await Client.PostAsJsonAsync(
            BaseUrl,
            new CreateProjectCommand(
                "Project 2",
                "Desc 2",
                YearMonth.Create(7, 2024),
                8,
                clientId,
                sectorId,
                new List<string> { "Java" }));

        var p1 = await project1.Content.ReadFromJsonAsync<ProjectDto>();
        var p2 = await project2.Content.ReadFromJsonAsync<ProjectDto>();

        // Act
        var response = await Client.GetAsync(new Uri(BaseUrl, UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<ProjectListItemDto>>();
        projects.Should().NotBeNull();
        projects!.Should().HaveCount(2);
        projects.Should().Contain(p => p.Id == p1!.Id);
        projects.Should().Contain(p => p.Id == p2!.Id);
    }

    /// <summary>
    /// Tests that GET /api/v1/projects with clientId filter returns filtered projects.
    /// </summary>
    [Fact]
    public async Task GetProjects_WithClientIdFilter_ReturnsFilteredProjects()
    {
        // Arrange - create two clients
        var client1Id = await CreateTestClientAsync("Client 1", "client1@test.com");
        var client2Id = await CreateTestClientAsync("Client 2", "client2@test.com");
        var sectorId = await CreateTestSectorAsync("Technology");

        // Create projects for different clients
        await CreateTestProjectAsync("Project for Client 1", clientId: client1Id, sectorId: sectorId);
        await CreateTestProjectAsync("Project for Client 2", clientId: client2Id, sectorId: sectorId);
        await CreateTestProjectAsync("Another Project for Client 1", clientId: client1Id, sectorId: sectorId);

        // Act - filter by client1Id
        var response = await Client.GetAsync(new Uri($"{BaseUrl}?clientId={client1Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<ProjectListItemDto>>();
        projects.Should().NotBeNull();
        projects!.Should().HaveCount(2);
        projects.Should().AllSatisfy(p => p.ClientId.Should().Be(client1Id));
    }

    /// <summary>
    /// Tests that GET /api/v1/projects with sectorId filter returns filtered projects.
    /// </summary>
    [Fact]
    public async Task GetProjects_WithSectorIdFilter_ReturnsFilteredProjects()
    {
        // Arrange - create sectors and client
        var sector1Id = await CreateTestSectorAsync("Technology");
        var sector2Id = await CreateTestSectorAsync("Finance");
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");

        // Create projects for different sectors
        await CreateTestProjectAsync("Tech Project", sectorId: sector1Id, clientId: clientId);
        await CreateTestProjectAsync("Finance Project", sectorId: sector2Id, clientId: clientId);
        await CreateTestProjectAsync("Another Tech Project", sectorId: sector1Id, clientId: clientId);

        // Act - filter by sector1Id
        var response = await Client.GetAsync(new Uri($"{BaseUrl}?sectorId={sector1Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<ProjectListItemDto>>();
        projects.Should().NotBeNull();
        projects!.Should().HaveCount(2);
        projects.Should().AllSatisfy(p => p.SectorId.Should().Be(sector1Id));
    }

    /// <summary>
    /// Tests that GET /api/v1/projects with technology filter returns filtered projects.
    /// </summary>
    [Fact]
    public async Task GetProjects_WithTechnologyFilter_ReturnsFilteredProjects()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");

        await CreateTestProjectAsync("C# Project", technologies: new List<string> { "C#", ".NET" }, clientId: clientId, sectorId: sectorId);
        await CreateTestProjectAsync("Java Project", technologies: new List<string> { "Java", "Spring" }, clientId: clientId, sectorId: sectorId);
        await CreateTestProjectAsync("Another C# Project", technologies: new List<string> { "C#", "ASP.NET" }, clientId: clientId, sectorId: sectorId);

        // Act - filter by technology "C#"
        var response = await Client.GetAsync(new Uri($"{BaseUrl}?technology=C%23", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<ProjectListItemDto>>();
        projects.Should().NotBeNull();
        projects!.Should().HaveCount(2);
    }

    /// <summary>
    /// Tests that GET /api/v1/projects with status filter returns filtered projects.
    /// </summary>
    [Fact]
    public async Task GetProjects_WithStatusFilter_ReturnsFilteredProjects()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");

        var draftProjectId = await CreateTestProjectAsync("Draft Project", clientId: clientId, sectorId: sectorId);
        var activeProjectId = await CreateTestProjectAsync("Active Project", clientId: clientId, sectorId: sectorId);

        // Change one project to Active status
        var changeStatusRequest = new ChangeProjectStatusCommand(activeProjectId, ProjectStatus.Active);
        await Client.PatchAsJsonAsync(new Uri($"{BaseUrl}/{activeProjectId}/status", UriKind.Relative), changeStatusRequest);

        // Act - filter by Draft status
        var response = await Client.GetAsync(new Uri($"{BaseUrl}?status={ProjectStatus.Draft}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<ProjectListItemDto>>();
        projects.Should().NotBeNull();
        projects!.Should().HaveCount(1);
        projects.Should().Contain(p => p.Id == draftProjectId);
        projects.Should().NotContain(p => p.Id == activeProjectId);
    }

    /// <summary>
    /// Tests that GET /api/v1/projects with multiple filters returns projects matching all filters.
    /// </summary>
    [Fact]
    public async Task GetProjects_WithMultipleFilters_ReturnsProjectsMatchingAllFilters()
    {
        // Arrange
        var client1Id = await CreateTestClientAsync("Client 1", "client1@test.com");
        var client2Id = await CreateTestClientAsync("Client 2", "client2@test.com");
        var sectorId = await CreateTestSectorAsync("Technology");

        await CreateTestProjectAsync("Client1 Active C# Project", clientId: client1Id, sectorId: sectorId, technologies: new List<string> { "C#" });
        var client1DraftId = await CreateTestProjectAsync("Client1 Draft C# Project", clientId: client1Id, sectorId: sectorId, technologies: new List<string> { "C#" });
        await CreateTestProjectAsync("Client2 Active C# Project", clientId: client2Id, sectorId: sectorId, technologies: new List<string> { "C#" });

        var client1ActiveId = await CreateTestProjectAsync("Client1 Active Java Project", clientId: client1Id, sectorId: sectorId, technologies: new List<string> { "Java" });

        // Change Client1 Active projects to Active status
        var changeStatusRequest1 = new ChangeProjectStatusCommand(client1ActiveId, ProjectStatus.Active);
        await Client.PatchAsJsonAsync(new Uri($"{BaseUrl}/{client1ActiveId}/status", UriKind.Relative), changeStatusRequest1);

        // Act - filter by client1, C# technology, and Active status
        var response = await Client.GetAsync(new Uri($"{BaseUrl}?clientId={client1Id}&technology=C%23&status={ProjectStatus.Active}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var projects = await response.Content.ReadFromJsonAsync<IReadOnlyList<ProjectListItemDto>>();
        projects.Should().NotBeNull();
        projects!.Should().HaveCount(1);
        projects.Should().Contain(p => p.Id == client1ActiveId);
    }

    /// <summary>
    /// Tests that PATCH /api/v1/projects/{id}/status changes project status successfully.
    /// </summary>
    [Fact]
    public async Task ChangeProjectStatus_ValidTransition_ReturnsNoContent()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");
        var projectId = await CreateTestProjectAsync("Status Test Project", clientId: clientId, sectorId: sectorId);

        // Verify initial status is Draft
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{projectId}", UriKind.Relative));
        var project = await getResponse.Content.ReadFromJsonAsync<ProjectDto>();
        project!.Status.Should().Be(ProjectStatus.Draft);

        // Act - change status from Draft to Active
        var request = new ChangeProjectStatusCommand(projectId, ProjectStatus.Active);
        var response = await Client.PatchAsJsonAsync(new Uri($"{BaseUrl}/{projectId}/status", UriKind.Relative), request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify status changed
        var verifyResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{projectId}", UriKind.Relative));
        var updatedProject = await verifyResponse.Content.ReadFromJsonAsync<ProjectDto>();
        updatedProject!.Status.Should().Be(ProjectStatus.Active);
    }

    /// <summary>
    /// Tests that PATCH /api/v1/projects/{id}/status with invalid transition returns 400.
    /// </summary>
    [Fact]
    public async Task ChangeProjectStatus_InvalidTransition_ReturnsBadRequest()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");
        var projectId = await CreateTestProjectAsync("Status Test Project", clientId: clientId, sectorId: sectorId);

        // First change to Active
        var toActiveRequest = new ChangeProjectStatusCommand(projectId, ProjectStatus.Active);
        await Client.PatchAsJsonAsync(new Uri($"{BaseUrl}/{projectId}/status", UriKind.Relative), toActiveRequest);

        // Act - try invalid transition Active to Draft
        var invalidRequest = new ChangeProjectStatusCommand(projectId, ProjectStatus.Draft);
        var response = await Client.PatchAsJsonAsync(new Uri($"{BaseUrl}/{projectId}/status", UriKind.Relative), invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>();
        problemDetails.GetProperty("status").GetInt32().Should().Be(400);
    }

    /// <summary>
    /// Tests that PATCH /api/v1/projects/{id}/status with mismatched ID returns 400.
    /// </summary>
    [Fact]
    public async Task ChangeProjectStatus_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");
        var projectId = await CreateTestProjectAsync("Status Test Project", clientId: clientId, sectorId: sectorId);

        // Act - request with different ID
        var request = new ChangeProjectStatusCommand(Guid.NewGuid(), ProjectStatus.Active);
        var response = await Client.PatchAsJsonAsync(new Uri($"{BaseUrl}/{projectId}/status", UriKind.Relative), request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Tests that PATCH /api/v1/projects/{nonexistent}/status returns 404.
    /// </summary>
    [Fact]
    public async Task ChangeProjectStatus_WhenNotFound_ReturnsNotFound()
    {
        // Act
        var request = new ChangeProjectStatusCommand(Guid.NewGuid(), ProjectStatus.Active);
        var response = await Client.PatchAsJsonAsync(new Uri($"{BaseUrl}/{Guid.NewGuid()}/status", UriKind.Relative), request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that POST /api/v1/projects/{id}/services/{serviceId} associates a service with a project.
    /// </summary>
    [Fact]
    public async Task AddProjectService_WhenValid_ReturnsNoContent()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");
        var projectId = await CreateTestProjectAsync("Project with Services", clientId: clientId, sectorId: sectorId);
        var serviceId = await CreateTestServiceAsync("Web Development", "Test service", ServiceCategory.Development);

        // Act
        var response = await Client.PostAsync(new Uri($"{BaseUrl}/{projectId}/services/{serviceId}", UriKind.Relative), null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify association
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{projectId}", UriKind.Relative));
        var project = await getResponse.Content.ReadFromJsonAsync<ProjectDto>();
        project!.ServiceIds.Should().Contain(serviceId);
    }

    /// <summary>
    /// Tests that POST /api/v1/projects/{id}/services/{serviceId} with non-existent project returns 404.
    /// </summary>
    [Fact]
    public async Task AddProjectService_WhenProjectNotFound_ReturnsNotFound()
    {
        // Arrange
        var serviceId = await CreateTestServiceAsync("Web Development", "Test service", ServiceCategory.Development);

        // Act
        var response = await Client.PostAsync(new Uri($"{BaseUrl}/{Guid.NewGuid()}/services/{serviceId}", UriKind.Relative), null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that POST /api/v1/projects/{id}/services/{serviceId} with non-existent service returns 404.
    /// </summary>
    [Fact]
    public async Task AddProjectService_WhenServiceNotFound_ReturnsNotFound()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");
        var projectId = await CreateTestProjectAsync("Project with Services", clientId: clientId, sectorId: sectorId);

        // Act
        var response = await Client.PostAsync(new Uri($"{BaseUrl}/{projectId}/services/{Guid.NewGuid()}", UriKind.Relative), null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that DELETE /api/v1/projects/{id}/services/{serviceId} removes a service association.
    /// </summary>
    [Fact]
    public async Task RemoveProjectService_WhenValid_ReturnsNoContent()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");
        var projectId = await CreateTestProjectAsync("Project with Services", clientId: clientId, sectorId: sectorId);
        var serviceId = await CreateTestServiceAsync("Web Development", "Test service", ServiceCategory.Development);

        // First add the service
        await Client.PostAsync(new Uri($"{BaseUrl}/{projectId}/services/{serviceId}", UriKind.Relative), null);

        // Verify association exists
        var getBeforeResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{projectId}", UriKind.Relative));
        var projectBefore = await getBeforeResponse.Content.ReadFromJsonAsync<ProjectDto>();
        projectBefore!.ServiceIds.Should().Contain(serviceId);

        // Act - remove the service
        var response = await Client.DeleteAsync(new Uri($"{BaseUrl}/{projectId}/services/{serviceId}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify association removed
        var getAfterResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{projectId}", UriKind.Relative));
        var projectAfter = await getAfterResponse.Content.ReadFromJsonAsync<ProjectDto>();
        projectAfter!.ServiceIds.Should().NotContain(serviceId);
    }

    /// <summary>
    /// Tests that DELETE /api/v1/projects/{id}/services/{serviceId} with non-existent project returns 404.
    /// </summary>
    [Fact]
    public async Task RemoveProjectService_WhenProjectNotFound_ReturnsNotFound()
    {
        // Arrange
        var serviceId = await CreateTestServiceAsync("Web Development", "Test service", ServiceCategory.Development);

        // Act
        var response = await Client.DeleteAsync(new Uri($"{BaseUrl}/{Guid.NewGuid()}/services/{serviceId}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that DELETE /api/v1/projects/{id}/services/{serviceId} with non-existent service returns 404.
    /// </summary>
    [Fact]
    public async Task RemoveProjectService_WhenServiceNotFound_ReturnsNotFound()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");
        var projectId = await CreateTestProjectAsync("Project with Services", clientId: clientId, sectorId: sectorId);

        // Act
        var response = await Client.DeleteAsync(new Uri($"{BaseUrl}/{projectId}/services/{Guid.NewGuid()}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that multiple services can be associated with a project.
    /// </summary>
    [Fact]
    public async Task ProjectServices_MultipleAssociations_WorkCorrectly()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var sectorId = await CreateTestSectorAsync("Technology");
        var projectId = await CreateTestProjectAsync("Multi-Service Project", clientId: clientId, sectorId: sectorId);
        var service1Id = await CreateTestServiceAsync("Web Development", "Test service", ServiceCategory.Development);
        var service2Id = await CreateTestServiceAsync("Consulting", "Test service", ServiceCategory.Consulting);
        var service3Id = await CreateTestServiceAsync("DevOps", "Test service", ServiceCategory.DevOps);

        // Act - add multiple services
        await Client.PostAsync(new Uri($"{BaseUrl}/{projectId}/services/{service1Id}", UriKind.Relative), null);
        await Client.PostAsync(new Uri($"{BaseUrl}/{projectId}/services/{service2Id}", UriKind.Relative), null);
        await Client.PostAsync(new Uri($"{BaseUrl}/{projectId}/services/{service3Id}", UriKind.Relative), null);

        // Verify all associations
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{projectId}", UriKind.Relative));
        var project = await getResponse.Content.ReadFromJsonAsync<ProjectDto>();
        project!.ServiceIds.Should().HaveCount(3);
        project.ServiceIds.Should().Contain(service1Id);
        project.ServiceIds.Should().Contain(service2Id);
        project.ServiceIds.Should().Contain(service3Id);

        // Act - remove one service
        await Client.DeleteAsync(new Uri($"{BaseUrl}/{projectId}/services/{service2Id}", UriKind.Relative));

        // Verify remaining associations
        var getAfterResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{projectId}", UriKind.Relative));
        var projectAfter = await getAfterResponse.Content.ReadFromJsonAsync<ProjectDto>();
        projectAfter!.ServiceIds.Should().HaveCount(2);
        projectAfter.ServiceIds.Should().Contain(service1Id);
        projectAfter.ServiceIds.Should().Contain(service3Id);
        projectAfter.ServiceIds.Should().NotContain(service2Id);
    }

    /// <summary>
    /// Tests that creating a project with non-existent client returns 400.
    /// </summary>
    [Fact]
    public async Task CreateProject_WithNonExistentClient_ReturnsBadRequest()
    {
        // Arrange
        var sectorId = await CreateTestSectorAsync("Technology");
        var nonExistentClientId = Guid.NewGuid();

        var request = new CreateProjectCommand(
            "Test Project",
            "Test Description",
            YearMonth.Create(6, 2024),
            6,
            nonExistentClientId,
            sectorId,
            new List<string> { "C#" });

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Tests that creating a project with non-existent sector returns 400.
    /// </summary>
    [Fact]
    public async Task CreateProject_WithNonExistentSector_ReturnsBadRequest()
    {
        // Arrange
        var clientId = await CreateTestClientAsync("Test Client", "test@client.com");
        var nonExistentSectorId = Guid.NewGuid();

        var request = new CreateProjectCommand(
            "Test Project",
            "Test Description",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            nonExistentSectorId,
            new List<string> { "C#" });

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
