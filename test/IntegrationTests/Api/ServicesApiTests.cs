// <copyright file="ServicesApiTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Api;

using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.IntegrationTests;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the Services API endpoints.
/// </summary>
public sealed class ServicesApiTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/v1/services";

    /// <summary>
    /// Tests that GET /api/v1/services returns 200 with empty array initially.
    /// </summary>
    [Fact]
    public async Task GetServices_WhenEmpty_ReturnsEmptyArray()
    {
        // Act
        var response = await Client.GetAsync(new Uri(BaseUrl, UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var services = await response.Content.ReadFromJsonAsync<IReadOnlyList<ServiceListItemDto>>();
        services.Should().NotBeNull();
        services.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GET /api/v1/services/active returns 200 with empty array initially.
    /// </summary>
    [Fact]
    public async Task GetActiveServices_WhenEmpty_ReturnsEmptyArray()
    {
        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/active", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var services = await response.Content.ReadFromJsonAsync<IReadOnlyList<ServiceListItemDto>>();
        services.Should().NotBeNull();
        services.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GET /api/v1/services/category/{category} returns 200 with empty array initially.
    /// </summary>
    [Fact]
    public async Task GetServicesByCategory_WhenEmpty_ReturnsEmptyArray()
    {
        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/category/{ServiceCategory.Development}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var services = await response.Content.ReadFromJsonAsync<IReadOnlyList<ServiceListItemDto>>();
        services.Should().NotBeNull();
        services.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that POST /api/v1/services with valid body returns 201 with Location header and ServiceDto.
    /// </summary>
    [Fact]
    public async Task CreateService_WithValidBody_ReturnsCreatedWithLocationAndDto()
    {
        // Arrange
        var request = new CreateServiceCommand("Web Development", "Custom web application development", ServiceCategory.Development);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().StartWith($"{BaseUrl}/");

        var service = await response.Content.ReadFromJsonAsync<ServiceDto>();
        service.Should().NotBeNull();
        service!.Name.Should().Be("Web Development");
        service.Description.Should().Be("Custom web application development");
        service.Category.Should().Be(ServiceCategory.Development);
        service.IsActive.Should().BeTrue();
        service.Id.Should().NotBeEmpty();
        service.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that POST /api/v1/services with minimal valid body returns 201.
    /// </summary>
    [Fact]
    public async Task CreateService_WithMinimalBody_ReturnsCreated()
    {
        // Arrange
        var request = new CreateServiceCommand("Minimal Service", null, ServiceCategory.Consulting);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var service = await response.Content.ReadFromJsonAsync<ServiceDto>();
        service.Should().NotBeNull();
        service!.Name.Should().Be("Minimal Service");
        service.Description.Should().BeNull();
        service.Category.Should().Be(ServiceCategory.Consulting);
        service.IsActive.Should().BeTrue();
    }

    /// <summary>
    /// Tests that GET /api/v1/services/{id} returns 200 with ServiceDto.
    /// </summary>
    [Fact]
    public async Task GetServiceById_WhenExists_ReturnsServiceDto()
    {
        // Arrange - create a service first
        var createRequest = new CreateServiceCommand("Get By ID Service", "Service for get by ID test", ServiceCategory.Design);
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<ServiceDto>();

        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/{createdService!.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var service = await response.Content.ReadFromJsonAsync<ServiceDto>();
        service.Should().NotBeNull();
        service!.Id.Should().Be(createdService.Id);
        service.Name.Should().Be("Get By ID Service");
        service.Description.Should().Be("Service for get by ID test");
        service.Category.Should().Be(ServiceCategory.Design);
    }

    /// <summary>
    /// Tests that PUT /api/v1/services/{id} returns 204.
    /// </summary>
    [Fact]
    public async Task UpdateService_WhenExists_ReturnsNoContent()
    {
        // Arrange - create a service first
        var createRequest = new CreateServiceCommand("Update Test", "Original description", ServiceCategory.DevOps);
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<ServiceDto>();

        // Act - update the service
        var updateRequest = new UpdateServiceCommand(createdService!.Id, "Updated Service", "Updated description", ServiceCategory.Training);
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{createdService.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdService.Id}", UriKind.Relative));
        var updatedService = await getResponse.Content.ReadFromJsonAsync<ServiceDto>();
        updatedService!.Name.Should().Be("Updated Service");
        updatedService.Description.Should().Be("Updated description");
        updatedService.Category.Should().Be(ServiceCategory.Training);
    }

    /// <summary>
    /// Tests that DELETE /api/v1/services/{id} returns 204.
    /// </summary>
    [Fact]
    public async Task DeleteService_WhenExists_ReturnsNoContent()
    {
        // Arrange - create a service first
        var createRequest = new CreateServiceCommand("Delete Test", "Service to be deleted", ServiceCategory.Development);
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<ServiceDto>();

        // Act
        var response = await Client.DeleteAsync(new Uri($"{BaseUrl}/{createdService!.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdService.Id}", UriKind.Relative));
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that POST /api/v1/services with invalid body returns 400 ProblemDetails.
    /// </summary>
    [Fact]
    public async Task CreateService_WithInvalidBody_ReturnsBadRequestWithProblemDetails()
    {
        // Arrange - empty name should fail validation
        var request = new CreateServiceCommand(string.Empty, "Test description", ServiceCategory.Development);

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
    /// Tests that GET /api/v1/services/{nonexistent} returns 404 ProblemDetails.
    /// </summary>
    [Fact]
    public async Task GetServiceById_WhenNotFound_ReturnsNotFoundWithProblemDetails()
    {
        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/{Guid.NewGuid()}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>();
        problemDetails.GetProperty("status").GetInt32().Should().Be(404);
        problemDetails.GetProperty("title").GetString().Should().Be("Not Found");
    }

    /// <summary>
    /// Tests that PUT /api/v1/services/{id} with mismatched ID returns 400.
    /// </summary>
    [Fact]
    public async Task UpdateService_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateServiceCommand("Mismatch Test", "Test mismatch", ServiceCategory.Development);
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdService = await createResponse.Content.ReadFromJsonAsync<ServiceDto>();

        // Act - update with different ID
        var updateRequest = new UpdateServiceCommand(Guid.NewGuid(), "Updated", "Updated description", ServiceCategory.Consulting);
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{createdService!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Tests that PUT /api/v1/services/{id} for non-existent service returns 404.
    /// </summary>
    [Fact]
    public async Task UpdateService_WhenNotFound_ReturnsNotFound()
    {
        // Act
        var updateRequest = new UpdateServiceCommand(Guid.NewGuid(), "Updated", "Updated description", ServiceCategory.Consulting);
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{Guid.NewGuid()}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that DELETE /api/v1/services/{nonexistent} returns 404.
    /// </summary>
    [Fact]
    public async Task DeleteService_WhenNotFound_ReturnsNotFound()
    {
        // Act
        var response = await Client.DeleteAsync(new Uri($"{BaseUrl}/{Guid.NewGuid()}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that GET /api/v1/services returns all services.
    /// </summary>
    [Fact]
    public async Task GetServices_WhenMultipleExist_ReturnsAll()
    {
        // Arrange - create multiple services
        var project1 = await Client.PostAsJsonAsync(BaseUrl, new CreateServiceCommand("Service 1", "First service", ServiceCategory.Development));
        var project2 = await Client.PostAsJsonAsync(BaseUrl, new CreateServiceCommand("Service 2", "Second service", ServiceCategory.Consulting));

        var p1 = await project1.Content.ReadFromJsonAsync<ServiceDto>();
        var p2 = await project2.Content.ReadFromJsonAsync<ServiceDto>();

        // Act
        var response = await Client.GetAsync(new Uri(BaseUrl, UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var services = await response.Content.ReadFromJsonAsync<IReadOnlyList<ServiceListItemDto>>();
        services.Should().NotBeNull();
        services!.Should().HaveCount(2);
        services.Should().Contain(c => c.Id == p1!.Id);
        services.Should().Contain(c => c.Id == p2!.Id);
    }

    /// <summary>
    /// Tests that GET /api/v1/services/active returns only active services.
    /// </summary>
    [Fact]
    public async Task GetActiveServices_ReturnsOnlyActiveServices()
    {
        // Arrange - create services (both are active on creation)
        var activeRequest = new CreateServiceCommand("Active Service", "Active service", ServiceCategory.Development);
        var activeResponse = await Client.PostAsJsonAsync(BaseUrl, activeRequest);
        var activeService = await activeResponse.Content.ReadFromJsonAsync<ServiceDto>();

        var inactiveRequest = new CreateServiceCommand("Inactive Service", "Inactive service", ServiceCategory.Consulting);
        var inactiveResponse = await Client.PostAsJsonAsync(BaseUrl, inactiveRequest);
        var inactiveService = await inactiveResponse.Content.ReadFromJsonAsync<ServiceDto>();

        // Both are active on creation
        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/active", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var services = await response.Content.ReadFromJsonAsync<IReadOnlyList<ServiceListItemDto>>();
        services.Should().NotBeNull();
        services!.Should().HaveCount(2);
        services.Should().Contain(c => c.Id == activeService!.Id);
        services.Should().Contain(c => c.Id == inactiveService!.Id);
    }

    /// <summary>
    /// Tests that GET /api/v1/services/category/{category} returns services filtered by category.
    /// </summary>
    [Fact]
    public async Task GetServicesByCategory_ReturnsFilteredServices()
    {
        // Arrange - create services in different categories
        var devRequest = new CreateServiceCommand("Dev Service", "Development service", ServiceCategory.Development);
        var devResponse = await Client.PostAsJsonAsync(BaseUrl, devRequest);
        var devService = await devResponse.Content.ReadFromJsonAsync<ServiceDto>();

        var consultingRequest = new CreateServiceCommand("Consulting Service", "Consulting service", ServiceCategory.Consulting);
        var consultingResponse = await Client.PostAsJsonAsync(BaseUrl, consultingRequest);
        var consultingService = await consultingResponse.Content.ReadFromJsonAsync<ServiceDto>();

        var designRequest = new CreateServiceCommand("Design Service", "Design service", ServiceCategory.Design);
        var designResponse = await Client.PostAsJsonAsync(BaseUrl, designRequest);
        var designService = await designResponse.Content.ReadFromJsonAsync<ServiceDto>();

        // Act - get services by Development category
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/category/{ServiceCategory.Development}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var services = await response.Content.ReadFromJsonAsync<IReadOnlyList<ServiceListItemDto>>();
        services.Should().NotBeNull();
        services!.Should().HaveCount(1);
        services.Should().Contain(c => c.Id == devService!.Id);
        services.Should().NotContain(c => c.Id == consultingService!.Id);
        services.Should().NotContain(c => c.Id == designService!.Id);

        // Act - get services by Consulting category
        var consultingFilterResponse = await Client.GetAsync(new Uri($"{BaseUrl}/category/{ServiceCategory.Consulting}", UriKind.Relative));

        // Assert
        consultingFilterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var consultingServices = await consultingFilterResponse.Content.ReadFromJsonAsync<IReadOnlyList<ServiceListItemDto>>();
        consultingServices.Should().NotBeNull();
        consultingServices!.Should().HaveCount(1);
        consultingServices.Should().Contain(c => c.Id == consultingService!.Id);
    }
}