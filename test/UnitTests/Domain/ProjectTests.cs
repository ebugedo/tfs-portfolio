// <copyright file="ProjectTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain;

using FluentAssertions;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Events;
using Tfs.Portfolio.Domain.Projects.Exceptions;
using Tfs.Portfolio.UnitTests.Common;
using Xunit;

/// <summary>
/// Unit tests for the Project aggregate root.
/// </summary>
public class ProjectTests
{
    private readonly ProjectFaker projectFaker = new();

    /// <summary>
    /// Tests that creating a project with a valid name raises a ProjectCreatedEvent.
    /// </summary>
    [Fact]
    public void Create_WithValidName_RaisesProjectCreatedEvent()
    {
        // Arrange
        var name = "Test Project";
        var description = "Test Description";
        var startDate = YearMonth.Create(6, 2024);
        var durationMonths = 6;
        var clientId = Guid.NewGuid();
        var sectorId = Guid.NewGuid();

        // Act
        var project = Project.Create(name, description, startDate, durationMonths, clientId, sectorId);

        // Assert
        project.Should().NotBeNull();
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
        project.StartDate.Should().Be(startDate);
        project.DurationMonths.Should().Be(durationMonths);
        project.EndDate.Should().Be(YearMonth.Create(12, 2024));
        project.ClientId.Should().Be(clientId);
        project.SectorId.Should().Be(sectorId);
        project.Status.Should().Be(ProjectStatus.Draft);
        project.Id.Should().NotBeEmpty();
        project.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        var events = project.Events;
        events.Should().HaveCount(1);
        events[0].Should().BeOfType<ProjectCreatedEvent>();
        var createdEvent = (ProjectCreatedEvent)events[0];
        createdEvent.ProjectId.Should().Be(project.Id);
        createdEvent.ProjectName.Should().Be(name);
    }

    /// <summary>
    /// Tests that creating a project with an empty name throws a DomainException.
    /// </summary>
    [Fact]
    public void Create_WithEmptyName_ThrowsDomainException()
    {
        // Act & Assert
        var act = () => Project.Create(string.Empty, null, YearMonth.Create(6, 2024), 6, Guid.NewGuid(), Guid.NewGuid());
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Project name cannot be empty");
    }

    /// <summary>
    /// Tests that creating a project with whitespace name throws a DomainException.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceName_ThrowsDomainException()
    {
        // Act & Assert
        var act = () => Project.Create("   ", null, YearMonth.Create(6, 2024), 6, Guid.NewGuid(), Guid.NewGuid());
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Project name cannot be empty");
    }

    /// <summary>
    /// Tests that creating a project with invalid duration throws a DomainException.
    /// </summary>
    [Fact]
    public void Create_WithInvalidDuration_ThrowsDomainException()
    {
        // Act & Assert
        var act = () => Project.Create("Test Project", null, YearMonth.Create(6, 2024), 0, Guid.NewGuid(), Guid.NewGuid());
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Duration must be between 1 and 120 months");

        var act2 = () => Project.Create("Test Project", null, YearMonth.Create(6, 2024), 121, Guid.NewGuid(), Guid.NewGuid());
        act2.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Duration must be between 1 and 120 months");
    }

    /// <summary>
    /// Tests that creating a project with empty client ID throws a DomainException.
    /// </summary>
    [Fact]
    public void Create_WithEmptyClientId_ThrowsDomainException()
    {
        // Act & Assert
        var act = () => Project.Create("Test Project", null, YearMonth.Create(6, 2024), 6, Guid.Empty, Guid.NewGuid());
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Client not found");
    }

    /// <summary>
    /// Tests that creating a project with empty sector ID throws a DomainException.
    /// </summary>
    [Fact]
    public void Create_WithEmptySectorId_ThrowsDomainException()
    {
        // Act & Assert
        var act = () => Project.Create("Test Project", null, YearMonth.Create(6, 2024), 6, Guid.NewGuid(), Guid.Empty);
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Sector not found");
    }

    /// <summary>
    /// Tests that updating a project with valid data works correctly.
    /// </summary>
    [Fact]
    public void Update_WithValidData_UpdatesProject()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        var originalId = project.Id;
        var originalCreatedAt = project.CreatedAt;
        var newName = "Updated Project";
        var newDescription = "Updated Description";
        var newStartDate = YearMonth.Create(7, 2024);
        var newDurationMonths = 8;
        var newClientId = Guid.NewGuid();
        var newSectorId = Guid.NewGuid();

        // Act
        project.Update(newName, newDescription, newStartDate, newDurationMonths, newClientId, newSectorId);

