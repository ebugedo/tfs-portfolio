// <copyright file="ClientsApiTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Api;

using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.IntegrationTests;
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
        var clients = await response.Content.ReadFromJsonAsync<IReadOnlyList<ClientListItemDto>>();
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
        var clients = await response.Content.ReadFromJsonAsync<IReadOnlyList<ClientListItemDto>>();
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

        // Debug: serialize request to see JSON
        var json = System.Text.Json.JsonSerializer.Serialize(request);
        System.Console.WriteLine($"Request JSON: {json}");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Debug: read response content for debugging
        var responseContent = await response.Content.ReadAsStringAsync();
        System.Console.WriteLine($"Response content: {responseContent}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created, because: $"Response content: {responseContent}");
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().StartWith($"{BaseUrl}/");

        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
        client.Should().NotBeNull();
        client!.Name.Should().Be("Test Client");
        client.Email.Should().Be("test@example.com");
        client.LogoUrl!.Value.Should().Be("https://example.com/logo.png");
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
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
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
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient!.Id}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
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
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - update the client
        var updateRequest = new UpdateClientCommand(createdClient!.Id, "Updated Client", "updated@client.com", TfsWebUrl.Create("https://updated.com/logo.png"), "+1112223333", "789 Updated Blvd", null);
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{createdClient.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient.Id}", UriKind.Relative));
        var updatedClient = await getResponse.Content.ReadFromJsonAsync<ClientDto>();
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
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();
        createdClient!.IsActive.Should().BeTrue();

        // Act - deactivate the client via update
        var updateRequest = new UpdateClientCommand(createdClient.Id, "Status Test", "status@client.com", null, null, null, false);
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{createdClient.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the deactivation
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient.Id}", UriKind.Relative));
        var updatedClient = await getResponse.Content.ReadFromJsonAsync<ClientDto>();
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
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();
        createdClient!.IsActive.Should().BeTrue();

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{createdClient.Id}/deactivate", (object?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the deactivation
        var getResponse = await Client.GetAsync(new Uri($"{BaseUrl}/{createdClient.Id}", UriKind.Relative));
        var deactivatedClient = await getResponse.Content.ReadFromJsonAsync<ClientDto>();
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
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

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
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>();
        problemDetails.GetProperty("status").GetInt32().Should().Be(400);
        problemDetails.GetProperty("title").GetString().Should().Be("Validation Failed");
        problemDetails.GetProperty("detail").GetString().Should().NotBeNullOrEmpty();
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
        var response = await Client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>();
        problemDetails.GetProperty("status").GetInt32().Should().Be(400);
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
        var problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>();
        problemDetails.GetProperty("status").GetInt32().Should().Be(404);
        problemDetails.GetProperty("title").GetString().Should().Be("Not Found");
    }

    /// <summary>
    /// Tests that PUT /api/v1/clients/{id} with mismatched ID returns 400.
    /// </summary>
    [Fact]
    public async Task UpdateClient_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateClientCommand("Mismatch Test", "mismatch@client.com", null, null, null);
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Act - update with different ID
        var updateRequest = new UpdateClientCommand(Guid.NewGuid(), "Updated", "updated@client.com", null, null, null, null);
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{createdClient!.Id}", updateRequest);

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
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{Guid.NewGuid()}/deactivate", (object?)null);

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
        var project1 = await Client.PostAsJsonAsync(BaseUrl, new CreateClientCommand("Client 1", "client1@test.com", null, null, null));
        var project2 = await Client.PostAsJsonAsync(BaseUrl, new CreateClientCommand("Client 2", "client2@test.com", null, null, null));

        var p1 = await project1.Content.ReadFromJsonAsync<ClientDto>();
        var p2 = await project2.Content.ReadFromJsonAsync<ClientDto>();

        // Act
        var response = await Client.GetAsync(new Uri(BaseUrl, UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clients = await response.Content.ReadFromJsonAsync<IReadOnlyList<ClientListItemDto>>();
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
        var activeResponse = await Client.PostAsJsonAsync(BaseUrl, activeRequest);
        var activeClient = await activeResponse.Content.ReadFromJsonAsync<ClientDto>();

        var inactiveRequest = new CreateClientCommand("Inactive Client", "inactive@test.com", null, null, null);
        var inactiveResponse = await Client.PostAsJsonAsync(BaseUrl, inactiveRequest);
        var inactiveClient = await inactiveResponse.Content.ReadFromJsonAsync<ClientDto>();

        // Deactivate the second client
        await Client.PutAsJsonAsync($"{BaseUrl}/{inactiveClient!.Id}/deactivate", (object?)null);

        // Act
        var response = await Client.GetAsync(new Uri($"{BaseUrl}/active", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clients = await response.Content.ReadFromJsonAsync<IReadOnlyList<ClientListItemDto>>();
        clients.Should().NotBeNull();
        clients!.Should().HaveCount(1);
        clients.Should().Contain(c => c.Id == activeClient!.Id);
        clients.Should().NotContain(c => c.Id == inactiveClient!.Id);
    }
}