// <copyright file="ProjectQueries.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Queries;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Projects.Dtos;

/// <summary>
/// Query to get a project by its identifier.
/// </summary>
public sealed record GetProjectByIdQuery(
    Guid Id
) : IQuery<ProjectDto>;

/// <summary>
/// Query to get all projects.
/// </summary>
public sealed record GetProjectsQuery
    : IQuery<IReadOnlyList<ProjectListItemDto>>;