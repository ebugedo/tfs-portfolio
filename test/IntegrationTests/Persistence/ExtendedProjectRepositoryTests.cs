// <copyright file="ExtendedProjectRepositoryTests.cs" company="Tfs.Portfolio">
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
/// Integration tests for the extended ProjectRepository query methods.
/// </summary>
public sealed class ExtendedProjectRepositoryTests : IntegrationTestBase
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
    /// Tests that GetByClientIdAsync returns projects for a specific client.
    /// </summary>
    [Fact]
    public async Task GetByClientIdAsync_ReturnsProjectsForClient()
    {
        // Arrange - create clients and sector via repositories
        var client1Id = await CreateTestClientAsync("Client 1", "client1@test.com");
        var client2Id = await CreateTestClientAsync("Client 2", "client2@test.com");
        var sectorId = await CreateTestSectorAsync("Technology");

        var project1 = Project.Create("Client1 Project 1", "Desc 1", YearMonth.Create(6, 2024), 6, client1Id, sectorId);
        var project2 = Project.Create("Client1 Project 2", "Desc 2", YearMonth.Create(7, 2024), 8, client1Id, sectorId);
        var project3 = Project.Create("Client2 Project 1", "Desc 3", YearMonth.Create(8, 2024), 10, client2Id, sectorId);

        await ProjectRepository.AddAsync(project1);
        await ProjectRepository.AddAsync(project2);
        await ProjectRepository.AddAsync(project3);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by client1Id
        var client1Projects = await ProjectRepository.GetByClientIdAsync(client1Id);

        // Assert
        client1Projects.Should().NotBeNull();
        client1Projects.Should().HaveCount(2);
        client1Projects.Should().Contain(p => p.Name == "Client1 Project 1");
        client1Projects.Should().Contain(p => p.Name == "Client1 Project 2");
        client1Projects.Should().NotContain(p => p.Name == "Client2 Project 1");
        client1Projects.Should().AllSatisfy(p => p.ClientId.Should().Be(client1Id));
    }

    /// <summary>
    /// Tests that GetByClientIdAsync returns empty list when client has no projects.
    /// </summary>
    [Fact]
    public async Task GetByClientIdAsync_WhenNoProjects_ReturnsEmptyList()
    {
        // Act
        var projects = await ProjectRepository.GetByClientIdAsync(Guid.NewGuid());

        // Assert
        projects.Should().NotBeNull();
        projects.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetBySectorIdAsync returns projects for a specific sector.
    /// </summary>
    [Fact]
    public async Task GetBySectorIdAsync_ReturnsProjectsForSector()
    {
        // Arrange
        var sector1Id = await CreateTestSectorAsync("Technology");
        var sector2Id = await CreateTestSectorAsync("Finance");
        var clientId = await CreateTestClientAsync();

        var project1 = Project.Create("Sector1 Project 1", "Desc 1", YearMonth.Create(6, 2024), 6, clientId, sector1Id);
        var project2 = Project.Create("Sector1 Project 2", "Desc 2", YearMonth.Create(7, 2024), 8, clientId, sector1Id);
        var project3 = Project.Create("Sector2 Project 1", "Desc 3", YearMonth.Create(8, 2024), 10, clientId, sector2Id);

        await ProjectRepository.AddAsync(project1);
        await ProjectRepository.AddAsync(project2);
        await ProjectRepository.AddAsync(project3);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by sector1Id
        var sector1Projects = await ProjectRepository.GetBySectorIdAsync(sector1Id);

        // Assert
        sector1Projects.Should().NotBeNull();
        sector1Projects.Should().HaveCount(2);
        sector1Projects.Should().Contain(p => p.Name == "Sector1 Project 1");
        sector1Projects.Should().Contain(p => p.Name == "Sector1 Project 2");
        sector1Projects.Should().NotContain(p => p.Name == "Sector2 Project 1");
        sector1Projects.Should().AllSatisfy(p => p.SectorId.Should().Be(sector1Id));
    }

    /// <summary>
    /// Tests that GetBySectorIdAsync returns empty list when sector has no projects.
    /// </summary>
    [Fact]
    public async Task GetBySectorIdAsync_WhenNoProjects_ReturnsEmptyList()
    {
        // Act
        var projects = await ProjectRepository.GetBySectorIdAsync(Guid.NewGuid());

        // Assert
        projects.Should().NotBeNull();
        projects.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetByTechnologyAsync returns projects containing a specific technology.
    /// </summary>
    [Fact]
    public async Task GetByTechnologyAsync_ReturnsProjectsWithTechnology()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        var project1 = Project.Create("C# Project 1", "Desc 1", YearMonth.Create(6, 2024), 6, clientId, sectorId);
        project1.AddTechnology(Technology.Create("C#", TechnologyCategory.Backend, ProficiencyLevel.Expert));
        project1.AddTechnology(Technology.Create(".NET", TechnologyCategory.Backend, ProficiencyLevel.Expert));

        var project2 = Project.Create("Java Project", "Desc 2", YearMonth.Create(7, 2024), 8, clientId, sectorId);
        project2.AddTechnology(Technology.Create("Java", TechnologyCategory.Backend, ProficiencyLevel.Intermediate));
        project2.AddTechnology(Technology.Create("Spring", TechnologyCategory.Backend, ProficiencyLevel.Intermediate));

        var project3 = Project.Create("C# Project 2", "Desc 3", YearMonth.Create(8, 2024), 10, clientId, sectorId);
        project3.AddTechnology(Technology.Create("C#", TechnologyCategory.Backend, ProficiencyLevel.Expert));
        project3.AddTechnology(Technology.Create("ASP.NET Core", TechnologyCategory.Backend, ProficiencyLevel.Expert));

        await ProjectRepository.AddAsync(project1);
        await ProjectRepository.AddAsync(project2);
        await ProjectRepository.AddAsync(project3);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by technology "C#"
        var csharpProjects = await ProjectRepository.GetByTechnologyAsync("C#");

        // Assert
        csharpProjects.Should().NotBeNull();
        csharpProjects.Should().HaveCount(2);
        csharpProjects.Should().Contain(p => p.Name == "C# Project 1");
        csharpProjects.Should().Contain(p => p.Name == "C# Project 2");
        csharpProjects.Should().NotContain(p => p.Name == "Java Project");
    }

    /// <summary>
    /// Tests that GetByTechnologyAsync is case-insensitive.
    /// </summary>
    [Fact]
    public async Task GetByTechnologyAsync_IsCaseInsensitive()
    {
        // Arrange
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        var project = Project.Create("Test Project", "Desc", YearMonth.Create(6, 2024), 6, clientId, sectorId);
        project.AddTechnology(Technology.Create("C#", TechnologyCategory.Backend, ProficiencyLevel.Expert));

        await ProjectRepository.AddAsync(project);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by lowercase "c#"
        var projects = await ProjectRepository.GetByTechnologyAsync("c#");

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCount(1);
        projects.Should().Contain(p => p.Name == "Test Project");
    }

    /// <summary>
    /// Tests that GetByTechnologyAsync returns empty list when no projects have the technology.
    /// </summary>
    [Fact]
    public async Task GetByTechnologyAsync_WhenNoMatch_ReturnsEmptyList()
    {
        // Arrange
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        var project = Project.Create("Test Project", "Desc", YearMonth.Create(6, 2024), 6, clientId, sectorId);
        project.AddTechnology(Technology.Create("C#", TechnologyCategory.Backend, ProficiencyLevel.Expert));

        await ProjectRepository.AddAsync(project);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by non-existent technology
        var projects = await ProjectRepository.GetByTechnologyAsync("NonExistent");

        // Assert
        projects.Should().NotBeNull();
        projects.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetByStatusAsync returns projects with a specific status.
    /// </summary>
    [Fact]
    public async Task GetByStatusAsync_ReturnsProjectsWithStatus()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        var draftProject = Project.Create("Draft Project", "Desc 1", YearMonth.Create(6, 2024), 6, clientId, sectorId);
        var activeProject = Project.Create("Active Project", "Desc 2", YearMonth.Create(7, 2024), 8, clientId, sectorId);
        activeProject.ChangeStatus(ProjectStatus.Active);
        var onHoldProject = Project.Create("OnHold Project", "Desc 3", YearMonth.Create(8, 2024), 10, clientId, sectorId);
        // Must go through Active first: Draft -> Active -> OnHold
        onHoldProject.ChangeStatus(ProjectStatus.Active);
        onHoldProject.ChangeStatus(ProjectStatus.OnHold);

        await ProjectRepository.AddAsync(draftProject);
        await ProjectRepository.AddAsync(activeProject);
        await ProjectRepository.AddAsync(onHoldProject);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by Active status
        var activeProjects = await ProjectRepository.GetByStatusAsync(ProjectStatus.Active);

        // Assert
        activeProjects.Should().NotBeNull();
        activeProjects.Should().HaveCount(1);
        activeProjects.Should().Contain(p => p.Name == "Active Project");
        activeProjects.Should().NotContain(p => p.Name == "Draft Project");
        activeProjects.Should().NotContain(p => p.Name == "OnHold Project");
    }

    /// <summary>
    /// Tests that GetByStatusAsync returns empty list when no projects have the status.
    /// </summary>
    [Fact]
    public async Task GetByStatusAsync_WhenNoMatch_ReturnsEmptyList()
    {
        // Arrange
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        var project = Project.Create("Draft Project", "Desc", YearMonth.Create(6, 2024), 6, clientId, sectorId);

        await ProjectRepository.AddAsync(project);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by Active status (no active projects)
        var activeProjects = await ProjectRepository.GetByStatusAsync(ProjectStatus.Active);

        // Assert
        activeProjects.Should().NotBeNull();
        activeProjects.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetActiveAsync returns only active projects.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActiveProjects()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        var draftProject = Project.Create("Draft Project", "Desc 1", YearMonth.Create(6, 2024), 6, clientId, sectorId);
        var activeProject = Project.Create("Active Project", "Desc 2", YearMonth.Create(7, 2024), 8, clientId, sectorId);
        activeProject.ChangeStatus(ProjectStatus.Active);
        var completedProject = Project.Create("Completed Project", "Desc 3", YearMonth.Create(8, 2024), 10, clientId, sectorId);
        // Must go through Active first: Draft -> Active -> Completed
        completedProject.ChangeStatus(ProjectStatus.Active);
        completedProject.ChangeStatus(ProjectStatus.Completed);

        await ProjectRepository.AddAsync(draftProject);
        await ProjectRepository.AddAsync(activeProject);
        await ProjectRepository.AddAsync(completedProject);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var activeProjects = await ProjectRepository.GetActiveAsync();

        // Assert
        activeProjects.Should().NotBeNull();
        activeProjects.Should().HaveCount(1);
        activeProjects.Should().Contain(p => p.Name == "Active Project");
        activeProjects.Should().NotContain(p => p.Name == "Draft Project");
        activeProjects.Should().NotContain(p => p.Name == "Completed Project");
    }

    /// <summary>
    /// Tests that GetFilteredAsync with clientId filter returns filtered projects.
    /// </summary>
    [Fact]
    public async Task GetFilteredAsync_WithClientIdFilter_ReturnsFilteredProjects()
    {
        // Arrange - create clients and sector via repositories
        var client1Id = await CreateTestClientAsync("Client 1", "client1@test.com");
        var client2Id = await CreateTestClientAsync("Client 2", "client2@test.com");
        var sectorId = await CreateTestSectorAsync("Technology");

        await ProjectRepository.AddAsync(Project.Create("Client1 Project 1", "Desc", YearMonth.Create(6, 2024), 6, client1Id, sectorId));
        await ProjectRepository.AddAsync(Project.Create("Client1 Project 2", "Desc", YearMonth.Create(7, 2024), 8, client1Id, sectorId));
        await ProjectRepository.AddAsync(Project.Create("Client2 Project 1", "Desc", YearMonth.Create(8, 2024), 10, client2Id, sectorId));
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by client1Id
        var projects = await ProjectRepository.GetFilteredAsync(clientId: client1Id);

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCount(2);
        projects.Should().AllSatisfy(p => p.ClientId.Should().Be(client1Id));
    }

    /// <summary>
    /// Tests that GetFilteredAsync with sectorId filter returns filtered projects.
    /// </summary>
    [Fact]
    public async Task GetFilteredAsync_WithSectorIdFilter_ReturnsFilteredProjects()
    {
        // Arrange - create sectors and client via repositories
        var sector1Id = await CreateTestSectorAsync("Technology");
        var sector2Id = await CreateTestSectorAsync("Finance");
        var clientId = await CreateTestClientAsync();

        await ProjectRepository.AddAsync(Project.Create("Sector1 Project 1", "Desc", YearMonth.Create(6, 2024), 6, clientId, sector1Id));
        await ProjectRepository.AddAsync(Project.Create("Sector1 Project 2", "Desc", YearMonth.Create(7, 2024), 8, clientId, sector1Id));
        await ProjectRepository.AddAsync(Project.Create("Sector2 Project 1", "Desc", YearMonth.Create(8, 2024), 10, clientId, sector2Id));
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by sector1Id
        var projects = await ProjectRepository.GetFilteredAsync(sectorId: sector1Id);

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCount(2);
        projects.Should().AllSatisfy(p => p.SectorId.Should().Be(sector1Id));
    }

    /// <summary>
    /// Tests that GetFilteredAsync with technology filter returns filtered projects.
    /// </summary>
    [Fact]
    public async Task GetFilteredAsync_WithTechnologyFilter_ReturnsFilteredProjects()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        var project1 = Project.Create("C# Project", "Desc", YearMonth.Create(6, 2024), 6, clientId, sectorId);
        project1.AddTechnology(Technology.Create("C#", TechnologyCategory.Backend, ProficiencyLevel.Expert));

        var project2 = Project.Create("Java Project", "Desc", YearMonth.Create(7, 2024), 8, clientId, sectorId);
        project2.AddTechnology(Technology.Create("Java", TechnologyCategory.Backend, ProficiencyLevel.Intermediate));

        await ProjectRepository.AddAsync(project1);
        await ProjectRepository.AddAsync(project2);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by technology "C#"
        var projects = await ProjectRepository.GetFilteredAsync(technologyName: "C#");

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCount(1);
        projects.Should().Contain(p => p.Name == "C# Project");
    }

    /// <summary>
    /// Tests that GetFilteredAsync with status filter returns filtered projects.
    /// </summary>
    [Fact]
    public async Task GetFilteredAsync_WithStatusFilter_ReturnsFilteredProjects()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        var draftProject = Project.Create("Draft Project", "Desc", YearMonth.Create(6, 2024), 6, clientId, sectorId);
        var activeProject = Project.Create("Active Project", "Desc", YearMonth.Create(7, 2024), 8, clientId, sectorId);
        activeProject.ChangeStatus(ProjectStatus.Active);

        await ProjectRepository.AddAsync(draftProject);
        await ProjectRepository.AddAsync(activeProject);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by Active status
        var projects = await ProjectRepository.GetFilteredAsync(status: ProjectStatus.Active);

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCount(1);
        projects.Should().Contain(p => p.Name == "Active Project");
    }

    /// <summary>
    /// Tests that GetFilteredAsync with multiple filters returns projects matching all filters.
    /// </summary>
    [Fact]
    public async Task GetFilteredAsync_WithMultipleFilters_ReturnsProjectsMatchingAllFilters()
    {
        // Arrange - create clients and sector via repositories
        var client1Id = await CreateTestClientAsync("Client 1", "client1@test.com");
        var client2Id = await CreateTestClientAsync("Client 2", "client2@test.com");
        var sectorId = await CreateTestSectorAsync("Technology");

        // Client1, Draft, C#
        var project1 = Project.Create("Client1 Draft C#", "Desc", YearMonth.Create(6, 2024), 6, client1Id, sectorId);
        project1.AddTechnology(Technology.Create("C#", TechnologyCategory.Backend, ProficiencyLevel.Expert));

        // Client1, Active, C#
        var project2 = Project.Create("Client1 Active C#", "Desc", YearMonth.Create(7, 2024), 8, client1Id, sectorId);
        project2.AddTechnology(Technology.Create("C#", TechnologyCategory.Backend, ProficiencyLevel.Expert));
        project2.ChangeStatus(ProjectStatus.Active);

        // Client1, Active, Java
        var project3 = Project.Create("Client1 Active Java", "Desc", YearMonth.Create(8, 2024), 10, client1Id, sectorId);
        project3.AddTechnology(Technology.Create("Java", TechnologyCategory.Backend, ProficiencyLevel.Intermediate));
        project3.ChangeStatus(ProjectStatus.Active);

        // Client2, Active, C#
        var project4 = Project.Create("Client2 Active C#", "Desc", YearMonth.Create(9, 2024), 12, client2Id, sectorId);
        project4.AddTechnology(Technology.Create("C#", TechnologyCategory.Backend, ProficiencyLevel.Expert));
        project4.ChangeStatus(ProjectStatus.Active);

        await ProjectRepository.AddAsync(project1);
        await ProjectRepository.AddAsync(project2);
        await ProjectRepository.AddAsync(project3);
        await ProjectRepository.AddAsync(project4);
        await UnitOfWork.SaveChangesAsync();

        // Act - filter by client1Id, C# technology, and Active status
        var projects = await ProjectRepository.GetFilteredAsync(
            clientId: client1Id,
            technologyName: "C#",
            status: ProjectStatus.Active);

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCount(1);
        projects.Should().Contain(p => p.Name == "Client1 Active C#");
    }

    /// <summary>
    /// Tests that GetFilteredAsync with no filters returns all projects.
    /// </summary>
    [Fact]
    public async Task GetFilteredAsync_WithNoFilters_ReturnsAllProjects()
    {
        // Arrange - create client and sector via repositories
        var clientId = await CreateTestClientAsync();
        var sectorId = await CreateTestSectorAsync();

        await ProjectRepository.AddAsync(Project.Create("Project 1", "Desc", YearMonth.Create(6, 2024), 6, clientId, sectorId));
        await ProjectRepository.AddAsync(Project.Create("Project 2", "Desc", YearMonth.Create(7, 2024), 8, clientId, sectorId));
        await ProjectRepository.AddAsync(Project.Create("Project 3", "Desc", YearMonth.Create(8, 2024), 10, clientId, sectorId));
        await UnitOfWork.SaveChangesAsync();

        // Act - no filters
        var projects = await ProjectRepository.GetFilteredAsync();

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCount(3);
    }
}