// <copyright file="ClientRepositoryTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Persistence;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Clients.Repositories;
using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.IntegrationTests;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the ClientRepository.
/// </summary>
public sealed class ClientRepositoryTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that AddAsync persists a client to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_PersistsClientToDatabase()
    {
        // Arrange - use fully qualified name to avoid Client property collision
        var client = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Test Client", "test@client.com", "https://client.com/logo.png", "+1234567890", "123 Client St");

        // Act
        await ClientRepository.AddAsync(client);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the client was persisted by retrieving it
        var retrieved = await ClientRepository.GetByIdAsync(client.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(client.Id);
        retrieved.Name.Should().Be("Test Client");
        retrieved.Email.Should().Be("test@client.com");
        retrieved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that GetByIdAsync returns the correct entity.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectEntity()
    {
        // Arrange
        var client = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("GetById Test", "getbyid@client.com", null, null, null);
        await ClientRepository.AddAsync(client);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var retrieved = await ClientRepository.GetByIdAsync(client.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(client.Id);
        retrieved.Name.Should().Be("GetById Test");
        retrieved.Email.Should().Be("getbyid@client.com");
    }

    /// <summary>
    /// Tests that GetByIdAsync returns null for non-existent ID.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var result = await ClientRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetAllAsync returns all clients.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllClients()
    {
        // Arrange
        var client1 = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Client 1", "client1@test.com", null, null, null);
        var client2 = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Client 2", "client2@test.com", null, null, null);
        var client3 = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Client 3", "client3@test.com", null, null, null);

        await ClientRepository.AddAsync(client1);
        await ClientRepository.AddAsync(client2);
        await ClientRepository.AddAsync(client3);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var clients = await ClientRepository.GetAllAsync();

        // Assert
        clients.Should().NotBeNull();
        clients.Should().HaveCount(3);
        clients.Should().Contain(c => c.Name == "Client 1");
        clients.Should().Contain(c => c.Name == "Client 2");
        clients.Should().Contain(c => c.Name == "Client 3");
    }

    /// <summary>
    /// Tests that GetActiveAsync returns all active clients.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_ReturnsActiveClients()
    {
        // Arrange
        var activeClient = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Active Client", "active@test.com", null, null, null);
        var inactiveClient = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Inactive Client", "inactive@test.com", null, null, null);
        inactiveClient.Deactivate();

        await ClientRepository.AddAsync(activeClient);
        await ClientRepository.AddAsync(inactiveClient);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var activeClients = await ClientRepository.GetActiveAsync();

        // Assert
        activeClients.Should().NotBeNull();
        activeClients.Should().HaveCount(1);
        activeClients.Should().Contain(c => c.Name == "Active Client");
        activeClients.Should().NotContain(c => c.Name == "Inactive Client");
    }

    /// <summary>
    /// Tests that Update updates an existing client.
    /// </summary>
    [Fact]
    public async Task Update_UpdatesExistingClient()
    {
        // Arrange
        var client = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Original Name", "original@test.com", null, null, null);
        await ClientRepository.AddAsync(client);
        await UnitOfWork.SaveChangesAsync();

        // Detach to simulate new request scope (production uses scoped DbContext per request)
        DbContext.Entry(client).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Act - Get tracked entity, modify it, save
        var trackedClient = await ClientRepository.GetByIdAsync(client.Id);
        trackedClient.Should().NotBeNull();
        trackedClient!.Update("Updated Name", "updated@test.com", "https://updated.com/logo.png", "+1112223333", "789 Updated Blvd");
        await UnitOfWork.SaveChangesAsync();

        // Assert - read from clean context to verify persisted state
        var retrieved = await DbContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == client.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Updated Name");
        retrieved.Email.Should().Be("updated@test.com");
        var actualLogoUrl = retrieved.LogoUrl!.Value.Value;
        actualLogoUrl.Should().Be("https://updated.com/logo.png");
        retrieved.Phone.Should().Be("+1112223333");
        retrieved.Address.Should().Be("789 Updated Blvd");
        retrieved.UpdatedAt.Should().NotBeNull();
        retrieved.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that Delete removes a client.
    /// </summary>
    [Fact]
    public async Task Delete_RemovesClient()
    {
        // Arrange
        var client = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Delete Test", "delete@test.com", null, null, null);
        await ClientRepository.AddAsync(client);
        await UnitOfWork.SaveChangesAsync();

        // Act
        ClientRepository.Delete(client);
        await UnitOfWork.SaveChangesAsync();

        // Assert
        var retrieved = await ClientRepository.GetByIdAsync(client.Id);
        retrieved.Should().BeNull();
    }

    /// <summary>
    /// Tests that UnitOfWork commits the transaction.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_CommitsTransaction()
    {
        // Arrange
        var client = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Transaction Test", "transaction@test.com", null, null, null);
        await ClientRepository.AddAsync(client);

        // Act
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert
        rowsAffected.Should().Be(1);

        // Verify the client was actually persisted
        var retrieved = await ClientRepository.GetByIdAsync(client.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Transaction Test");
    }

    /// <summary>
    /// Tests that multiple operations in a single transaction are atomic.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_MultipleOperations_AreAtomic()
    {
        // Arrange
        var client1 = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Atomic Test 1", "atomic1@test.com", null, null, null);
        var client2 = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Atomic Test 2", "atomic2@test.com", null, null, null);

        // Act - add both but only save once
        await ClientRepository.AddAsync(client1);
        await ClientRepository.AddAsync(client2);
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert - both should be saved in a single transaction
        rowsAffected.Should().Be(2);

        var allClients = await ClientRepository.GetAllAsync();
        allClients.Should().HaveCount(2);
    }

    /// <summary>
    /// Tests that Client with non-null LogoUrl persists and loads correctly.
    /// </summary>
    [Fact]
    public async Task AddAsync_WithNonNullLogoUrl_PersistsAndLoadsCorrectly()
    {
        // Arrange
        var logoUrl = "https://example.com/logo.png";
        var client = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Logo Test", "logo@test.com", logoUrl, "+1234567890", "123 Logo St");

        // Act
        await ClientRepository.AddAsync(client);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the client was persisted and LogoUrl is correct
        var retrieved = await ClientRepository.GetByIdAsync(client.Id);
        retrieved.Should().NotBeNull();
        retrieved!.LogoUrl.Should().NotBeNull();
        retrieved.LogoUrl!.Value.IsValid.Should().BeTrue();
        retrieved.LogoUrl.Value.Value.Should().Be(logoUrl);
    }

    /// <summary>
    /// Tests that Client with null LogoUrl persists and loads correctly.
    /// </summary>
    [Fact]
    public async Task AddAsync_WithNullLogoUrl_PersistsAndLoadsCorrectly()
    {
        // Arrange
        var client = Tfs.Portfolio.Domain.Clients.Entities.Client.Create("Null Logo Test", "nulllogo@test.com", null, "+1234567890", "123 Null St");

        // Act
        await ClientRepository.AddAsync(client);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the client was persisted and LogoUrl is null
        var retrieved = await ClientRepository.GetByIdAsync(client.Id);
        retrieved.Should().NotBeNull();
        retrieved!.LogoUrl.Should().BeNull();
    }
}