// <copyright file="ServiceCommands.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Services.Commands;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// Command to create a new service.
/// </summary>
public sealed record CreateServiceCommand(
    string Name,
    string? Description,
    ServiceCategory Category
) : ICommand<Guid>;

/// <summary>
/// Command to update an existing service.
/// </summary>
public sealed record UpdateServiceCommand(
    Guid Id,
    string Name,
    string? Description,
    ServiceCategory Category
) : ICommand;

/// <summary>
/// Command to delete a service.
/// </summary>
public sealed record DeleteServiceCommand(
    Guid Id
) : ICommand;