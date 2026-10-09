// <copyright file="ClientCommands.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Clients.Commands;

using System.Diagnostics.CodeAnalysis;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Newtonsoft.Json;

/// <summary>
/// Command to create a new client.
/// </summary>
[SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "URL is validated and converted to TfsWebUrl in handler")]
[SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "URL is validated and converted to TfsWebUrl in handler")]
public sealed record CreateClientCommand(
    string Name,
    string Email,
    string? LogoUrl,
    string? Phone,
    string? Address
) : ICommand<Guid>;

/// <summary>
/// Command to update an existing client.
/// </summary>
[SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "URL is validated and converted to TfsWebUrl in handler")]
[SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "URL is validated and converted to TfsWebUrl in handler")]
public sealed record UpdateClientCommand(
    Guid Id,
    string Name,
    string Email,
    string? LogoUrl,
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