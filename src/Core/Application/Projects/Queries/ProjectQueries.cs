// <copyright file="ProjectQueries.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Queries;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Query to get a project by its identifier.
/// </summary>
public sealed record GetProjectByIdQuery(
    Guid Id
) : IQuery<ProjectDto>;

/// <summary>
/// Query to get all projects with optional filters.
/// </summary>
public sealed record GetProjectsQuery(
    Guid? ClientId = null,
    Guid? SectorId = null,
    string? Technology = null,
    ProjectStatus? Status = null
) : IQuery<IReadOnlyList<ProjectListItemDto>>;

/// <summary>
/// Query to get projects by client identifier.
/// </summary>
public sealed record GetProjectsByClientQuery(
    Guid ClientId
) : IQuery<IReadOnlyList<ProjectListItemDto>>;

/// <summary>
/// Query to get projects by sector identifier.
/// </summary>
public sealed record GetProjectsBySectorQuery(
    Guid SectorId
) : IQuery<IReadOnlyList<ProjectListItemDto>>;

/// <summary>
/// Query to get projects by technology name.
/// </summary>
public sealed record GetProjectsByTechnologyQuery(
    string TechnologyName
) : IQuery<IReadOnlyList<ProjectListItemDto>>;

/// <summary>
/// Query to get projects by status.
/// </summary>
public sealed record GetProjectsByStatusQuery(
    ProjectStatus Status
) : IQuery<IReadOnlyList<ProjectListItemDto>>;