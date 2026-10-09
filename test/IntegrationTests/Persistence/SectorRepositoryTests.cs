// <copyright file="SectorRepositoryTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Persistence;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Sectors.Entities;
using Tfs.Portfolio.Domain.Sectors.Repositories;
using Tfs.Portfolio.IntegrationTests;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the SectorRepository.
/// </summary>
public sealed class SectorRepositoryTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that AddAsync persists a sector to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_PersistsSectorToDatabase()
    {
        // Arrange
        var sector = Sector.Create("Technology", "Technology sector for software projects");

        // Act
        await SectorRepository.AddAsync(sector);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the sector was persisted by retrieving it
        var retrieved = await SectorRepository.GetByIdAsync(sector.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(sector.Id);
        retrieved.Name.Should().Be("Technology");
        retrieved.Description.Should().Be("Technology sector for software projects");
        retrieved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that GetByIdAsync returns the correct entity.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectEntity()
    {
        // Arrange
        var sector = Sector.Create("GetById Sector", "Sector for get by ID test");
        await SectorRepository.AddAsync(sector);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var retrieved = await SectorRepository.GetByIdAsync(sector.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(sector.Id);
        retrieved.Name.Should().Be("GetById Sector");
        retrieved.Description.Should().Be("Sector for get by ID test");
    }

    /// <summary>
    /// Tests that GetByIdAsync returns null for non-existent ID.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var result = await SectorRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetAllAsync returns all sectors.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllSectors()
    {
        // Arrange
        var sector1 = Sector.Create("Sector 1", "First sector");
        var sector2 = Sector.Create("Sector 2", "Second sector");
        var sector3 = Sector.Create("Sector 3", "Third sector");

        await SectorRepository.AddAsync(sector1);
        await SectorRepository.AddAsync(sector2);
        await SectorRepository.AddAsync(sector3);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var sectors = await SectorRepository.GetAllAsync();

        // Assert
        sectors.Should().NotBeNull();
        sectors.Should().HaveCount(3);
        sectors.Should().Contain(s => s.Name == "Sector 1");
        sectors.Should().Contain(s => s.Name == "Sector 2");
        sectors.Should().Contain(s => s.Name == "Sector 3");
    }

    /// <summary>
    /// Tests that GetActiveAsync returns all active sectors.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_ReturnsActiveSectors()
    {
        // Arrange - by default sectors are active on creation
        var activeSector1 = Sector.Create("Active Sector 1", "First active sector");
        var activeSector2 = Sector.Create("Active Sector 2", "Second active sector");

        await SectorRepository.AddAsync(activeSector1);
        await SectorRepository.AddAsync(activeSector2);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var activeSectors = await SectorRepository.GetActiveAsync();

        // Assert
        activeSectors.Should().NotBeNull();
        activeSectors.Should().HaveCount(2);
        activeSectors.Should().Contain(s => s.Name == "Active Sector 1");
        activeSectors.Should().Contain(s => s.Name == "Active Sector 2");
    }

    /// <summary>
    /// Tests that Update updates an existing sector.
    /// </summary>
    [Fact]
    public async Task Update_UpdatesExistingSector()
    {
        // Arrange
        var sector = Sector.Create("Original Name", "Original description");
        await SectorRepository.AddAsync(sector);
        await UnitOfWork.SaveChangesAsync();

        // Detach to simulate new request scope
        DbContext.Entry(sector).State = EntityState.Detached;

        // Act - Get tracked entity, modify it, save
        var trackedSector = await SectorRepository.GetByIdAsync(sector.Id);
        trackedSector.Should().NotBeNull();
        trackedSector!.Update("Updated Name", "Updated description");
        await UnitOfWork.SaveChangesAsync();

        // Assert - read from clean context to verify persisted state
        var retrieved = await DbContext.Sectors
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sector.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Updated Name");
        retrieved.Description.Should().Be("Updated description");
    }

    /// <summary>
    /// Tests that Delete removes a sector.
    /// </summary>
    [Fact]
    public async Task Delete_RemovesSector()
    {
        // Arrange
        var sector = Sector.Create("Delete Test", "Sector to be deleted");
        await SectorRepository.AddAsync(sector);
        await UnitOfWork.SaveChangesAsync();

        // Act
        SectorRepository.Delete(sector);
        await UnitOfWork.SaveChangesAsync();

        // Assert
        var retrieved = await SectorRepository.GetByIdAsync(sector.Id);
        retrieved.Should().BeNull();
    }

    /// <summary>
    /// Tests that UnitOfWork commits the transaction.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_CommitsTransaction()
    {
        // Arrange
        var sector = Sector.Create("Transaction Test", "Test description");
        await SectorRepository.AddAsync(sector);

        // Act
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert
        rowsAffected.Should().Be(1);

        // Verify the sector was actually persisted
        var retrieved = await SectorRepository.GetByIdAsync(sector.Id);
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
        var sector1 = Sector.Create("Atomic Sector 1", "Description 1");
        var sector2 = Sector.Create("Atomic Sector 2", "Description 2");

        // Act - add both but only save once
        await SectorRepository.AddAsync(sector1);
        await SectorRepository.AddAsync(sector2);
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert - both should be saved in a single transaction
        rowsAffected.Should().Be(2);

        var allSectors = await SectorRepository.GetAllAsync();
        allSectors.Should().HaveCount(2);
    }
}