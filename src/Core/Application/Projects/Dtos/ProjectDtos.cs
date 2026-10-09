// <copyright file="ProjectDto.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Dtos;

using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Data transfer object for a project.
/// </summary>
public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    YearMonth StartDate,
    int DurationMonths,
    YearMonth EndDate,
    Guid ClientId,
    Guid SectorId,
    ProjectStatus Status,
    IReadOnlyList<string> Technologies,
    IReadOnlyList<Guid> ServiceIds,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Data transfer object for a project list item.
/// </summary>
public sealed record ProjectListItemDto(
    Guid Id,
    string Name,
    string? Description,
    YearMonth StartDate,
    int DurationMonths,
    YearMonth EndDate,
    Guid ClientId,
    Guid SectorId,
    ProjectStatus Status,
    DateTime CreatedAt
);

/// <summary>
/// Request to create a new project.
/// </summary>
public sealed record CreateProjectRequest(
    string Name,
    string? Description,
    YearMonth StartDate,
    int DurationMonths,
    Guid ClientId,
    Guid SectorId,
    IReadOnlyList<string> Technologies
);

/// <summary>
/// Request to update an existing project.
/// </summary>
public sealed record UpdateProjectRequest(
    string Name,
    string? Description,
    YearMonth StartDate,
    int DurationMonths,
    Guid ClientId,
    Guid SectorId,
    IReadOnlyList<string> Technologies,
    ProjectStatus Status
);