// <copyright file="ProjectTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain;

using FluentAssertions;
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

        // Act
        var project = Project.Create(name, description);

        // Assert
        project.Should().NotBeNull();
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
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
        var act = () => Project.Create(string.Empty);
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
        var act = () => Project.Create("   ");
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Project name cannot be empty");
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

        // Act
        project.Update(newName, newDescription);

        // Assert
        project.Id.Should().Be(originalId);
        project.Name.Should().Be(newName);
        project.Description.Should().Be(newDescription);
        project.CreatedAt.Should().Be(originalCreatedAt);
        project.UpdatedAt.Should().NotBeNull();
        project.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
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
        var act = () => project.Update(string.Empty, "Description");
        act.Should().Throw<InvalidProjectStateException>()
            .WithMessage("Project name cannot be empty");
    }
}
