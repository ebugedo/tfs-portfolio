// <copyright file="ProjectCommands.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Commands;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Command to create a new project.
/// </summary>
public sealed record CreateProjectCommand(
    string Name,
    string? Description,
    YearMonth StartDate,
    int DurationMonths,
    Guid ClientId,
    Guid SectorId
) : ICommand<Guid>;

/// <summary>
/// Command to update an existing project.
/// </summary>
public sealed record UpdateProjectCommand(
    Guid Id,
    string Name,
    string? Description,
    YearMonth StartDate,
    int DurationMonths,
    Guid ClientId,
    Guid SectorId
) : ICommand;

/// <summary>
/// Command to delete a project.
/// </summary>
public sealed record DeleteProjectCommand(
    Guid Id
) : ICommand;

/// <summary>
/// Command to change a project's status.
/// </summary>
public sealed record ChangeProjectStatusCommand(
    Guid Id,
    ProjectStatus Status
) : ICommand;

/// <summary>
/// Command to add a service to a project.
/// </summary>
public sealed record AddProjectServiceCommand(
    Guid ProjectId,
    Guid ServiceId
) : ICommand;

/// <summary>
/// Command to remove a service from a project.
/// </summary>
public sealed record RemoveProjectServiceCommand(
    Guid ProjectId,
    Guid ServiceId
) : ICommand;