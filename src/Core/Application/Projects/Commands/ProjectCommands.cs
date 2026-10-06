// <copyright file="ProjectCommands.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Commands;

using Tfs.Portfolio.Application.Common.CQRS;

/// <summary>
/// Command to create a new project.
/// </summary>
public sealed record CreateProjectCommand(
    string Name,
    string? Description
) : ICommand<Guid>;

/// <summary>
/// Command to update an existing project.
/// </summary>
public sealed record UpdateProjectCommand(
    Guid Id,
    string Name,
    string? Description
) : ICommand;

/// <summary>
/// Command to delete a project.
/// </summary>
public sealed record DeleteProjectCommand(
    Guid Id
) : ICommand;