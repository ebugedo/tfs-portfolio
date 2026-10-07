// <copyright file="ClientCommands.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Clients.Commands;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Command to create a new client.
/// </summary>
public sealed record CreateClientCommand(
    string Name,
    string Email,
    Url? LogoUrl,
    string? Phone,
    string? Address
) : ICommand<Guid>;

/// <summary>
/// Command to update an existing client.
/// </summary>
public sealed record UpdateClientCommand(
    Guid Id,
    string Name,
    string Email,
    Url? LogoUrl,
    string? Phone,
    string? Address,
    bool? IsActive
) : ICommand;

/// <summary>
/// Command to deactivate a client.
/// </summary>
public sealed record DeactivateClientCommand(
    Guid Id
) : ICommand;

/// <summary>
/// Command to delete a client.
/// </summary>
public sealed record DeleteClientCommand(
    Guid Id
) : ICommand;