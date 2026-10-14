// <copyright file="ProjectCommandHandlerTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Application.Projects.Commands;

using AutoMapper;
using FluentAssertions;
using Moq;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Projects.Handlers;
using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.UnitTests.Common;
using Xunit;

/// <summary>
/// Unit tests for the CreateProjectCommandHandler.
/// </summary>
public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<IUnitOfWork> unitOfWorkMock = new();
    private readonly Mock<IMapper> mapperMock = new();
    private readonly CreateProjectCommandHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectCommandHandlerTests"/> class.
    /// </summary>
    public CreateProjectCommandHandlerTests()
    {
        this.handler = new CreateProjectCommandHandler(
            this.projectRepositoryMock.Object,
            this.unitOfWorkMock.Object,
            this.mapperMock.Object);
    }

    /// <summary>
    /// Tests that a valid command calls the repository and returns the project ID.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithValidCommand_CallsRepositoryAndReturnsId()
    {
        // Arrange
        var command = ApplicationFakers.GenerateCreateProjectCommand();

        this.projectRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        this.unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await this.handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        this.projectRepositoryMock.Verify(r => r.AddAsync(It.Is<Project>(p => p.Name == command.Name && p.Description == command.Description), It.IsAny<CancellationToken>()), Times.Once);
        this.unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

/// <summary>
/// Unit tests for the UpdateProjectCommandHandler.
/// </summary>
public class UpdateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<IUnitOfWork> unitOfWorkMock = new();
    private readonly UpdateProjectCommandHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectCommandHandlerTests"/> class.
    /// </summary>
    public UpdateProjectCommandHandlerTests()
    {
        this.handler = new UpdateProjectCommandHandler(
            this.projectRepositoryMock.Object,
            this.unitOfWorkMock.Object);
    }

    /// <summary>
    /// Tests that updating an existing project works correctly.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithExistingProject_UpdatesAndSaves()
    {
        // Arrange
        var project = new ProjectFaker().Generate();
        var command = ApplicationFakers.GenerateUpdateProjectCommand();
        command = command with { Id = project.Id };

        this.projectRepositoryMock
            .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        this.unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await this.handler.HandleAsync(command, CancellationToken.None);

        // Assert
        this.projectRepositoryMock.Verify(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()), Times.Once);

        // EF Core auto-detects changes on tracked entity, Update() not called
        this.projectRepositoryMock.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);

        this.unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Verify the tracked entity was modified
        project.Name.Should().Be(command.Name);
        project.Description.Should().Be(command.Description);
        project.UpdatedAt.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that updating a non-existent project throws ProjectNotFoundException.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithNonExistentProject_ThrowsProjectNotFoundException()
    {
        // Arrange
        var command = ApplicationFakers.GenerateUpdateProjectCommand();

        this.projectRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act & Assert
        await this.handler.Invoking(h => h.HandleAsync(command, CancellationToken.None))
            .Should().ThrowAsync<Tfs.Portfolio.Domain.Projects.Exceptions.ProjectNotFoundException>();
    }
}

/// <summary>
/// Unit tests for the DeleteProjectCommandHandler.
/// </summary>
public class DeleteProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<IUnitOfWork> unitOfWorkMock = new();
    private readonly DeleteProjectCommandHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProjectCommandHandlerTests"/> class.
    /// </summary>
    public DeleteProjectCommandHandlerTests()
    {
        this.handler = new DeleteProjectCommandHandler(
            this.projectRepositoryMock.Object,
            this.unitOfWorkMock.Object);
    }

    /// <summary>
    /// Tests that deleting an existing project works correctly.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithExistingProject_DeletesAndSaves()
    {
        // Arrange
        var project = new ProjectFaker().Generate();
        var command = new DeleteProjectCommand(project.Id);

        this.projectRepositoryMock
            .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        this.unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await this.handler.HandleAsync(command, CancellationToken.None);

        // Assert
        this.projectRepositoryMock.Verify(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()), Times.Once);

        // Delete handler calls Delete to remove entity
        this.projectRepositoryMock.Verify(r => r.Delete(It.Is<Project>(p => p.Id == project.Id)), Times.Once);

        this.unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
