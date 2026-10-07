// <copyright file="ProjectsApiTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Api;

using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.IntegrationTests;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the Projects API endpoints.
/// </summary>
public sealed class ProjectsApiTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/v1/projects";
    private readonly Guid _testClientId = Guid.NewGuid();
    private readonly Guid _testSectorId = Guid.NewGuid();

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
        // Arrange
        var request = new CreateProjectCommand(
            "Test Project",
            "Test Description",
            YearMonth.Create(6, 2024),
            6,
            _testClientId,
            _testSectorId);

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
        var createRequest = new CreateProjectCommand(
            "Get By ID Test",
            "Description",
            YearMonth.Create(6, 2024),
            6,
            _testClientId,
            _testSectorId);
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
        var createRequest = new CreateProjectCommand(
            "Update Test",
            "Original Description",
            YearMonth.Create(6, 2024),
            6,
            _testClientId,
            _testSectorId);
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        // Act - update the project
        var updateRequest = new UpdateProjectCommand(
            createdProject!.Id,
            "Updated Name",
            "Updated Description",
            YearMonth.Create(7, 2024),
            8,
            _testClientId,
            _testSectorId);
        var response = await Client.PutAsJsonAsync(
            $"{BaseUrl}/{createdProject.Id}",
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
        var createRequest = new CreateProjectCommand(
            "Delete Test",
            "To be deleted",
            YearMonth.Create(6, 2024),
            6,
            _testClientId,
            _testSectorId);
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
        // Arrange - empty name should fail validation
        var request = new CreateProjectCommand(
            string.Empty,
            "Test Description",
            YearMonth.Create(6, 2024),
            6,
            _testClientId,
            _testSectorId);

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
        // Arrange
        var createRequest = new CreateProjectCommand(
            "Mismatch Test",
            "Test",
            YearMonth.Create(6, 2024),
            6,
            _testClientId,
            _testSectorId);
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        // Act - update with different ID
        var updateRequest = new UpdateProjectCommand(
            Guid.NewGuid(),
            "Updated",
            "Test",
            YearMonth.Create(6, 2024),
            6,
            _testClientId,
            _testSectorId);
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
        // Arrange - create multiple projects
        var project1 = await Client.PostAsJsonAsync(
            BaseUrl,
            new CreateProjectCommand(
                "Project 1",
                "Desc 1",
                YearMonth.Create(6, 2024),
                6,
                _testClientId,
                _testSectorId));
        var project2 = await Client.PostAsJsonAsync(
            BaseUrl,
            new CreateProjectCommand(
                "Project 2",
                "Desc 2",
                YearMonth.Create(7, 2024),
                8,
                _testClientId,
                _testSectorId));

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
}