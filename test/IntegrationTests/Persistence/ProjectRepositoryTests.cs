// <copyright file="ProjectRepositoryTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Persistence;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.IntegrationTests;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the ProjectRepository.
/// </summary>
public sealed class ProjectRepositoryTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that AddAsync persists a project to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_PersistsProjectToDatabase()
    {
        // Arrange
        var project = Project.Create("Persistence Test", "Test Description");

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
        // Arrange
        var project = Project.Create("GetById Test", "Test Description");
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
        // Arrange
        var project1 = Project.Create("Project 1", "Description 1");
        var project2 = Project.Create("Project 2", "Description 2");
        var project3 = Project.Create("Project 3", "Description 3");

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
        // Arrange
        var project = Project.Create("Original Name", "Original Description");
        await ProjectRepository.AddAsync(project);
        await UnitOfWork.SaveChangesAsync();

        // Detach to simulate new request scope (production uses scoped DbContext per request)
        this.DbContext.Entry(project).State = EntityState.Detached;

        // Act - Get tracked entity, modify it, save
        var trackedProject = await ProjectRepository.GetByIdAsync(project.Id);
        trackedProject.Should().NotBeNull();
        trackedProject!.Update("Updated Name", "Updated Description");
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
        // Arrange
        var project = Project.Create("Transaction Test", "Test Description");
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
        // Arrange
        var project1 = Project.Create("Atomic Test 1", "Description 1");
        var project2 = Project.Create("Atomic Test 2", "Description 2");

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
