// <copyright file="SectorCommands.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Sectors.Commands;

using Tfs.Portfolio.Application.Common.CQRS;

/// <summary>
/// Command to create a new sector.
/// </summary>
public sealed record CreateSectorCommand(
    string Name,
    string? Description
) : ICommand<Guid>;

/// <summary>
/// Command to update an existing sector.
/// </summary>
public sealed record UpdateSectorCommand(
    Guid Id,
    string Name,
    string? Description
) : ICommand;

/// <summary>
/// Command to delete a sector.
/// </summary>
public sealed record DeleteSectorCommand(
    Guid Id
) : ICommand;