// <copyright file="ProjectRepositoryTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Persistence;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Sectors.Entities;
using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.IntegrationTests;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the ProjectRepository.
/// </summary>
public sealed class ProjectRepositoryTests : IntegrationTestBase
{
    /// <summary>
    /// Creates a test client via repository and returns its ID.
    /// </summary>
    private new async Task<Guid> CreateTestClientAsync(string name = "Test Client", string email = "test@client.com")
    {
        var client = Tfs.Portfolio.Domain.Clients.Entities.Client.Create(name, email);
        await ClientRepository.AddAsync(client);
        await UnitOfWork.SaveChangesAsync();
        return client.Id;
    }

    /// <summary>
    /// Creates a test sector via repository and returns its ID.
    /// </summary>
    private new async Task<Guid> CreateTestSectorAsync(string name = "Technology", string description = "Test sector")
    {
        var sector = Tfs.Portfolio.Domain.Sectors.Entities.Sector.Create(name, description);
        await SectorRepository.AddAsync(sector);
        await UnitOfWork.SaveChangesAsync();
        return sector.Id;
    }

    /// <summary>
    /// Tests that AddAsync persists a project to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_PersistsProjectToDatabase()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var project = Project.Create(
            "Persistence Test",
            "Test Description",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId);

        // Act
        await ProjectRepository.AddAsync(project);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the project was persisted by retrieving it
        var retrieved = await ProjectRepository.GetByIdAsync(project.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(project.Id);
        retrieved.Name.Should().Be("Persistence Test");
        retrieved.Description.Should().Be("Test Description");
        retrieved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that GetByIdAsync returns the correct entity.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectEntity()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var project = Project.Create(
            "GetById Test",
            "Test Description",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId);
        await ProjectRepository.AddAsync(project);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var retrieved = await ProjectRepository.GetByIdAsync(project.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(project.Id);
        retrieved.Name.Should().Be("GetById Test");
        retrieved.Description.Should().Be("Test Description");
    }

    /// <summary>
    /// Tests that GetByIdAsync returns null for non-existent ID.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var result = await ProjectRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetAllAsync returns all projects.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllProjects()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var project1 = Project.Create(
            "Project 1",
            "Description 1",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId);
        var project2 = Project.Create(
            "Project 2",
            "Description 2",
            YearMonth.Create(7, 2024),
            8,
            clientId,
            sectorId);
        var project3 = Project.Create(
            "Project 3",
            "Description 3",
            YearMonth.Create(8, 2024),
            10,
            clientId,
            sectorId);

        await ProjectRepository.AddAsync(project1);
        await ProjectRepository.AddAsync(project2);
        await ProjectRepository.AddAsync(project3);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var projects = await ProjectRepository.GetAllAsync();

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCount(3);
        projects.Should().Contain(p => p.Name == "Project 1");
        projects.Should().Contain(p => p.Name == "Project 2");
        projects.Should().Contain(p => p.Name == "Project 3");
    }

    /// <summary>
    /// Tests that Update updates an existing project.
    /// </summary>
    [Fact]
    public async Task Update_UpdatesExistingProject()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var project = Project.Create(
            "Original Name",
            "Original Description",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId);
        await ProjectRepository.AddAsync(project);
        await UnitOfWork.SaveChangesAsync();

        // Detach to simulate new request scope (production uses scoped DbContext per request)
        this.DbContext.Entry(project).State = EntityState.Detached;

        // Act - Get tracked entity, modify it, save
        var trackedProject = await ProjectRepository.GetByIdAsync(project.Id);
        trackedProject.Should().NotBeNull();
        trackedProject!.Update(
            "Updated Name",
            "Updated Description",
            YearMonth.Create(7, 2024),
            8,
            clientId,
            sectorId);
        await UnitOfWork.SaveChangesAsync();

        // Assert - read from clean context to verify persisted state
        var retrieved = await this.DbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == project.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Updated Name");
        retrieved.Description.Should().Be("Updated Description");
        retrieved.UpdatedAt.Should().NotBeNull();
        retrieved.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that UnitOfWork commits the transaction.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_CommitsTransaction()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var project = Project.Create(
            "Transaction Test",
            "Test Description",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId);
        await ProjectRepository.AddAsync(project);

        // Act
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert
        rowsAffected.Should().Be(1);

        // Verify the project was actually persisted
        var retrieved = await ProjectRepository.GetByIdAsync(project.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Transaction Test");
    }

    /// <summary>
    /// Tests that multiple operations in a single transaction are atomic.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_MultipleOperations_AreAtomic()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();
        
        var project1 = Project.Create(
            "Atomic Test 1",
            "Description 1",
            YearMonth.Create(6, 2024),
            6,
            clientId,
            sectorId);
        var project2 = Project.Create(
            "Atomic Test 2",
            "Description 2",
            YearMonth.Create(7, 2024),
            8,
            clientId,
            sectorId);

        // Act - add both but only save once
        await ProjectRepository.AddAsync(project1);
        await ProjectRepository.AddAsync(project2);
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert - both should be saved in a single transaction
        rowsAffected.Should().Be(2);

        var allProjects = await ProjectRepository.GetAllAsync();
        allProjects.Should().HaveCount(2);
    }
}