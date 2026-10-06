// <copyright file="ProjectQueryHandlerTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Application.Projects.Queries;

using AutoMapper;
using FluentAssertions;
using Moq;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Projects.Handlers;
using Tfs.Portfolio.Application.Projects.Queries;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.UnitTests.Common;
using Xunit;

/// <summary>
/// Unit tests for the GetProjectByIdQueryHandler.
/// </summary>
public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly IMapper mapper;
    private readonly GetProjectByIdQueryHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectByIdQueryHandlerTests"/> class.
    /// </summary>
    public GetProjectByIdQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Project, ProjectDto>();
        });
        this.mapper = config.CreateMapper();

        this.handler = new GetProjectByIdQueryHandler(
            this.projectRepositoryMock.Object,
            this.mapper);
    }

    /// <summary>
    /// Tests that getting an existing project returns the DTO.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithExistingProject_ReturnsDto()
    {
        // Arrange
        var project = new ProjectFaker().Generate();
        var query = new GetProjectByIdQuery(project.Id);

        this.projectRepositoryMock
            .Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await this.handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(project.Id);
        result.Name.Should().Be(project.Name);
    }

    /// <summary>
    /// Tests that getting a non-existent project returns null.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithNonExistentProject_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectByIdQuery(Guid.NewGuid());

        this.projectRepositoryMock
            .Setup(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await this.handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

/// <summary>
/// Unit tests for the GetProjectsQueryHandler.
/// </summary>
public class GetProjectsQueryHandlerTests
{
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly IMapper mapper;
    private readonly GetProjectsQueryHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsQueryHandlerTests"/> class.
    /// </summary>
    public GetProjectsQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Project, ProjectListItemDto>();
        });
        this.mapper = config.CreateMapper();

        this.handler = new GetProjectsQueryHandler(
            this.projectRepositoryMock.Object,
            this.mapper);
    }

    /// <summary>
    /// Tests that getting all projects returns the list of DTOs.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WithProjects_ReturnsListOfDtos()
    {
        // Arrange
        var projects = new ProjectFaker().Generate(3);
        var query = new GetProjectsQuery();

        this.projectRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // Act
        var result = await this.handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }
}