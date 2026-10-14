// <copyright file="ClientsApiTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Api;

using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.IntegrationTests;
using Tfs.Portfolio.IntegrationTests.Common;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the Clients API endpoints.
/// </summary>
public sealed class ClientsApiTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/v1/clients";

    /// <summary>
    /// Tests that GET /api/v1/clients returns 200 with empty array initially.
    /// </summary>
    [Fact]
    public async Task GetClients_WhenEmpty_ReturnsEmptyArray()
    {
        // Act
        var response = await Client.GetAsync(new Uri(BaseUrl, UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clients = await response.Content.ReadFromJsonNewtonsoftAsync<IReadOnlyList<ClientListItemDto>>();
        clients.Should().NotBeNull();
        clients.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GET /api/v1/clients/active returns 200 with empty array initially.
    /// </summary>
    [Fact]
    public async Task GetActiveClients_WhenEmpty_ReturnsEmptyArray()
    {
        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/active", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clients = await response.Content.ReadFromJsonNewtonsoftAsync<IReadOnlyList<ClientListItemDto>>();
        clients.Should().NotBeNull();
        clients.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that POST /api/v1/clients with valid body returns 201 with Location header and ClientDto.
    /// </summary>
    [Fact]
    public async Task CreateClient_WithValidBody_ReturnsCreatedWithLocationAndDto()
    {
        // Arrange
        var request = new CreateClientCommand("Test Client", "test@example.com", "https://example.com/logo.png", "+1234567890", "123 Client St");

        // Act
        var response = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().StartWith($"{BaseUrl}/");

        var client = await response.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        client.Should().NotBeNull();
        client!.Name.Should().Be("Test Client");
        client.Email.Should().Be("test@example.com");
        var logoUrlValue = client.LogoUrl?.Value;
        var expectedUrl = "https://example.com/logo.png";
        // Debug logging removed - CA1303 compliance
        var minLen = Math.Min(logoUrlValue?.Length ?? 0, expectedUrl.Length);
        for (int i = 0; i < Math.Min(logoUrlValue?.Length ?? 0, expectedUrl.Length); i++)
        {
            if (logoUrlValue[i] != expectedUrl[i])
            {
                System.Console.WriteLine($"DEBUG: Diff at index {i}: actual='{logoUrlValue[i]}' expected='{expectedUrl[i]}'");
            }
        }
        logoUrlValue.Should().Be(expectedUrl);
        client.Phone.Should().Be("+1234567890");
        client.Address.Should().Be("123 Client St");
        client.IsActive.Should().BeTrue();
        client.Id.Should().NotBeEmpty();
        client.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that POST /api/v1/clients with minimal valid body returns 201.
    /// </summary>
    [Fact]
    public async Task CreateClient_WithMinimalBody_ReturnsCreated()
    {
        // Arrange
        var request = new CreateClientCommand("Minimal Client", "minimal@client.com", null, null, null);

        // Act
        var response = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var client = await response.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        client.Should().NotBeNull();
        client!.Name.Should().Be("Minimal Client");
        client.Email.Should().Be("minimal@client.com");
        client.LogoUrl.Should().BeNull();
        client.Phone.Should().BeNull();
        client.Address.Should().BeNull();
    }

    /// <summary>
    /// Tests that GET /api/v1/clients/{id} returns 200 with ClientDto.
    /// </summary>
    [Fact]
    public async Task GetClientById_WhenExists_ReturnsClientDto()
    {
        // Arrange - create a client first
        var createRequest = new CreateClientCommand("Get By ID Client", "getbyid@client.com", TfsWebUrl.Create("https://client.com/logo.png"), "+1987654321", "456 Client Ave");
        var createResponse = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();

        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient!.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var client = await response.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        client.Should().NotBeNull();
        client!.Id.Should().Be(createdClient.Id);
        client.Name.Should().Be("Get By ID Client");
        client.Email.Should().Be("getbyid@client.com");
    }

    /// <summary>
    /// Tests that PUT /api/v1/clients/{id} returns 204.
    /// </summary>
    [Fact]
    public async Task UpdateClient_WhenExists_ReturnsNoContent()
    {
        // Arrange - create a client first
        var createRequest = new CreateClientCommand("Update Test", "update@client.com", null, null, null);
        var createResponse = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();

        // Act - update the client
        var updateRequest = new UpdateClientCommand(createdClient!.Id, "Updated Client", "updated@client.com", TfsWebUrl.Create("https://updated.com/logo.png"), "+1112223333", "789 Updated Blvd", null);
        var response = await Client.PutAsJsonNewtonsoftAsync($"{BaseUrl}/{createdClient.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient.Id}", UriKind.Relative));
        var updatedClient = await getResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        updatedClient!.Name.Should().Be("Updated Client");
        updatedClient.Email.Should().Be("updated@client.com");
    }

    /// <summary>
    /// Tests that PUT /api/v1/clients/{id} with IsActive updates the status.
    /// </summary>
    [Fact]
    public async Task UpdateClient_WithIsActive_UpdatesStatus()
    {
        // Arrange - create a client first
        var createRequest = new CreateClientCommand("Status Test", "status@client.com", null, null, null);
        var createResponse = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        createdClient!.IsActive.Should().BeTrue();

        // Act - deactivate the client via update
        var updateRequest = new UpdateClientCommand(createdClient.Id, "Status Test", "status@client.com", null, null, null, false);
        var response = await Client.PutAsJsonNewtonsoftAsync($"{BaseUrl}/{createdClient.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the deactivation
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient.Id}", UriKind.Relative));
        var updatedClient = await getResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        updatedClient!.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Tests that PUT /api/v1/clients/{id}/deactivate returns 204.
    /// </summary>
    [Fact]
    public async Task DeactivateClient_WhenExists_ReturnsNoContent()
    {
        // Arrange - create a client first
        var createRequest = new CreateClientCommand("Deactivate Test", "deactivate@client.com", null, null, null);
        var createResponse = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        createdClient!.IsActive.Should().BeTrue();

        // Act
        var response = await Client.PutAsJsonNewtonsoftAsync($"{BaseUrl}/{createdClient.Id}/deactivate", (object?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the deactivation
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient.Id}", UriKind.Relative));
        var deactivatedClient = await getResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        deactivatedClient!.IsActive.Should().BeFalse();
    }

    /// <summary>
    /// Tests that DELETE /api/v1/clients/{id} returns 204.
    /// </summary>
    [Fact]
    public async Task DeleteClient_WhenExists_ReturnsNoContent()
    {
        // Arrange - create a client first
        var createRequest = new CreateClientCommand("Delete Test", "delete@client.com", null, null, null);
        var createResponse = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();

        // Act
        var response = await Client.DeleteAsync(new Uri($"{BaseUrl}/{createdClient!.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient.Id}", UriKind.Relative));
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that POST /api/v1/clients with invalid body returns 400 ProblemDetails.
    /// </summary>
    [Fact]
    public async Task CreateClient_WithInvalidBody_ReturnsBadRequestWithProblemDetails()
    {
        // Arrange - empty name should fail validation
        var request = new CreateClientCommand(string.Empty, "test@client.com", null, null, null);

        // Act
        var response = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonNewtonsoftAsync<ProblemDetailsDto>();
        problemDetails.Status.Should().Be(400);
        problemDetails.Title.Should().Be("Validation Failed");
        problemDetails.Detail.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that POST /api/v1/clients with invalid email returns 400 ProblemDetails.
    /// </summary>
    [Fact]
    public async Task CreateClient_WithInvalidEmail_ReturnsBadRequestWithProblemDetails()
    {
        // Arrange - invalid email should fail validation
        var request = new CreateClientCommand("Test Client", "invalid-email", null, null, null);

        // Act
        var response = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonNewtonsoftAsync<ProblemDetailsDto>();
        problemDetails.Status.Should().Be(400);
    }

    /// <summary>
    /// Tests that GET /api/v1/clients/{nonexistent} returns 404 ProblemDetails.
    /// </summary>
    [Fact]
    public async Task GetClientById_WhenNotFound_ReturnsNotFoundWithProblemDetails()
    {
        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/{Guid.NewGuid()}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problemDetails = await response.Content.ReadFromJsonNewtonsoftAsync<ProblemDetailsDto>();
        problemDetails.Status.Should().Be(404);
        problemDetails.Title.Should().Be("Not Found");
    }

    /// <summary>
    /// Tests that PUT /api/v1/clients/{id} with mismatched ID returns 400.
    /// </summary>
    [Fact]
    public async Task UpdateClient_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateClientCommand("Mismatch Test", "mismatch@client.com", null, null, null);
        var createResponse = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();

        // Act - update with different ID
        var updateRequest = new UpdateClientCommand(Guid.NewGuid(), "Updated", "updated@client.com", null, null, null, null);
        var response = await Client.PutAsJsonNewtonsoftAsync($"{BaseUrl}/{createdClient!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Tests that PUT /api/v1/clients/{id}/deactivate for non-existent client returns 404.
    /// </summary>
    [Fact]
    public async Task DeactivateClient_WhenNotFound_ReturnsNotFound()
    {
        // Act
        var response = await Client.PutAsJsonNewtonsoftAsync($"{BaseUrl}/{Guid.NewGuid()}/deactivate", (object?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that DELETE /api/v1/clients/{nonexistent} returns 404.
    /// </summary>
    [Fact]
    public async Task DeleteClient_WhenNotFound_ReturnsNotFound()
    {
        // Act
        var response = await Client.DeleteAsync(new Uri($"{BaseUrl}/{Guid.NewGuid()}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Tests that GET /api/v1/clients returns all clients.
    /// </summary>
    [Fact]
    public async Task GetClients_WhenMultipleExist_ReturnsAll()
    {
        // Arrange - create multiple clients
        var project1 = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, new CreateClientCommand("Client 1", "client1@test.com", null, null, null));
        var project2 = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, new CreateClientCommand("Client 2", "client2@test.com", null, null, null));

        var p1 = await project1.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();
        var p2 = await project2.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();

        // Act
        var response = await Client.GetAsync(new Uri(BaseUrl, UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clients = await response.Content.ReadFromJsonNewtonsoftAsync<IReadOnlyList<ClientListItemDto>>();
        clients.Should().NotBeNull();
        clients!.Should().HaveCount(2);
        clients.Should().Contain(c => c.Id == p1!.Id);
        clients.Should().Contain(c => c.Id == p2!.Id);
    }

    /// <summary>
    /// Tests that GET /api/v1/clients/active returns only active clients.
    /// </summary>
    [Fact]
    public async Task GetActiveClients_ReturnsOnlyActiveClients()
    {
        // Arrange - create an active and an inactive client
        var activeRequest = new CreateClientCommand("Active Client", "active@test.com", null, null, null);
        var activeResponse = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, activeRequest);
        var activeClient = await activeResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();

        var inactiveRequest = new CreateClientCommand("Inactive Client", "inactive@test.com", null, null, null);
        var inactiveResponse = await Client.PostAsJsonNewtonsoftAsync(BaseUrl, inactiveRequest);
        var inactiveClient = await inactiveResponse.Content.ReadFromJsonNewtonsoftAsync<ClientDto>();

        // Deactivate the second client
        await Client.PutAsJsonNewtonsoftAsync($"{BaseUrl}/{inactiveClient!.Id}/deactivate", (object?)null);

        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/active", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clients = await response.Content.ReadFromJsonNewtonsoftAsync<IReadOnlyList<ClientListItemDto>>();
        clients.Should().NotBeNull();
        clients!.Should().HaveCount(1);
        clients.Should().Contain(c => c.Id == activeClient!.Id);
        clients.Should().NotContain(c => c.Id == inactiveClient!.Id);
    }
}
