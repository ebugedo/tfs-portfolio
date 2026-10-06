// <copyright file="ApplicationFakers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Common;

using Bogus;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;

/// <summary>
/// Factory for creating test commands and DTOs.
/// </summary>
public static class ApplicationFakers
{
    private static readonly Faker<CreateProjectCommand> CreateProjectCommandFaker = new Faker<CreateProjectCommand>()
        .CustomInstantiator(f => new CreateProjectCommand(
            f.Commerce.ProductName(),
            f.Lorem.Paragraph()));

    private static readonly Faker<UpdateProjectCommand> UpdateProjectCommandFaker = new Faker<UpdateProjectCommand>()
        .CustomInstantiator(f => new UpdateProjectCommand(
            f.Random.Guid(),
            f.Commerce.ProductName(),
            f.Lorem.Paragraph()));

    private static readonly Faker<DeleteProjectCommand> DeleteProjectCommandFaker = new Faker<DeleteProjectCommand>()
        .CustomInstantiator(f => new DeleteProjectCommand(f.Random.Guid()));

    private static readonly Faker<ProjectDto> ProjectDtoFaker = new Faker<ProjectDto>()
        .CustomInstantiator(f => new ProjectDto(
            f.Random.Guid(),
            f.Commerce.ProductName(),
            f.Lorem.Paragraph(),
            f.Date.Recent(),
            f.Date.Recent()));

    private static readonly Faker<ProjectListItemDto> ProjectListItemDtoFaker = new Faker<ProjectListItemDto>()
        .CustomInstantiator(f => new ProjectListItemDto(
            f.Random.Guid(),
            f.Commerce.ProductName(),
            f.Lorem.Paragraph(),
            f.Date.Recent()));

    /// <summary>
    /// Generates a valid CreateProjectCommand.
    /// </summary>
    public static CreateProjectCommand GenerateCreateProjectCommand()
    {
        return CreateProjectCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid UpdateProjectCommand.
    /// </summary>
    public static UpdateProjectCommand GenerateUpdateProjectCommand()
    {
        return UpdateProjectCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid DeleteProjectCommand.
    /// </summary>
    public static DeleteProjectCommand GenerateDeleteProjectCommand()
    {
        return DeleteProjectCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid ProjectDto.
    /// </summary>
    public static ProjectDto GenerateProjectDto()
    {
        return ProjectDtoFaker.Generate();
    }

    /// <summary>
    /// Generates a valid ProjectListItemDto.
    /// </summary>
    public static ProjectListItemDto GenerateProjectListItemDto()
    {
        return ProjectListItemDtoFaker.Generate();
    }
}