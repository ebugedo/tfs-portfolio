// <copyright file="ClientDtos.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Clients.Dtos;

using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Data transfer object for a client.
/// </summary>
public sealed record ClientDto(
    Guid Id,
    string Name,
    string Email,
    TfsWebUrl? LogoUrl,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Data transfer object for a client list item.
/// </summary>
public sealed record ClientListItemDto(
    Guid Id,
    string Name,
    string Email,
    TfsWebUrl? LogoUrl,
    bool IsActive,
    DateTime CreatedAt
);

/// <summary>
/// Request to create a new client.
/// </summary>
public sealed record CreateClientRequest(
    string Name,
    string Email,
    TfsWebUrl? LogoUrl,
    string? Phone,
    string? Address
);

/// <summary>
/// Request to update an existing client.
/// </summary>
public sealed record UpdateClientRequest(
    string Name,
    string Email,
    TfsWebUrl? LogoUrl,
    string? Phone,
    string? Address,
    bool? IsActive
);