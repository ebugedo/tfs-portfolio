// <copyright file="ApiKeyAuthTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Api;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.IntegrationTests;
using Tfs.Portfolio.IntegrationTests.Common;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for API Key authentication on protected endpoints.
/// </summary>
public sealed class ApiKeyAuthTests : IntegrationTestBase
{
    private const string BaseUrl = "/api/v1/clients";
    private const string ValidApiKey = "test-api-key-12345";
    private const string InvalidApiKey = "invalid-api-key";

    /// <summary>
    /// Tests that GET /api/v1/clients returns 200 without an X-API-Key header (GET is public).
    /// </summary>
    [Fact]
    public async Task GetClients_WithoutApiKey_ReturnsOk()
    {
        // Act
        var response = await Client.GetAsync(new Uri(BaseUrl, UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Tests that POST /api/v1/clients returns 401 without an X-API-Key header.
    /// </summary>
    [Fact]
    public async Task CreateClient_WithoutApiKey_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateClientCommand("Test Client", "test@example.com", "https://example.com/logo.png", "+1234567890", "123 Client St");

        // Act
        var clientWithoutKey = _factory.CreateClient();
        var response = await clientWithoutKey.PostAsJsonNewtonsoftAsync(BaseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Tests that POST /api/v1/clients returns 401 with an invalid X-API-Key header.
    /// </summary>
    [Fact]
    public async Task CreateClient_WithInvalidApiKey_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateClientCommand("Test Client", "test@example.com", "https://example.com/logo.png", "+1234567890", "123 Client St");

        // Act
        using var requestWithInvalidKey = new HttpRequestMessage(HttpMethod.Post, BaseUrl)
        {
            Content = JsonContent.Create(request)
        };
        requestWithInvalidKey.Headers.Add("X-API-Key", InvalidApiKey);

        var response = await Client.SendAsync(requestWithInvalidKey);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Tests that POST /api/v1/clients returns 201 with a valid X-API-Key header.
    /// </summary>
    [Fact]
    public async Task CreateClient_WithValidApiKey_ReturnsCreated()
    {
        // Arrange
        var request = new CreateClientCommand("Test Client", "test@example.com", "https://example.com/logo.png", "+1234567890", "123 Client St");

        // Act
        using var requestWithValidKey = new HttpRequestMessage(HttpMethod.Post, BaseUrl)
        {
            Content = JsonContent.Create(request)
        };
        requestWithValidKey.Headers.Add("X-API-Key", ValidApiKey);

        var response = await Client.SendAsync(requestWithValidKey);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    /// <summary>
    /// Tests that PUT /api/v1/clients/{id} returns 401 without an X-API-Key header.
    /// </summary>
    [Fact]
    public async Task UpdateClient_WithoutApiKey_ReturnsUnauthorized()
    {
        // Arrange
        var updateRequest = new UpdateClientCommand(
            Guid.NewGuid(), "Updated Client", "updated@client.com", null, null, null, null);

        // Act
        var clientWithoutKey = _factory.CreateClient();
        var response = await clientWithoutKey.PutAsJsonNewtonsoftAsync($"{BaseUrl}/{Guid.NewGuid()}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Tests that DELETE /api/v1/clients/{id} returns 401 without an X-API-Key header.
    /// </summary>
    [Fact]
    public async Task DeleteClient_WithoutApiKey_ReturnsUnauthorized()
    {
        // Act
        var clientWithoutKey = _factory.CreateClient();
        var response = await clientWithoutKey.DeleteAsync(new Uri($"{BaseUrl}/{Guid.NewGuid()}", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Tests that PUT /api/v1/clients/{id}/deactivate returns 401 without an X-API-Key header.
    /// </summary>
    [Fact]
    public async Task DeactivateClient_WithoutApiKey_ReturnsUnauthorized()
    {
        // Act
        var clientWithoutKey = _factory.CreateClient();
        var response = await clientWithoutKey.PutAsJsonNewtonsoftAsync($"{BaseUrl}/{Guid.NewGuid()}/deactivate", (object?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