        // Assert
        project.Id.Should().Be(originalId);
        project.Name.Should().Be(newName);
        project.Description.Should().Be(newDescription);
        project.StartDate.Should().Be(newStartDate);
        project.DurationMonths.Should().Be(newDurationMonths);
        project.EndDate.Should().Be(YearMonth.Create(3, 2025));
        project.ClientId.Should().Be(newClientId);
        project.SectorId.Should().Be(newSectorId);
        project.CreatedAt.Should().Be(originalCreatedAt);
        project.UpdatedAt.Should().NotBeNull();
        project.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        var events = project.Events;
        events.Should().Contain(e => e is ProjectUpdatedEvent);
    }

    /// <summary>
    /// Tests that updating a project with an empty name throws a DomainException.
    /// </summary>
    [Fact]
    public void Update_WithEmptyName_ThrowsDomainException()
    {
        // Arrange
        var project = this.projectFaker.Generate();

        // Act & Assert
        var act = () => project.Update(string.Empty, "Description", YearMonth.Create(6, 2024), 6, Guid.NewGuid(), Guid.NewGuid());
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Project name cannot be empty");
    }

    /// <summary>
    /// Tests that changing project status with valid transition works.
    /// </summary>
    [Fact]
    public void ChangeStatus_ValidTransition_UpdatesStatusAndRaisesEvent()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        project.Status.Should().Be(ProjectStatus.Draft);

        // Act
        project.ChangeStatus(ProjectStatus.Active);

        // Assert
        project.Status.Should().Be(ProjectStatus.Active);
        project.UpdatedAt.Should().NotBeNull();

        var events = project.Events;
        events.Should().Contain(e => e is ProjectStatusChangedEvent);
        var statusEvent = (ProjectStatusChangedEvent)events.First(e => e is ProjectStatusChangedEvent);
        statusEvent.ProjectId.Should().Be(project.Id);
        statusEvent.OldStatus.Should().Be(ProjectStatus.Draft);
        statusEvent.NewStatus.Should().Be(ProjectStatus.Active);
    }

    /// <summary>
    /// Tests that changing project status with invalid transition throws exception.
    /// </summary>
    [Fact]
    public void ChangeStatus_InvalidTransition_ThrowsException()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        project.ChangeStatus(ProjectStatus.Active); // First make it Active

        // Act & Assert - Active to Draft is invalid
        var act = () => project.ChangeStatus(ProjectStatus.Draft);
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Invalid status transition from Active to Draft");
    }

    /// <summary>
    /// Tests that adding a technology works correctly.
    /// </summary>
    [Fact]
    public void AddTechnology_AddsTechnology()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        var technology = Technology.Create("React", TechnologyCategory.Frontend, ProficiencyLevel.Expert);

        // Act
        project.AddTechnology(technology);

        // Assert
        project.Technologies.Should().Contain(t => t.Equals(technology));
    }

    /// <summary>
    /// Tests that adding duplicate technology throws exception.
    /// </summary>
    [Fact]
    public void AddTechnology_Duplicate_ThrowsException()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        var technology = Technology.Create("React", TechnologyCategory.Frontend, ProficiencyLevel.Expert);
        project.AddTechnology(technology);

        // Act & Assert
        var act = () => project.AddTechnology(technology);
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Technology already exists in project");
    }

    /// <summary>
    /// Tests that removing a technology works correctly.
    /// </summary>
    [Fact]
    public void RemoveTechnology_RemovesTechnology()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        var technology = Technology.Create("React", TechnologyCategory.Frontend, ProficiencyLevel.Expert);
        project.AddTechnology(technology);

        // Act
        project.RemoveTechnology(technology);

        // Assert
        project.Technologies.Should().NotContain(t => t.Equals(technology));
    }

    /// <summary>
    /// Tests that adding a service works correctly.
    /// </summary>
    [Fact]
    public void AddService_AddsService()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        var serviceId = Guid.NewGuid();

        // Act
        project.AddService(serviceId);

        // Assert
        project.ServiceIds.Should().Contain(serviceId);
    }

    /// <summary>
    /// Tests that adding duplicate service throws exception.
    /// </summary>
    [Fact]
    public void AddService_Duplicate_ThrowsException()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        var serviceId = Guid.NewGuid();
        project.AddService(serviceId);

        // Act & Assert
        var act = () => project.AddService(serviceId);
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Service already associated with project");
    }

    /// <summary>
    /// Tests that removing a service works correctly.
    /// </summary>
    [Fact]
    public void RemoveService_RemovesService()
    {
        // Arrange
        var project = this.projectFaker.Generate();
        var serviceId = Guid.NewGuid();
        project.AddService(serviceId);

        // Act
        project.RemoveService(serviceId);

        // Assert
        project.ServiceIds.Should().NotContain(serviceId);
    }
}