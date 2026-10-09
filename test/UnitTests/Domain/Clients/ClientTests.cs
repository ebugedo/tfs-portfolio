// <copyright file="ClientTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain.Clients;

using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Clients.Exceptions;
using Tfs.Portfolio.Domain.Clients.Events;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Xunit;

/// <summary>
/// Unit tests for <see cref="Client"/> aggregate.
/// </summary>
public sealed class ClientTests
{
    /// <summary>
    /// Tests that Client can be created with valid data.
    /// </summary>
    [Fact]
    public void Create_WithValidData_RaisesClientCreatedEvent()
    {
        // Act
        var client = Client.Create("Acme Corp", "contact@acme.com", TfsWebUrl.Create("https://acme.com/logo.png"), "+1234567890", "123 Street");

        // Assert
        Assert.NotEqual(Guid.Empty, client.Id);
        Assert.Equal("Acme Corp", client.Name);
        Assert.Equal("contact@acme.com", client.Email);
        Assert.Equal("https://acme.com/logo.png", client.LogoUrl?.Value);
        Assert.Equal("+1234567890", client.Phone);
        Assert.Equal("123 Street", client.Address);
        Assert.True(client.IsActive);
        Assert.True(client.CreatedAt <= DateTime.UtcNow);
        Assert.Null(client.UpdatedAt);

        // Verify domain event
        var events = client.Events;
        Assert.Single(events);
        var createdEvent = Assert.IsType<ClientCreatedEvent>(events[0]);
        Assert.Equal(client.Id, createdEvent.ClientId);
        Assert.Equal("Acme Corp", createdEvent.ClientName);
    }

    /// <summary>
    /// Tests that Client creation fails with empty name.
    /// </summary>
    [Fact]
    public void Create_WithEmptyName_ThrowsInvalidClientStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidClientStateException>(() => Client.Create(string.Empty, "contact@acme.com"));
        Assert.Contains("Client name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Client creation fails with whitespace name.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceName_ThrowsInvalidClientStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidClientStateException>(() => Client.Create("   ", "contact@acme.com"));
        Assert.Contains("Client name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Client creation fails with name exceeding max length.
    /// </summary>
    [Fact]
    public void Create_WithNameTooLong_ThrowsInvalidClientStateException()
    {
        // Arrange
        var longName = new string('a', 201);

        // Act & Assert
        var exception = Assert.Throws<InvalidClientStateException>(() => Client.Create(longName, "contact@acme.com"));
        Assert.Contains("Client name must not exceed 200 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Client creation trims whitespace from name.
    /// </summary>
    [Fact]
    public void Create_TrimsName()
    {
        // Act
        var client = Client.Create("  Acme Corp  ", "contact@acme.com");

        // Assert
        Assert.Equal("Acme Corp", client.Name);
    }

    /// <summary>
    /// Tests that Client can be updated with valid data.
    /// </summary>
    [Fact]
    public void Update_WithValidData_UpdatesClientAndRaisesEvent()
    {
        // Arrange
        var client = Client.Create("Acme Corp", "contact@acme.com");
        var originalCreatedAt = client.CreatedAt;

        // Act
        client.Update("Acme Updated", "updated@acme.com", TfsWebUrl.Create("https://acme.com/new-logo.png"), "+1987654321", "456 Avenue");

        // Assert
        Assert.Equal("Acme Updated", client.Name);
        Assert.Equal("updated@acme.com", client.Email);
        Assert.Equal("https://acme.com/new-logo.png", client.LogoUrl?.Value);
        Assert.Equal("+1987654321", client.Phone);
        Assert.Equal("456 Avenue", client.Address);
        Assert.NotNull(client.UpdatedAt);
        Assert.True(client.UpdatedAt > originalCreatedAt);
        Assert.Equal(originalCreatedAt, client.CreatedAt);

        // Verify domain event (last event should be ClientUpdatedEvent)
        var events = client.Events;
        Assert.Equal(2, events.Count); // ClientCreatedEvent + ClientUpdatedEvent
        var updatedEvent = Assert.IsType<ClientUpdatedEvent>(events[1]);
        Assert.Equal(client.Id, updatedEvent.ClientId);
    }

    /// <summary>
    /// Tests that Client update fails with empty name.
    /// </summary>
    [Fact]
    public void Update_WithEmptyName_ThrowsInvalidClientStateException()
    {
        // Arrange
        var client = Client.Create("Acme Corp", "contact@acme.com");

        // Act & Assert
        var exception = Assert.Throws<InvalidClientStateException>(() => client.Update(string.Empty, "contact@acme.com"));
        Assert.Contains("Client name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Client update fails with whitespace name.
    /// </summary>
    [Fact]
    public void Update_WithWhitespaceName_ThrowsInvalidClientStateException()
    {
        // Arrange
        var client = Client.Create("Acme Corp", "contact@acme.com");

        // Act & Assert
        var exception = Assert.Throws<InvalidClientStateException>(() => client.Update("   ", "contact@acme.com"));
        Assert.Contains("Client name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Client update fails with name exceeding max length.
    /// </summary>
    [Fact]
    public void Update_WithNameTooLong_ThrowsInvalidClientStateException()
    {
        // Arrange
        var client = Client.Create("Acme Corp", "contact@acme.com");
        var longName = new string('a', 201);

        // Act & Assert
        var exception = Assert.Throws<InvalidClientStateException>(() => client.Update(longName, "contact@acme.com"));
        Assert.Contains("Client name must not exceed 200 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Client update trims whitespace from name.
    /// </summary>
    [Fact]
    public void Update_TrimsName()
    {
        // Arrange
        var client = Client.Create("Acme Corp", "contact@acme.com");

        // Act
        client.Update("  Acme Updated  ", "contact@acme.com");

        // Assert
        Assert.Equal("Acme Updated", client.Name);
    }

    /// <summary>
    /// Tests that Client deactivation sets IsActive to false and raises event.
    /// </summary>
    [Fact]
    public void Deactivate_SetsIsActiveFalseAndRaisesEvent()
    {
        // Arrange
        var client = Client.Create("Acme Corp", "contact@acme.com");

        // Act
        client.Deactivate();

        // Assert
        Assert.False(client.IsActive);
        Assert.NotNull(client.UpdatedAt);

        // Verify domain event (last event should be ClientDeactivatedEvent)
        var events = client.Events;
        Assert.Equal(2, events.Count); // ClientCreatedEvent + ClientDeactivatedEvent
        var deactivatedEvent = Assert.IsType<ClientDeactivatedEvent>(events[1]);
        Assert.Equal(client.Id, deactivatedEvent.ClientId);
    }

    /// <summary>
    /// Tests that Client can be created without optional fields.
    /// </summary>
    [Fact]
    public void Create_WithoutOptionalFields_Succeeds()
    {
        // Act
        var client = Client.Create("Acme Corp", "contact@acme.com");

        // Assert
        Assert.Equal("Acme Corp", client.Name);
        Assert.Equal("contact@acme.com", client.Email);
        Assert.Null(client.LogoUrl);
        Assert.Null(client.Phone);
        Assert.Null(client.Address);
    }
}